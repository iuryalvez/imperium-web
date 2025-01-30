using backend.Data;
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

        // Valida as informações do usuário contidas no token
        public async Task<bool> ValidaUsuario(string token)
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();
                // Extrai informações do token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    _logger.LogWarning("Token inválido na validação de usuário.");
                    return false;
                }

                // Obtém os valores das claims no token
                var acessoLogin = jwtToken.Claims.FirstOrDefault(c => c.Type == "AcessoLogin")?.Value;
                var usuarioPerfilClinicaClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                // Verifica se o token é de um usuário que acabou de fazer login (pula a validação)
                if (string.IsNullOrEmpty(acessoLogin))
                {
                    _logger.LogInformation("Token de login detectado, pulando validação.");
                    return true;
                }

                // Valida se o ID do vinculo é um valor numérico válido
                if (!int.TryParse(usuarioPerfilClinicaClaim, out int usuarioPerfilClinicaId))
                {
                    _logger.LogWarning("Falha ao converter o valor da claim 'NameIdentifier' para int. Valor: {Value}", usuarioPerfilClinicaClaim);
                    return false;
                }

                // Verifica se o vínculo ainda existe no banco
                var vinculo = await _context.UsuariosPerfisClinicas
                    .FirstOrDefaultAsync(upc => upc.Id == usuarioPerfilClinicaId);

                if (vinculo == null)
                {
                    _logger.LogWarning("Vínculo não encontrado para o ID {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                    return false;
                }

                _logger.LogInformation("Token válido para o vínculo com ID {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar o token.");
                return false;
            }
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
            var acessoLogin = principal.FindFirst("acessoLogin")?.Value;

            var tokenIpClaim = principal.FindFirst("UsuarioIP")?.Value;
            var tokenUserAgentClaim = principal.FindFirst("UsuarioAgent")?.Value;
            var upcIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Valida se o ID do vinculo é um valor numérico válido
            if (!int.TryParse(upcIdClaim, out int upcId)){
                throw new SecurityTokenException("O Id contido no token não é um valor numérico válido!");
            }

            if (tokenIpClaim != currentIp || tokenUserAgentClaim != currentUserAgent)
            {
                throw new SecurityTokenException("IP ou navegador não correspondem ao token.");
            }
            
            // Valida se o vínculo ainda existe no banco e se sim define sua Role no contexto da requisição
            if (string.IsNullOrEmpty(acessoLogin)) {
                try
                {
                    using var _context = _contextFactory.CreateDbContext();
                    var upc = await _context.UsuariosPerfisClinicas
                    .Include(upc => upc.Perfil)
                    .Include(upc => upc.Clinica)
                    .FirstOrDefaultAsync(upc => upc.Id == upcId);
    
                    if (upc == null)
                    {
                        Console.WriteLine($"Erro: Vínculo com ID {upcId} não encontrado.");
                        Console.WriteLine("Id do vínculo presente no token não existe no banco de dados!");
                    }

                    // tirei a definição da role no contexto que estava aqui e 
                    // coloquei na geração do token de vínculo
                    // porque na verdade não estava sendo adicionada e 
                    // não estava validando o authorize corretamente.
                    
                    Console.WriteLine($"\nRole do usuário: {upc.Perfil.Name}");
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
