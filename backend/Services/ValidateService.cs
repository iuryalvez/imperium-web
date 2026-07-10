using backend.Infra.Data;
using backend.Interfaces;
using backend.Domain.Auth.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public class ValidateService : IValidateService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ValidateService> _logger;

        public ValidateService(IConfiguration configuration, IDbContextFactory<AppDbContext> contextFactory, IHttpContextAccessor httpContextAccessor, ILogger<ValidateService> logger)
        {
            _configuration = configuration;
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Valida o token JWT contido no cookie e retorna o ClaimsPrincipal.
        /// </summary>
        /// <param name="token">Token JWT extraído do cookie.</param>
        /// <returns>ClaimsPrincipal se o token for válido; caso contrário, lança uma exceção.</returns>
        /// <exception cref="SecurityTokenException">Lança exceção se o token for inválido.</exception>
        public ClaimsPrincipal ValidateTokenFromCookie(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token), "Token não pode ser nulo ou vazio.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[ValidateService] Falha ao validar a assinatura/validade do token.");
                throw new SecurityTokenException("Falha ao validar o token.", ex);
            }
        }

        /// <summary>
        /// Valida o usuário com base nas claims do token e adiciona perfil ao contexto da requisição.
        /// </summary>
        /// <param name="principal">ClaimsPrincipal extraído do token.</param>
        /// <param name="httpContext">Contexto HTTP para validações adicionais.</param>
        /// <exception cref="SecurityTokenException">Lança exceção se a validação falhar.</exception>
        public async Task ValidateUserClaims(ClaimsPrincipal principal, HttpContext httpContext)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                throw new SecurityTokenException("Usuário não autenticado.");
            }

            var currentIp = httpContext.Connection.RemoteIpAddress?.ToString();
            var currentUserAgent = httpContext.Request.Headers["User-Agent"].ToString();

            bool isAtivo = principal.GetClaimValue<bool>("isAtivo");
            var tokenIpClaim = principal.FindFirst("UsuarioIp")?.Value;
            var tokenUserAgentClaim = principal.FindFirst("UsuarioAgent")?.Value;
            var acessoLogin = principal.FindFirst("acessoLogin")?.Value;

            _logger.LogInformation("[ValidateService] Validação de Claims iniciada. IP Atual: {CurrentIP}, IP Token: {TokenIP}, Agent Atual: {CurrentAgent}, Agent Token: {TokenAgent}, Acesso Login: {AcessoLogin}",
                currentIp, tokenIpClaim, currentUserAgent, tokenUserAgentClaim, acessoLogin);

            if (tokenIpClaim != currentIp && !(tokenIpClaim == "::1" && currentIp == "127.0.0.1") && !(tokenIpClaim == "127.0.0.1" && currentIp == "::1"))
            {
                _logger.LogWarning("[ValidateService] Rejeitado: IP não corresponde. Token: {TokenIP}, Requisição: {CurrentIP}", tokenIpClaim, currentIp);
                throw new SecurityTokenException("IP não corresponde ao token.");
            }

            if (tokenUserAgentClaim != currentUserAgent)
            {
                _logger.LogWarning("[ValidateService] Rejeitado: Navegador não corresponde.");
                throw new SecurityTokenException("Navegador não corresponde ao token.");
            }

            if (acessoLogin == "true")
            {
                _logger.LogInformation("[ValidateService] Token de login básico (apenas sistema) validado com sucesso.");
                return;
            }

            bool isAdmin = principal.GetClaimValue<bool>("isAdmin");
            var upcIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfisClaim = principal.FindFirst("Perfis")?.Value;

            _logger.LogInformation("[ValidateService] Validando token de vínculo. IsAdmin: {IsAdmin}, Perfil/VinculoId: {UpcId}, Perfis: {Perfis}", isAdmin, upcIdClaim, perfisClaim);

            if (isAdmin)
            {
                if (string.IsNullOrEmpty(perfisClaim))
                {
                    _logger.LogWarning("[ValidateService] Administrador não possui perfis definidos no token de simulação.");
                    throw new SecurityTokenException("Administrador sem perfis atribuídos no token de vínculo.");
                }
                _logger.LogInformation("[ValidateService] Administrador validado com sucesso para simulação de clínica.");
            }
            else
            {
                if (!int.TryParse(upcIdClaim, out int upcId))
                {
                    _logger.LogWarning("[ValidateService] Id de Vínculo não numérico.");
                    throw new SecurityTokenException("O Id contido no token não é um valor numérico válido!");
                }

                try
                {
                    using var _context = _contextFactory.CreateDbContext();
                    var upc = await _context.UsuariosPerfisClinicas
                        .Include(upc => upc.Clinica)
                        .FirstOrDefaultAsync(upc => upc.Id == upcId);

                    if (upc == null)
                    {
                        _logger.LogError("[ValidateService] Vínculo ID {upcId} não encontrado no banco de dados.", upcId);
                        throw new SecurityTokenException("Vínculo do usuário não encontrado no banco de dados.");
                    }

                    _logger.LogInformation("[ValidateService] Vínculo comum validado com sucesso. Perfis ativos: {Perfis}", string.Join(", ", upc.Perfis ?? new List<string>()));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[ValidateService] Erro ao buscar vínculo no banco de dados.");
                    throw new SecurityTokenException("Erro interno ao validar o vínculo.");
                }
            }
        }
    }
}
