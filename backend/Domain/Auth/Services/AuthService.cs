using backend.Infra.Data;
using backend.Domain.Auth.Interfaces;
using backend.Domain.Auth.DTOs;
using backend.Domain.Usuarios.DTOs;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Usuarios.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace backend.Domain.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly ITurnstileService _turnstileService;
        private readonly ITokenService _tokenService;

        public AuthService(
            IConfiguration configuration, 
            IDbContextFactory<AppDbContext> contextFactory, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<AuthService> logger,
            UserManager<Usuario> userManager,
            ITurnstileService turnstileService,
            ITokenService tokenService)
        {
            _configuration = configuration;
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _userManager = userManager;
            _turnstileService = turnstileService;
            _tokenService = tokenService;
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
                .Where(p => p.Perfis != null && p.Perfis.Any(perfil => perfil.ToUpper() != "INATIVO"))
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
    }
}