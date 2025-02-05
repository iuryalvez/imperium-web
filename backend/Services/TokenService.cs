using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookie;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IDbContextFactory<AppDbContext> contextFactory, IConfiguration configuration, ICookieService cookie, ILogger<TokenService> logger)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
            _cookie = cookie;
            _logger = logger;
        }

        public string GenerateTokenVinculo(string usuarioPerfilClinicaId, string usuarioIp, string usuarioAgent)
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();
                
                if (!int.TryParse(usuarioPerfilClinicaId, out int idVinculo))
                {
                    _logger.LogError("ID do vínculo inválido: {UsuarioPerfilClinicaId}", usuarioPerfilClinicaId);
                    throw new ArgumentException("ID do vínculo deve ser um número inteiro válido.");
                }

                var upc = _context.UsuariosPerfisClinicas
                    .AsNoTracking()
                    .Include(upc => upc.Perfil)
                    .Include(upc => upc.Usuario)
                    .Include(upc => upc.Clinica)
                    .FirstOrDefault(upc => upc.Id == idVinculo);

                if (upc == null)
                {
                    _logger.LogError("Vínculo de usuário não encontrado para ID {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                    throw new Exception("Vínculo de usuário não encontrado.");
                }

                var clinicaId = upc.ClinicaId;

                var clinicaNome = upc.Clinica?.NomeClinica ?? throw new Exception("Clínica não encontrada para o usuário.");

                var roleName = upc.Perfil?.Name ?? throw new Exception("Perfil não encontrado para o usuário.");

                var userName = upc.Usuario?.Nome ?? throw new Exception("Nome não encontrado para o usuário.");

                var tokenHandler = new JwtSecurityTokenHandler();
                
                // validação da chave JWT
                var secret = _configuration["JWT:Secret"];
                if (string.IsNullOrWhiteSpace(secret))
                {
                    throw new InvalidOperationException("A chave secreta do JWT não foi configurada.");
                }
                var key = Encoding.UTF8.GetBytes(secret);
                
                var now = DateTime.UtcNow;

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuarioPerfilClinicaId),
                        new Claim(ClaimTypes.Role, roleName), // adiciona a claim de Role
                        new Claim(ClaimTypes.Name, userName), // adiciona a claim de clinicaId
                        new Claim("UsuarioIP", usuarioIp),
                        new Claim("UsuarioAgent", usuarioAgent),
                        new Claim("clinicaNome", clinicaNome), // adiciona a claim de clinicaId
                        new Claim("clinicaId", clinicaId.ToString()) // adiciona a claim de clinicaId
                    }),
                    Expires = now.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiresInMinutes"])),
                    NotBefore = now,
                    Issuer = _configuration["JWT:Issuer"],
                    Audience = _configuration["JWT:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                _logger.LogInformation("Token de vínculo gerado com sucesso para o vínculo {UsuarioPerfilClinicaId}, associado à clínica {ClinicaId}.", usuarioPerfilClinicaId, clinicaId);
                return tokenHandler.WriteToken(token);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de entrada ao gerar token de vínculo: {Message}", ex.Message);
                throw new BadHttpRequestException("Requisição inválida: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token de vínculo para o ID {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                throw new Exception("Erro interno ao gerar token. Tente novamente mais tarde.");
            }
        }


        public string GenerateTokenLogin(string usuarioId, string usuarioIp, string usuarioAgent)
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();
                
                if (!int.TryParse(usuarioId, out int idUsuario))
                {
                    _logger.LogError("ID do usuário inválido: {usuarioId}", usuarioId);
                    throw new ArgumentException("ID do usuário deve ser um número inteiro válido.");
                }

                var upc = _context.UsuariosPerfisClinicas
                    .AsNoTracking()
                    .Include(upc => upc.Perfil)
                    .Include(upc => upc.Usuario)
                    .FirstOrDefault(upc => upc.UsuarioId == idUsuario);

                if (upc == null)
                {
                    _logger.LogError("Perfil de usuário não encontrado para ID {upc}.", upc);
                    throw new Exception("Perfil de usuário não encontrado.");
                }

                var roleName = upc.Perfil?.Name ?? throw new Exception("Perfil não encontrado para o usuário.");

                var userName = upc.Usuario?.Nome ?? throw new Exception("Nome não encontrado para o usuário.");

                var tokenHandler = new JwtSecurityTokenHandler();
                
                // validação da chave JWT
                var secret = _configuration["JWT:Secret"];
                if (string.IsNullOrWhiteSpace(secret))
                {
                    throw new InvalidOperationException("A chave secreta do JWT não foi configurada.");
                }
                var key = Encoding.UTF8.GetBytes(secret);

                var now = DateTime.UtcNow;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim("UsuarioIP", usuarioIp),
                    new Claim("UsuarioAgent", usuarioAgent),
                    new Claim("acessoLogin", "true")
                };

                // Se o usuário for ADMIN, adiciona a claim extra
                if (roleName == "ADMIN")
                {
                    claims.Add(new Claim("isAdmin", "true"));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = now.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiresInMinutes"])),
                    NotBefore = now,
                    Issuer = _configuration["JWT:Issuer"],
                    Audience = _configuration["JWT:Audience"],
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

        public Task RenovaToken(TokenValidatedContext context) 
        {
            var token = context.SecurityToken;
            var timeToExpire = token.ValidTo - DateTime.UtcNow;

            // Só roda se o token expirar em menos que o prazo configurado
            if (timeToExpire.TotalMinutes < double.Parse(_configuration["JWT:RenewInMinutesLeft"]))
            {
                var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userIp = context.Principal.FindFirst("UserIP").Value;
                var userAgent = context.Principal.FindFirst("UserAgent").Value;
                var acessoLogin = context.Principal.FindFirst("acessoLogin")?.Value;

                // gera o token correspondente ao que existia anteriormente
                string newToken = string.IsNullOrEmpty(acessoLogin)
                    ? GenerateTokenVinculo(userId, userIp, userAgent)
                    : GenerateTokenLogin(userId, userIp, userAgent);

                _cookie.SetCookieToken(context.HttpContext, newToken);
                Console.WriteLine($"Token renovado com sucesso para o usuário: {userId}");
            }

            return Task.CompletedTask;
        }
    }
}
