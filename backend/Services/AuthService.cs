using backend.Infra.Data;
using backend.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, IDbContextFactory<AppDbContext> contextFactory, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
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
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token não pode ser nulo ou vazio.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

            try
            {
                // Validação do token
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
                // Você pode adicionar logs aqui se necessário
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

            var tokenIpClaim = principal.FindFirst("UsuarioIP")?.Value;
            var tokenUserAgentClaim = principal.FindFirst("UsuarioAgent")?.Value;
            var acessoLogin = principal.FindFirst("acessoLogin")?.Value;

            // In local environments, IPv4 and IPv6 loopback addresses might alternate.
            if (tokenIpClaim != currentIp && !(tokenIpClaim == "::1" && currentIp == "127.0.0.1") && !(tokenIpClaim == "127.0.0.1" && currentIp == "::1"))
            {
                throw new SecurityTokenException("IP não corresponde ao token.");
            }

            if (tokenUserAgentClaim != currentUserAgent)
            {
                throw new SecurityTokenException("Navegador não corresponde ao token.");
            }

            // Se for apenas o token de login inicial, a validação de segurança (IP/Agent) já é suficiente.
            // The login token does not contain 'Perfis' nor 'UsuarioPerfilClinicaId', so we exit early.
            if (acessoLogin == "true")
            {
                return;
            }

            // TOKEN DE VÍNCULO.
            bool isAdmin = principal.GetClaimValue<bool>("isAdmin");
            var upcIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfisClaim = principal.FindFirst("Perfis")?.Value;

            if (isAdmin)
            {
                if (string.IsNullOrEmpty(perfisClaim))
                {
                    throw new SecurityTokenException("Administrador sem perfis atribuídos no token de vínculo.");
                }
            }
            else
            {
                // Usuário comum: Precisa validar o vínculo no banco
                if (!int.TryParse(upcIdClaim, out int upcId))
                {
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
                        Console.WriteLine($"Erro: Vínculo com ID {upcId} não encontrado.");
                        throw new SecurityTokenException("Vínculo do usuário não encontrado no banco de dados.");
                    }

                    // Obtém os perfis ativos do dicionário
                    var perfisAtivos = upc.Perfis;

                    Console.WriteLine($"Usuário autenticado com perfis: {string.Join(", ", perfisAtivos)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao validar vínculo no banco de dados: {ex.Message}");
                    throw new SecurityTokenException("Erro interno ao validar o vínculo.");
                }
            }
        }
    }
}
