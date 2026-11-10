using backend.Domain.Auth.Extensions;
using backend.Infra.Data;
using backend.Domain.Auth.Interfaces;
using backend.Domain.Auth.DTOs;
using backend.Domain.Usuarios.DTOs.Vinculo;
using backend.Domain.Usuarios.Services;
using backend.Domain.Utils;
using backend.Domain.Usuarios.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace backend.Domain.Auth.Services
{
    public class AuthService : IAuthService
    {
        // Uma mensagem só para e-mail inexistente, senha errada e conta bloqueada: não revela qual foi o caso
        private const string MensagemLoginInvalido =
            "E-mail ou senha inválidos. Após 5 tentativas seguidas, o acesso fica bloqueado por 15 minutos.";
        private const string MensagemCaptcha = "Falha na verificação do captcha. Recarregue a página e tente novamente.";
        private const string MensagemLinkEnviado = "Se o e-mail estiver registrado, um link será enviado.";
        private const string MensagemLinkInvalido = "O link de recuperação é inválido ou expirou. Solicite um novo.";

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly ITurnstileService _turnstileService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IHostEnvironment _ambiente;

        public AuthService(
            IDbContextFactory<AppDbContext> contextFactory,
            ILogger<AuthService> logger,
            UserManager<Usuario> userManager,
            ITurnstileService turnstileService,
            ITokenService tokenService,
            IEmailService emailService,
            IHostEnvironment ambiente)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _userManager = userManager;
            _turnstileService = turnstileService;
            _tokenService = tokenService;
            _emailService = emailService;
            _ambiente = ambiente;
        }

        public async Task<ServiceResult> LoginAsync(LoginDto model, string remoteIp)
        {
            if (!await _turnstileService.ValidarTokenAsync(model.CaptchaToken, remoteIp))
                return ServiceResult.Invalido(MensagemCaptcha);

            var usuario = await _userManager.FindByEmailAsync(model.Email);
            if (usuario == null)
            {
                _logger.LogWarning("Falha de autenticação: e-mail não cadastrado.");
                return ServiceResult.Invalido(MensagemLoginInvalido);
            }

            // Usuários criados antes do bloqueio de conta existir estão com ele desligado
            if (!usuario.LockoutEnabled)
                await _userManager.SetLockoutEnabledAsync(usuario, true);

            if (await _userManager.IsLockedOutAsync(usuario))
            {
                _logger.LogWarning("Login recusado: usuário {UsuarioId} está bloqueado por excesso de tentativas.", usuario.Id);
                return ServiceResult.Invalido(MensagemLoginInvalido);
            }

            if (!await _userManager.CheckPasswordAsync(usuario, model.Senha))
            {
                await _userManager.AccessFailedAsync(usuario); // conta a falha e bloqueia ao atingir o limite
                _logger.LogWarning("Falha de autenticação para o usuário {UsuarioId}.", usuario.Id);
                return ServiceResult.Invalido(MensagemLoginInvalido);
            }

            if (!usuario.EmailConfirmed)
                return ServiceResult.Invalido("Confirme seu email antes de fazer login.");

            if (!usuario.IsAtivo)
                return ServiceResult.Invalido("Usuário inativo. Procure o gestor da clínica.");

            usuario.AccessFailedCount = 0;
            usuario.DataUltimoAcesso = DateTime.UtcNow;
            await _userManager.UpdateAsync(usuario);

            try
            {
                var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), model.RememberMe);
                return new ServiceResult { Succeeded = true, Data = tokenLogin };
            }
            catch (InvalidOperationException)
            {
                return ServiceResult.Invalido("Nenhum vínculo disponível ou ativo no momento.");
            }
        }

        public async Task LogoutAsync(int usuarioId)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId.ToString());
            if (usuario != null)
                await _userManager.UpdateSecurityStampAsync(usuario);
        }

        public async Task<ServiceResult> GetVinculosAsync(int usuarioId)
        {
            if (usuarioId <= 0)
                return new ServiceResult { Succeeded = false, Message = "Erro ao identificar usuário." };

            using var context = _contextFactory.CreateDbContext();

            var vinculosDb = await context.UsuariosPerfisClinicas
                .Include(p => p.Clinica)
                .AsNoTracking()
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();

            var vinculos = vinculosDb
                // Vínculo com o perfil INATIVO não aparece para escolha, mesmo que tenha outros perfis
                .Where(p => p.Perfis != null
                         && p.Perfis.Any()
                         && !p.Perfis.Any(perfil => perfil.ToUpper() == AuthExtension.PerfilInativo))
                .Select(p => new GetVinculoDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    ClinicaId = p.ClinicaId,
                    NomeClinica = p.Clinica?.NomeClinica ?? "Clínica Desconhecida",
                    Perfis = p.Perfis
                })
                .ToList();

            if (!vinculos.Any())
                return new ServiceResult { Succeeded = false, Message = "Nenhum vínculo disponível ou ativo no momento." };

            return new ServiceResult { Succeeded = true, Data = vinculos };
        }

        public async Task<ServiceResult> SelecionarVinculoAsync(int upcId, int usuarioId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Só vale vínculo ativo do próprio usuário
            var vinculo = await context.UsuariosPerfisClinicas
                .AsNoTracking()
                .Include(v => v.Usuario)
                .Include(v => v.Clinica)
                .FirstOrDefaultAsync(v => v.Id == upcId && v.UsuarioId == usuarioId && !v.Perfis.Contains("INATIVO"));

            if (vinculo == null)
                return new ServiceResult { Succeeded = false, Message = "Vínculo não encontrado ou não pertence ao mesmo usuário." };

            var token = _tokenService.GenerateTokenVinculo(vinculo.Id.ToString());

            var vinculoDto = new GetVinculoDto
            {
                Id = vinculo.Id,
                UsuarioId = vinculo.UsuarioId,
                ClinicaId = vinculo.ClinicaId,
                NomeClinica = vinculo.Clinica?.NomeClinica,
                Perfis = vinculo.Perfis
            };

            return new ServiceResult { Succeeded = true, Data = new { Token = token, Vinculo = vinculoDto } };
        }

        public async Task<ServiceResult> SelecionarVinculoAdminAsync(GetVinculoAdminDto dto)
        {
            // Perfis simuláveis: os da clínica. INATIVO não é perfil de acesso (bloqueia o vínculo inteiro)
            var perfisValidos = AuthExtension.PerfisDaClinica;
            if (dto.Perfis == null || !dto.Perfis.Any())
                return new ServiceResult { Succeeded = false, Message = "Selecione pelo menos um perfil para simular.", Errors = new List<string> { "Perfis" } };
            if (dto.Perfis.Any(p => string.Equals(p, AuthExtension.PerfilInativo, StringComparison.OrdinalIgnoreCase)))
                return new ServiceResult { Succeeded = false, Message = "O perfil INATIVO não pode ser simulado: um vínculo inativo não tem acesso à clínica.", Errors = new List<string> { "Perfis" } };
            if (dto.Perfis.Any(p => !perfisValidos.Contains(p)))
                return new ServiceResult { Succeeded = false, Message = "Perfil inválido. Use GESTOR, VETERINARIO, SECRETARIO ou TECNICO.", Errors = new List<string> { "Perfis" } };

            using var context = _contextFactory.CreateDbContext();

            var clinica = await context.Clinicas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == dto.ClinicaId);
            if (clinica == null)
                return new ServiceResult { Succeeded = false, Message = "Clínica não encontrada." };

            var token = _tokenService.GenerateTokenVinculoAdmin(dto.ClinicaId, dto.Perfis);

            return new ServiceResult { Succeeded = true, Data = new { Token = token, ClinicaId = dto.ClinicaId, PerfisAtivos = dto.Perfis } };
        }

        public Task<ServiceResult> TrocarVinculoAsync(int usuarioId, bool isAcessoLogin)
        {
            if (isAcessoLogin)
                return Task.FromResult(new ServiceResult { Succeeded = false, Message = "Token atual já é de login." });

            try
            {
                // Volta ao token de login, mantendo "lembrar-me" e o início da sessão atual
                var tokenLogin = _tokenService.GenerateTokenLogin(usuarioId.ToString());
                return Task.FromResult(new ServiceResult { Succeeded = true, Data = tokenLogin });
            }
            catch (InvalidOperationException)
            {
                return Task.FromResult(new ServiceResult { Succeeded = false, Message = "Usuário não encontrado." });
            }
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto model, string frontendUrl, string remoteIp)
        {
            if (!await _turnstileService.ValidarTokenAsync(model.CaptchaToken, remoteIp))
                return ServiceResult.Invalido(MensagemCaptcha);

            // A resposta é sempre a mesma: não revela se o e-mail está cadastrado
            var resposta = new ServiceResult { Succeeded = true, Message = MensagemLinkEnviado };

            var usuario = await _userManager.FindByEmailAsync(model.Email);
            if (usuario == null || !usuario.IsAtivo || string.IsNullOrEmpty(usuario.Email))
                return resposta;

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // O link leva o id do usuário, não o e-mail (dado pessoal não fica em URL, histórico nem log de proxy)
            var resetLink = $"{frontendUrl?.TrimEnd('/')}/recuperar-senha/nova-senha?token={encodedToken}&uid={usuario.Id}";

            if (SimularEmail)
            {
                _logger.LogWarning("MOCK DE E-MAIL (Development sem SMTP). Link de redefinição: {ResetLink}", resetLink);
                return resposta;
            }

            var corpoEmail = $@"
                <h2>Redefinição de Senha - Sistema IMPERIUM</h2>
                <p>Olá,</p>
                <p>Você solicitou a redefinição da sua senha. Clique no link abaixo para criar uma nova senha:</p>
                <p><a href='{resetLink}'>Redefinir minha senha</a></p>
                <p>Se você não solicitou isso, pode ignorar este e-mail com segurança. O link expira em 15 minutos.</p>";

            EnviarEmailEmSegundoPlano(usuario.Email, "Redefinição de Senha - IMPERIUM", corpoEmail);
            return resposta;
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            var usuario = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (usuario == null || !usuario.IsAtivo)
                return ServiceResult.Invalido(MensagemLinkInvalido);

            string tokenOriginal;
            try
            {
                tokenOriginal = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            }
            catch (FormatException)
            {
                return ServiceResult.Invalido(MensagemLinkInvalido);
            }

            // O Identity troca o carimbo de segurança junto com a senha: todas as sessões abertas caem
            var result = await _userManager.ResetPasswordAsync(usuario, tokenOriginal, model.NewPassword);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code == "InvalidToken"))
                    return ServiceResult.Invalido(MensagemLinkInvalido);

                // Senha fora da regra: o usuário precisa saber o que corrigir
                var erros = result.Errors.Select(UsuarioService.TraduzirErroSenhaIdentity).ToList();
                return new ServiceResult { Succeeded = false, Message = string.Join(" ", erros), Errors = erros };
            }

            // Quem provou ser dono do e-mail volta a poder entrar, mesmo se estava bloqueado por tentativas
            await _userManager.ResetAccessFailedCountAsync(usuario);
            await _userManager.SetLockoutEndDateAsync(usuario, null);

            if (!SimularEmail && !string.IsNullOrEmpty(usuario.Email))
            {
                EnviarEmailEmSegundoPlano(usuario.Email, "Sua senha foi alterada - IMPERIUM", @"
                    <h2>Senha alterada - Sistema IMPERIUM</h2>
                    <p>Olá,</p>
                    <p>A senha da sua conta acabou de ser redefinida e as sessões abertas foram encerradas.</p>
                    <p>Se não foi você, redefina a senha novamente e avise o administrador do sistema.</p>");
            }

            return new ServiceResult { Succeeded = true, Message = "Senha redefinida com sucesso." };
        }

        /// <summary>
        /// Dispara o e-mail sem esperar o SMTP, para o tempo de resposta não revelar se o e-mail existe.
        /// A falha de envio fica só no log.
        /// </summary>
        /// <summary>
        /// Só em Development e sem SMTP configurado o e-mail vira log. Fora de Development o link nunca vai para o log.
        /// </summary>
        private bool SimularEmail => _ambiente.IsDevelopment() && !_emailService.Configurado;

        private void EnviarEmailEmSegundoPlano(string para, string assunto, string corpo)
        {
            var emailService = _emailService;
            var logger = _logger;

            _ = Task.Run(async () =>
            {
                try
                {
                    await emailService.SendEmailAsync(para, assunto, corpo);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Falha ao enviar e-mail em segundo plano.");
                }
            });
        }
    }
}
