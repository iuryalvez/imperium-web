using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookie;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ICookieService cookie, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _cookie = cookie;
            _logger = logger;
        }

        public string GenerateTokenVinculo(string usuarioPerfilClinicaId, string usuarioIp, string usuarioAgent)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
                var now = DateTime.UtcNow;

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuarioPerfilClinicaId),
                        new Claim("UsuarioIP", usuarioIp),
                        new Claim("UsuarioAgent", usuarioAgent)
                    }),
                    Expires = now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                    NotBefore = now,
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                _logger.LogInformation("Token de vínculo gerado com sucesso para o usuário com ID de vínculo {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token de vínculo para o usuário com ID de vínculo {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                throw;
            }
        }

        public string GenerateTokenLogin(string usuarioId, string usuarioIp, string usuarioAgent)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
                var now = DateTime.UtcNow;

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuarioId),
                        new Claim("UsuarioIP", usuarioIp),
                        new Claim("UsuarioAgent", usuarioAgent),
                        new Claim("AcessoLogin", "true")
                    }),
                    Expires = now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                    NotBefore = now,
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                _logger.LogInformation("Token de login gerado com sucesso para o usuário com ID {UsuarioId}.", usuarioId);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token de login para o usuário com ID {UsuarioId}.", usuarioId);
                throw;
            }
        }
        public async Task RenovaToken(TokenValidatedContext context) 
        {
            var token = context.SecurityToken;
            var timeToExpire = token.ValidTo - DateTime.UtcNow;

            // Só roda se o token expirar em menos que o prazo configurado
            if (timeToExpire.TotalMinutes < double.Parse(_configuration["Jwt:RenewInMinutesLeft"]))
            {
                var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userIp = context.Principal.FindFirst("UserIP").Value;
                var userAgent = context.Principal.FindFirst("UserAgent").Value;
                var acessoLogin = context.Principal.FindFirst("AcessoLogin")?.Value;

                // gera o token correspondente ao que existia anteriormente
                string newToken = string.IsNullOrEmpty(acessoLogin)
                    ? GenerateTokenVinculo(userId, userIp, userAgent)
                    : GenerateTokenLogin(userId, userIp, userAgent);

                _cookie.SetCookieToken(context.HttpContext, newToken);
                Console.WriteLine($"Token renovado com sucesso para o usuário: {userId}");
            }
        }
    }
}
