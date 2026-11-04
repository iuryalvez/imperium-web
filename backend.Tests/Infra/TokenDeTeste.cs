using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace backend.Tests.Infra
{
    /// <summary>
    /// Gera tokens com as mesmas claims do TokenService real (vínculo da clínica e admin simulando acesso),
    /// assinados com a chave só de teste. A API lê o token do cookie "auth_token" e confere IP e User-Agent
    /// (ValidateService), por isso o cliente de teste envia os mesmos valores das claims.
    /// </summary>
    public static class TokenDeTeste
    {
        public const string Ip = "127.0.0.1";
        public const string UserAgent = "imperium-testes";

        /// <summary>Token de vínculo de um usuário comum (TokenService.GenerateTokenVinculo).</summary>
        public static string Vinculo(int vinculoId, int usuarioId, string nomeUsuario, int clinicaId, string nomeClinica, IEnumerable<string> perfis)
        {
            return Gerar(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, vinculoId.ToString()),
                new Claim("usuarioId", usuarioId.ToString()),
                new Claim(ClaimTypes.Name, nomeUsuario),
                new Claim("UsuarioIp", Ip),
                new Claim("UsuarioAgent", UserAgent),
                new Claim("nomeClinica", nomeClinica),
                new Claim("clinicaId", clinicaId.ToString()),
                new Claim("isAdmin", "false"),
                new Claim("isAtivo", "true"),
                new Claim("rememberMe", "False"),
                new Claim("Perfis", JsonSerializer.Serialize(perfis.ToList())),
            });
        }

        /// <summary>Token de login do admin, usado nas rotas do Sistema (TokenService.GenerateTokenLogin).</summary>
        public static string AdminSistema(int usuarioId, string nomeUsuario)
        {
            return Gerar(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Name, nomeUsuario),
                new Claim("UsuarioIp", Ip),
                new Claim("UsuarioAgent", UserAgent),
                new Claim("acessoLogin", "true"),
                new Claim("rememberMe", "False"),
                new Claim("isAdmin", "true"),
                new Claim("isAtivo", "true"),
            });
        }

        /// <summary>Token do admin simulando acesso a uma clínica (TokenService.GenerateTokenVinculoAdmin).</summary>
        public static string AdminSimulando(int usuarioId, string nomeUsuario, int clinicaId, string nomeClinica, IEnumerable<string> perfis)
        {
            return Gerar(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim("usuarioId", usuarioId.ToString()),
                new Claim(ClaimTypes.Name, nomeUsuario),
                new Claim("UsuarioIp", Ip),
                new Claim("UsuarioAgent", UserAgent),
                new Claim("nomeClinica", nomeClinica),
                new Claim("clinicaId", clinicaId.ToString()),
                new Claim("isAdmin", "true"),
                new Claim("isAtivo", "true"),
                new Claim("Perfis", JsonSerializer.Serialize(perfis.ToList())),
            });
        }

        private static string Gerar(IEnumerable<Claim> claims)
        {
            var agora = DateTime.UtcNow;
            var descritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = agora,
                Expires = agora.AddMinutes(60),
                Issuer = ImperiumApiFactory.JwtIssuer,
                Audience = ImperiumApiFactory.JwtAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ImperiumApiFactory.JwtKey)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(descritor));
        }
    }
}
