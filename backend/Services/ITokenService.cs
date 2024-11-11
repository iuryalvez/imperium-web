using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services
{
    public interface ITokenService
    {
        string GenerateToken(string usuarioPerfilClinicaId);
        string GenerateTokenLogin(string usuarioId, string usuarioEmail);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string usuarioPerfilClinicaId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioPerfilClinicaId),
                    new Claim("AcessoLogin", "true")
                }),
                Expires = now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                NotBefore = now,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateTokenLogin(string usuarioId, string usuarioEmail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                // Comentário aqui só pra pegar o nameidentifier no commit
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioId),
                    new Claim(ClaimTypes.Email, usuarioEmail),
                    new Claim("AcessoLogin", "true")
                }),
                Expires = now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                NotBefore = now,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
