using backend.Domain.Auth.Extensions;
using backend.Infra.Data;
using backend.Domain.Auth.Interfaces;
using backend.Domain.Auth.DTOs;
using backend.Domain.Usuarios.DTOs.Usuario;
using backend.Domain.Usuarios.DTOs.Vinculo;
using backend.Domain.Usuarios.Interfaces;
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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly ITurnstileService _turnstileService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(
            IConfiguration configuration,
            IDbContextFactory<AppDbContext> contextFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger,
            UserManager<Usuario> userManager,
            ITurnstileService turnstileService,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _userManager = userManager;
            _turnstileService = turnstileService;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<ServiceResult> LoginAsync(LoginDto model, string remoteIp, string userAgent)
        {
            var captchaValido = await _turnstileService.ValidarTokenAsync(model.CaptchaToken, remoteIp);

            if (!captchaValido)
                return new ServiceResult { Succeeded = false, Message = "Falha na verificação do captcha. Recarregue a página e tente novamente." };

            var usuario = await _userManager.FindByEmailAsync(model.Email);

            if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, model.Senha))
            {
                _logger.LogWarning("Falha de autenticação para o e-mail {Email}.", model.Email);
                return new ServiceResult { Succeeded = false, Message = "E-mail ou senha inválidos." };
            }

            if (!usuario.EmailConfirmed)
                return new ServiceResult { Succeeded = false, Message = "Confirme seu email antes de fazer login." };

            using var context = _contextFactory.CreateDbContext();

            // Attach the entity to the new DbContext instance before updating
            context.Usuarios.Attach(usuario);
            usuario.DataUltimoAcesso = DateTime.UtcNow;
            await context.SaveChangesAsync();

            var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), remoteIp, userAgent);

            return new ServiceResult
            {
                Succeeded = true,
                Data = tokenLogin
            };
        }

        public async Task<ServiceResult> GetVinculosAsync(string userIdClaim)
        {
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId) || usuarioId == 0)
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

        public async Task<ServiceResult> SelecionarVinculoAsync(int upcId, string userIdClaim, bool isAcessoLogin, string remoteIp, string userAgent)
        {
            using var context = _contextFactory.CreateDbContext();
            int usuarioId;

            // Resolves the true User ID depending on whether the current token is a base access token or an already established linkage token
            if (isAcessoLogin)
            {
                usuarioId = int.Parse(userIdClaim);
            }
            else
            {
                var vinculoIdClaim = int.Parse(userIdClaim);
                var vinculoAtual = await context.UsuariosPerfisClinicas.FirstOrDefaultAsync(v => v.Id == vinculoIdClaim);

                if (vinculoAtual == null)
                    return new ServiceResult { Succeeded = false, Message = "Vínculo atual não encontrado." };

                usuarioId = vinculoAtual.UsuarioId;
            }

            var vinculo = await context.UsuariosPerfisClinicas
                .AsNoTracking()
                .Include(v => v.Usuario)
                .Include(v => v.Clinica)
                .FirstOrDefaultAsync(v => v.Id == upcId && v.UsuarioId == usuarioId && !v.Perfis.Contains("INATIVO"));

            if (vinculo == null)
                return new ServiceResult { Succeeded = false, Message = "Vínculo não encontrado ou não pertence ao mesmo usuário." };

            var token = _tokenService.GenerateTokenVinculo(vinculo.Id.ToString(), remoteIp, userAgent);

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

        public async Task<ServiceResult> SelecionarVinculoAdminAsync(GetVinculoAdminDto dto, string userIdClaim, string remoteIp, string userAgent, string usuarioNome)
        {
            // Perfis simuláveis: os da clínica. INATIVO não é perfil de acesso (bloqueia o vínculo inteiro)
            var perfisValidos = new[] { "GESTOR", "VETERINARIO", "SECRETARIO", "TECNICO" };
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

            var token = _tokenService.GenerateTokenVinculoAdmin(dto.ClinicaId, dto.Perfis, remoteIp, userAgent);

            return new ServiceResult { Succeeded = true, Data = new { Token = token, ClinicaId = dto.ClinicaId, PerfisAtivos = dto.Perfis } };
        }

        public async Task<ServiceResult> TrocarVinculoAsync(string userIdClaim, bool isAcessoLogin, bool isAdmin, string remoteIp, string userAgent)
        {
            if (isAcessoLogin)
                return new ServiceResult { Succeeded = false, Message = "Token atual já é de login." };

            string usuarioId;

            if (isAdmin)
            {
                usuarioId = userIdClaim;
            }
            else
            {
                var vinculoId = int.Parse(userIdClaim);
                using var context = _contextFactory.CreateDbContext();

                var vinculo = await context.UsuariosPerfisClinicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == vinculoId);

                if (vinculo == null)
                    return new ServiceResult { Succeeded = false, Message = "Vínculo não encontrado." };

                usuarioId = vinculo.UsuarioId.ToString();
            }

            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario == null)
                return new ServiceResult { Succeeded = false, Message = "Usuário não encontrado." };

            var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), remoteIp, userAgent);

            return new ServiceResult { Succeeded = true, Data = tokenLogin };
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto model, string frontendUrl)
        {
            var usuario = await _userManager.FindByEmailAsync(model.Email);
            if (usuario == null)
            {
                // Retornar sucesso falso (no controller tratamos para exibir msg genérica)
                return new ServiceResult { Succeeded = true, Message = "Se o e-mail estiver registrado, um link será enviado." };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

            // Codifica o token para ser seguro na URL
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Monta o link para o Frontend
            var resetLink = $"{frontendUrl}/recuperar-senha/nova-senha?token={encodedToken}&email={usuario.Email}";

            var corpoEmail = $@"
                <h2>Redefinição de Senha - Sistema IMPERIUM</h2>
                <p>Olá,</p>
                <p>Você solicitou a redefinição da sua senha. Clique no link abaixo para criar uma nova senha:</p>
                <p><a href='{resetLink}'>Redefinir minha senha</a></p>
                <p>Se você não solicitou isso, pode ignorar este e-mail com segurança. O link expira em 15 minutos.</p>";

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                _logger.LogWarning("====================================================================");
                _logger.LogWarning("MOCK DE E-MAIL (Ambiente de Desenvolvimento)");
                _logger.LogWarning("Link gerado: {ResetLink}", resetLink);
                _logger.LogWarning("====================================================================");
            }
            else
            {
                // Em produção, dispara o e-mail real
                await _emailService.SendEmailAsync(usuario.Email, "Redefinição de Senha - IMPERIUM", corpoEmail);
            }

            return new ServiceResult { Succeeded = true, Message = "Se o e-mail estiver registrado, um link será enviado." };
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            var usuario = await _userManager.FindByEmailAsync(model.Email);
            if (usuario == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Erro ao redefinir senha." }; // Mensagem genérica
            }

            // Decodifica o token seguro da URL de volta para o formato original do Identity
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
            var originalToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ResetPasswordAsync(usuario, originalToken, model.NewPassword);

            if (result.Succeeded)
            {
                return new ServiceResult { Succeeded = true, Message = "Senha redefinida com sucesso." };
            }

            return new ServiceResult
            {
                Succeeded = false,
                Message = "O link de recuperação é inválido ou expirou. Solicite um novo."
            };
        }
    }
}