using backend.Infra.Data;
using backend.Interfaces;
using backend.Domain.Auth.Extensions;
using backend.Domain.Auth.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;
using System.Text.Json;

namespace backend.Services
{
    public class ValidateService : IValidateService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogger<ValidateService> _logger;

        public ValidateService(IDbContextFactory<AppDbContext> contextFactory, ILogger<ValidateService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        /// <summary>
        /// Confere no banco, a cada requisição, se a sessão do token ainda vale:
        /// usuário existente e ativo, carimbo de segurança igual ao do token (muda no logout e na troca de senha)
        /// e, no token de vínculo, vínculo existente e ativo. Os perfis do principal passam a ser os do banco,
        /// para que perfil removido ou acrescentado valha na requisição seguinte.
        /// </summary>
        /// <exception cref="SecurityTokenException">Quando a sessão não vale mais.</exception>
        public async Task ValidateUserClaims(ClaimsPrincipal principal, HttpContext httpContext)
        {
            if (principal?.Identity is not ClaimsIdentity identidade || !identidade.IsAuthenticated)
                throw new SecurityTokenException("Usuário não autenticado.");

            var usuarioId = principal.GetUsuarioId()
                ?? throw new SecurityTokenException("Token sem identificação do usuário.");

            var stampDoToken = principal.FindFirst(TokenService.ClaimStamp)?.Value;
            if (string.IsNullOrEmpty(stampDoToken))
                throw new SecurityTokenException("Token sem carimbo de segurança.");

            await using var context = await _contextFactory.CreateDbContextAsync();

            var usuario = await context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == usuarioId)
                .Select(u => new { u.IsAtivo, u.IsAdmin, u.SecurityStamp })
                .FirstOrDefaultAsync();

            if (usuario == null || !usuario.IsAtivo)
                throw new SecurityTokenException("Usuário inexistente ou inativo.");

            if (!string.Equals(usuario.SecurityStamp, stampDoToken, StringComparison.Ordinal))
                throw new SecurityTokenException("Sessão encerrada.");

            // O token nunca dá mais do que o banco: quem deixou de ser admin perde o acesso de admin na hora
            var tokenDeAdmin = principal.GetClaimValue<bool>("isAdmin");
            if (tokenDeAdmin && !usuario.IsAdmin)
                throw new SecurityTokenException("Usuário não é mais administrador.");

            // Token de login (antes de escolher o vínculo): não há vínculo para conferir
            if (principal.FindFirst("acessoLogin")?.Value == "true") return;

            if (tokenDeAdmin)
            {
                // Admin simulando acesso: os perfis são os escolhidos na simulação
                if (string.IsNullOrEmpty(principal.FindFirst("Perfis")?.Value))
                    throw new SecurityTokenException("Administrador sem perfis no token de simulação.");
                return;
            }

            if (!int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int vinculoId))
                throw new SecurityTokenException("Token sem identificação do vínculo.");

            var vinculo = await context.UsuariosPerfisClinicas
                .AsNoTracking()
                .Where(upc => upc.Id == vinculoId && upc.UsuarioId == usuarioId)
                .Select(upc => new { upc.Perfis })
                .FirstOrDefaultAsync();

            if (vinculo == null)
                throw new SecurityTokenException("Vínculo do usuário não encontrado.");

            var perfis = vinculo.Perfis ?? new List<string>();

            // Vínculo desativado depois de o token ser emitido: o token antigo deixa de valer na hora
            if (perfis.Any(p => string.Equals(p, AuthExtension.PerfilInativo, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Token recusado: vínculo {VinculoId} está inativo.", vinculoId);
                throw new SecurityTokenException("Vínculo do usuário está inativo.");
            }

            foreach (var claim in identidade.FindAll("Perfis").ToList()) identidade.RemoveClaim(claim);
            identidade.AddClaim(new Claim("Perfis", JsonSerializer.Serialize(perfis)));
        }
    }
}
