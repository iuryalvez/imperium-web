using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace backend.Tests.Infra
{
    /// <summary>
    /// Gera tokens com as mesmas claims do TokenService real (login, vínculo da clínica e admin simulando acesso),
    /// assinados com a chave só de teste. A API lê o token do cookie "auth_token" e confere no banco, a cada
    /// requisição, o usuário ativo e o carimbo de segurança (claim "stamp"), por isso o token leva o SecurityStamp
    /// do usuário criado no cenário.
    /// </summary>
    public static class TokenDeTeste
    {
        /// <summary>Token de vínculo de um usuário comum (TokenService.GenerateTokenVinculo).</summary>
        public static string Vinculo(int vinculoId, int usuarioId, string nomeUsuario, int clinicaId, string nomeClinica, IEnumerable<string> perfis, string stamp)
        {
            return Gerar(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, vinculoId.ToString()),
                new Claim("usuarioId", usuarioId.ToString()),
                new Claim(ClaimTypes.Name, nomeUsuario),
                new Claim("nomeClinica", nomeClinica),
                new Claim("clinicaId", clinicaId.ToString()),
                new Claim("isAdmin", "false"),
                new Claim("isAtivo", "true"),
                new Claim("Perfis", JsonSerializer.Serialize(perfis.ToList())),
                new Claim("stamp", stamp),
            });
        }

        /// <summary>Token de login, antes de escolher o vínculo (TokenService.GenerateTokenLogin).</summary>
        public static string Login(int usuarioId, string nomeUsuario, bool isAdmin, string? stamp,
            bool rememberMe = false, DateTimeOffset? loginEm = null, TimeSpan? validade = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim("usuarioId", usuarioId.ToString()),
                new Claim(ClaimTypes.Name, nomeUsuario),
                new Claim("acessoLogin", "true"),
                new Claim("isAdmin", isAdmin ? "true" : "false"),
                new Claim("isAtivo", "true"),
            };
            if (stamp != null) claims.Add(new Claim("stamp", stamp)); // nulo = token antigo, sem carimbo

            return Gerar(claims, rememberMe, loginEm, validade);
        }

        /// <summary>Token de login do admin, usado nas rotas do Sistema.</summary>
        public static string AdminSistema(int usuarioId, string nomeUsuario, string stamp) =>
            Login(usuarioId, nomeUsuario, isAdmin: true, stamp);

        /// <summary>Token do admin simulando acesso a uma clínica (TokenService.GenerateTokenVinculoAdmin).</summary>
        public static string AdminSimulando(int usuarioId, string nomeUsuario, int clinicaId, string nomeClinica, IEnumerable<string> perfis, string stamp)
        {
            return Gerar(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim("usuarioId", usuarioId.ToString()),
                new Claim(ClaimTypes.Name, nomeUsuario),
                new Claim("nomeClinica", nomeClinica),
                new Claim("clinicaId", clinicaId.ToString()),
                new Claim("isAdmin", "true"),
                new Claim("isAtivo", "true"),
                new Claim("Perfis", JsonSerializer.Serialize(perfis.ToList())),
                new Claim("stamp", stamp),
            });
        }

        private static string Gerar(IEnumerable<Claim> claims, bool rememberMe = false, DateTimeOffset? loginEm = null, TimeSpan? validade = null)
        {
            var agora = DateTime.UtcNow;
            var todas = claims.ToList();
            todas.Add(new Claim("rememberMe", rememberMe.ToString()));
            todas.Add(new Claim("loginEm", (loginEm ?? DateTimeOffset.UtcNow).ToUnixTimeSeconds().ToString()));

            var descritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(todas),
                NotBefore = agora.AddMinutes(-1),
                Expires = agora.Add(validade ?? TimeSpan.FromMinutes(60)),
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
