using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using backend.Domain.Auth.Extensions;
using backend.Domain.Auth.Interfaces;
using backend.Infra.Data;

namespace backend.Domain.Auth.Services
{
    /// <summary>
    /// Emite os tokens JWT da sessão (login, vínculo e admin simulando acesso) e controla o cookie que os carrega.
    /// Todo token leva o carimbo de segurança do usuário (claim "stamp"), conferido no banco a cada requisição
    /// pelo ValidateService: é o que permite derrubar sessões no logout, na troca de senha e na desativação.
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>Claims próprias da sessão, copiadas de um token para o seguinte (renovação e reemissão).</summary>
        private static readonly string[] ClaimsDaSessao =
        {
            ClaimTypes.NameIdentifier, "usuarioId", ClaimTypes.Name, "nomeClinica", "clinicaId",
            "isAdmin", "isAtivo", "acessoLogin", "Perfis", ClaimStamp,
        };

        public const string ClaimStamp = "stamp";
        public const string ClaimLoginEm = "loginEm";
        public const string ClaimRememberMe = "rememberMe";

        private static readonly TimeSpan DuracaoLembrarMe = TimeSpan.FromDays(7);
        private static readonly TimeSpan MargemRenovacaoLembrarMe = TimeSpan.FromDays(2);

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IDbContextFactory<AppDbContext> contextFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<TokenService> logger)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string CookieName => NomeDoCookie(_configuration);

        /// <summary>Nome do cookie da sessão (configuração Cookie:Name). Produção e homologação usam nomes diferentes.</summary>
        public static string NomeDoCookie(IConfiguration configuration)
        {
            var nome = configuration["Cookie:Name"];
            return string.IsNullOrWhiteSpace(nome) ? "auth_token" : nome;
        }

        public string GenerateTokenLogin(string usuarioId, bool? rememberMe = null)
        {
            using var _context = _contextFactory.CreateDbContext();

            if (!int.TryParse(usuarioId, out int idUsuario))
                throw new ArgumentException("Id do usuário deve ser um número inteiro válido.");

            var usuario = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == idUsuario)
                ?? throw new InvalidOperationException("Usuário não encontrado.");

            if (!usuario.IsAtivo)
                throw new UnauthorizedAccessException("Usuário inativo.");

            if (!usuario.IsAdmin && !_context.UsuariosPerfisClinicas.AsNoTracking().Any(upc => upc.UsuarioId == idUsuario))
                throw new InvalidOperationException("Usuário sem vínculo com clínica.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId),
                new Claim("usuarioId", usuarioId),
                new Claim(ClaimTypes.Name, usuario.Nome ?? string.Empty),
                new Claim("acessoLogin", "true"),
                new Claim("isAdmin", usuario.IsAdmin ? "true" : "false"),
                new Claim("isAtivo", "true"),
                new Claim(ClaimStamp, usuario.SecurityStamp ?? string.Empty),
            };

            // rememberMe informado = login novo (a sessão começa agora); nulo = troca de vínculo, herda a sessão atual
            var token = rememberMe.HasValue
                ? EscreverToken(claims, rememberMe.Value, DateTimeOffset.UtcNow)
                : EscreverToken(claims, RememberMeAtual(), LoginEmAtual());

            _logger.LogInformation("Token de login gerado para o usuário {UsuarioId}.", usuarioId);
            return token;
        }

        public string GenerateTokenVinculo(string usuarioPerfilClinicaId)
        {
            using var _context = _contextFactory.CreateDbContext();

            if (!int.TryParse(usuarioPerfilClinicaId, out int idVinculo))
                throw new ArgumentException("ID do vínculo deve ser um número inteiro válido.");

            var upc = _context.UsuariosPerfisClinicas
                .AsNoTracking()
                .Include(upc => upc.Usuario)
                .Include(upc => upc.Clinica)
                .FirstOrDefault(upc => upc.Id == idVinculo);

            if (upc?.Usuario == null || upc.Clinica == null)
                throw new InvalidOperationException("Vínculo de usuário não encontrado.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioPerfilClinicaId), // id do VÍNCULO
                new Claim("usuarioId", upc.UsuarioId.ToString()), // id do USUÁRIO (quem marcou consulta, preencheu avaliação...)
                new Claim(ClaimTypes.Name, upc.Usuario.Nome ?? string.Empty),
                new Claim("nomeClinica", upc.Clinica.NomeClinica ?? string.Empty),
                new Claim("clinicaId", upc.ClinicaId.ToString()),
                new Claim("isAdmin", "false"),
                new Claim("isAtivo", "true"),
                new Claim("Perfis", JsonSerializer.Serialize(upc.Perfis)),
                new Claim(ClaimStamp, upc.Usuario.SecurityStamp ?? string.Empty),
            };

            var token = EscreverToken(claims, RememberMeAtual(), LoginEmAtual());

            _logger.LogInformation("Token de vínculo gerado para o vínculo {VinculoId}, clínica {ClinicaId}.", usuarioPerfilClinicaId, upc.ClinicaId);
            return token;
        }

        public string GenerateTokenVinculoAdmin(int clinicaId, List<string> perfisAtivos)
        {
            using var _context = _contextFactory.CreateDbContext();

            var usuarioAtual = _httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

            var usuarioId = usuarioAtual.GetUsuarioId();
            if (!usuarioAtual.GetClaimValue<bool>("isAdmin") || usuarioId == null)
                throw new UnauthorizedAccessException("Usuário não possui permissão de acesso.");

            var admin = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == usuarioId && u.IsAdmin && u.IsAtivo)
                ?? throw new UnauthorizedAccessException("Usuário não possui permissão de acesso.");

            var clinica = _context.Clinicas.AsNoTracking().FirstOrDefault(c => c.Id == clinicaId)
                ?? throw new InvalidOperationException("Clínica não encontrada.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.Value.ToString()),
                new Claim("usuarioId", usuarioId.Value.ToString()), // no admin, o NameIdentifier já é o id do usuário
                new Claim(ClaimTypes.Name, admin.Nome ?? "Administrador"),
                new Claim("nomeClinica", clinica.NomeClinica ?? string.Empty),
                new Claim("clinicaId", clinicaId.ToString()),
                new Claim("isAdmin", "true"),
                new Claim("isAtivo", "true"),
                new Claim("Perfis", JsonSerializer.Serialize(perfisAtivos)),
                new Claim(ClaimStamp, admin.SecurityStamp ?? string.Empty),
            };

            var token = EscreverToken(claims, RememberMeAtual(), LoginEmAtual());

            _logger.LogInformation("Token de simulação gerado para o administrador {UsuarioId} na clínica {ClinicaId}.", usuarioId, clinicaId);
            return token;
        }

        public void SetCookieToken(HttpContext context, string token, bool rememberMe = false)
        {
            var opcoes = OpcoesDoCookie();

            // Sem "lembrar-me" o cookie é de sessão: some ao fechar o navegador
            if (rememberMe) opcoes.Expires = DateTimeOffset.UtcNow.Add(DuracaoLembrarMe);

            context.Response.Cookies.Append(CookieName, token, opcoes);
        }

        public void ClearCookieToken(HttpContext context)
        {
            context.Response.Cookies.Delete(CookieName, OpcoesDoCookie());
        }

        public Task RenovaToken(TokenValidatedContext context)
        {
            var principal = context.Principal;
            if (principal == null || context.SecurityToken == null) return Task.CompletedTask;

            var rememberMe = LerRememberMe(principal);
            var margem = rememberMe
                ? MargemRenovacaoLembrarMe
                : TimeSpan.FromMinutes(double.Parse(_configuration["Jwt:RenewInMinutesLeft"] ?? "5"));

            if (context.SecurityToken.ValidTo - DateTime.UtcNow >= margem) return Task.CompletedTask;

            try
            {
                var loginEm = LerLoginEm(principal);

                // Teto absoluto da sessão: passou dele, não renova mais e o usuário faz login de novo
                if (loginEm + DuracaoMaximaDaSessao() <= DateTimeOffset.UtcNow) return Task.CompletedTask;

                var novoToken = EscreverToken(CopiarClaimsDaSessao(principal), rememberMe, loginEm);
                SetCookieToken(context.HttpContext, novoToken, rememberMe);
            }
            catch (Exception ex)
            {
                // Não invalida o token atual, que ainda tem alguns minutos
                _logger.LogWarning(ex, "Falha ao renovar o token da sessão.");
            }

            return Task.CompletedTask;
        }

        public async Task ReemitirSessaoAtualAsync(HttpContext context)
        {
            var principal = context.User;
            var usuarioId = principal.GetUsuarioId();
            if (usuarioId == null) return;

            await using var db = await _contextFactory.CreateDbContextAsync();
            var stamp = await db.Usuarios.AsNoTracking()
                .Where(u => u.Id == usuarioId)
                .Select(u => u.SecurityStamp)
                .FirstOrDefaultAsync();

            var claims = CopiarClaimsDaSessao(principal).Where(c => c.Type != ClaimStamp).ToList();
            claims.Add(new Claim(ClaimStamp, stamp ?? string.Empty));

            var rememberMe = LerRememberMe(principal);
            SetCookieToken(context, EscreverToken(claims, rememberMe, LerLoginEm(principal)), rememberMe);
        }

        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// Monta e assina o JWT. A validade é a do tipo de sessão (padrão ou "lembrar-me"),
        /// limitada pelo teto absoluto contado a partir do login (Jwt:MaxSessionDays).
        /// </summary>
        private string EscreverToken(IEnumerable<Claim> claims, bool rememberMe, DateTimeOffset loginEm)
        {
            var secret = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("A chave secreta do JWT não foi configurada.");

            var agora = DateTimeOffset.UtcNow;
            var duracao = rememberMe
                ? DuracaoLembrarMe
                : TimeSpan.FromMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]));

            var expira = agora + duracao;
            var teto = loginEm + DuracaoMaximaDaSessao();
            if (expira > teto) expira = teto;
            if (expira <= agora) throw new SecurityTokenExpiredException("A sessão atingiu o tempo máximo. Faça login novamente.");

            var todas = claims.ToList();
            todas.Add(new Claim(ClaimRememberMe, rememberMe.ToString()));
            todas.Add(new Claim(ClaimLoginEm, loginEm.ToUnixTimeSeconds().ToString()));

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(todas),
                Expires = expira.UtcDateTime,
                NotBefore = agora.UtcDateTime,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private CookieOptions OpcoesDoCookie()
        {
            var path = _configuration["Cookie:Path"];
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = _configuration.GetValue<bool>("CookieSecure"),
                SameSite = SameSiteMode.Strict,
                Path = string.IsNullOrWhiteSpace(path) ? "/" : path,
            };
        }

        private TimeSpan DuracaoMaximaDaSessao() =>
            TimeSpan.FromDays(_configuration.GetValue<double?>("Jwt:MaxSessionDays") ?? 30);

        private static List<Claim> CopiarClaimsDaSessao(ClaimsPrincipal principal) =>
            principal.Claims
                .Where(c => ClaimsDaSessao.Contains(c.Type))
                .Select(c => new Claim(c.Type, c.Value))
                .ToList();

        private bool RememberMeAtual()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return principal != null && LerRememberMe(principal);
        }

        private DateTimeOffset LoginEmAtual()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return principal == null ? DateTimeOffset.UtcNow : LerLoginEm(principal);
        }

        private static bool LerRememberMe(ClaimsPrincipal principal) =>
            bool.TryParse(principal.FindFirst(ClaimRememberMe)?.Value, out var lembrar) && lembrar;

        /// <summary>Instante do login da sessão. Token sem a claim é tratado como sessão já vencida.</summary>
        private static DateTimeOffset LerLoginEm(ClaimsPrincipal principal) =>
            long.TryParse(principal.FindFirst(ClaimLoginEm)?.Value, out var segundos)
                ? DateTimeOffset.FromUnixTimeSeconds(segundos)
                : DateTimeOffset.UnixEpoch;
    }
}
