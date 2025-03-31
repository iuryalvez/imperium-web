using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IConfiguration _configuration;
        private readonly ICookieService _cookie;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IDbContextFactory<AppDbContext> contextFactory, IConfiguration configuration, ICookieService cookie, IHttpContextAccessor httpContextAccessor, ILogger<TokenService> logger)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
            _cookie = cookie;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string GenerateTokenLogin(string usuarioId, string usuarioIp, string usuarioAgent)
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();
                
                if (!int.TryParse(usuarioId, out int idUsuario)) {
                    _logger.LogError("Id do usuário inválido: {usuarioId}", usuarioId);
                    throw new ArgumentException("Id do usuário deve ser um número inteiro válido.");
                }

                var usuario = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == idUsuario);
                if (usuario == null) {
                    _logger.LogError("Usuário não encontrado para Id: {usuarioId}.", usuarioId);
                    throw new Exception("Usuário não encontrado.");
                }

                if (!usuario.IsAdmin) {
                    var upc = _context.UsuariosPerfisClinicas
                        .AsNoTracking()
                        .Include(upc => upc.Usuario)
                        .FirstOrDefault(upc => upc.UsuarioId == idUsuario);

                    if (upc == null || upc.Usuario == null)
                    {
                        _logger.LogError("Usuário ou vínculo com clínica não encontrado para ID {UsuarioId}.", usuarioId);
                        throw new Exception("Usuário ou vínculo com clínica não encontrado.");
                    }
                }

                var usuarioNome = usuario.Nome ?? throw new Exception("Nome não encontrado para o usuário");
                
                var tokenHandler = new JwtSecurityTokenHandler();
                
                var secret = _configuration["JWT:Secret"];
                if (string.IsNullOrWhiteSpace(secret))
                {
                    throw new InvalidOperationException("A chave secreta do JWT não foi configurada.");
                }
                var key = Encoding.UTF8.GetBytes(secret);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioId),
                    new Claim(ClaimTypes.Name, usuarioNome),
                    new Claim("UsuarioIP", usuarioIp),
                    new Claim("UsuarioAgent", usuarioAgent),
                    new Claim("acessoLogin", "true"),
                };

                if (usuario.IsAdmin) claims.Add(new Claim("isAdmin", "true"));
                else {
                    var upc = _context.UsuariosPerfisClinicas
                        .AsNoTracking()
                        .Include(upc => upc.Usuario)
                        .FirstOrDefault(upc => upc.UsuarioId == idUsuario);
                    var perfisAtivos = upc.Perfis.Where(p => p.Value).Select(p => p.Key).ToList();
                    claims.Add(new Claim("Perfis", JsonSerializer.Serialize(perfisAtivos))); // Lista de perfis ativos
                    claims.Add(new Claim("isAdmin", "false"));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiresInMinutes"])),
                    NotBefore = DateTime.UtcNow,
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

        public string GenerateTokenVinculo(string usuarioPerfilClinicaId, string usuarioIp, string usuarioAgent)
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();
                
                var userPrincipal = _httpContextAccessor.HttpContext?.User;

                var isAdmin = userPrincipal.Claims.Any(c => c.Type == "isAdmin" && c.Value == "true");

                if (!int.TryParse(usuarioPerfilClinicaId, out int idVinculo))
                {
                    _logger.LogError("ID do vínculo inválido: {UsuarioPerfilClinicaId}", usuarioPerfilClinicaId);
                    throw new ArgumentException("ID do vínculo deve ser um número inteiro válido.");
                }

                var upc = _context.UsuariosPerfisClinicas
                    .AsNoTracking()
                    .Include(upc => upc.Usuario)
                    .Include(upc => upc.Clinica)
                    .FirstOrDefault(upc => upc.Id == idVinculo);

                if (upc == null || upc.Usuario == null)
                {
                    _logger.LogError("Vínculo de usuário não encontrado para ID {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                    throw new Exception("Vínculo de usuário não encontrado.");
                }

                var clinicaId = upc.ClinicaId;

                var clinicaNome = upc.Clinica?.NomeClinica ?? throw new Exception("Clínica não encontrada para o usuário.");

                var usuarioNome = upc.Usuario?.Nome ?? throw new Exception("Nome não encontrado para o usuário.");
                
                // validação da chave JWT
                var secret = _configuration["JWT:Secret"];
                if (string.IsNullOrWhiteSpace(secret))
                {
                    throw new InvalidOperationException("A chave secreta do JWT não foi configurada.");
                }
                var key = Encoding.UTF8.GetBytes(secret);
                
                var now = DateTime.UtcNow;

                var perfisAtivos = upc.Perfis.Where(p => p.Value).Select(p => p.Key).ToList();

                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuarioPerfilClinicaId),
                        new Claim(ClaimTypes.Name, usuarioNome),
                        new Claim("UsuarioIp", usuarioIp),
                        new Claim("UsuarioAgent", usuarioAgent),
                        new Claim("clinicaNome", clinicaNome),
                        new Claim("clinicaId", clinicaId.ToString()),
                        new Claim("isAdmin", "false"),
                        new Claim("Perfis", JsonSerializer.Serialize(perfisAtivos)) // Lista de perfis ativos
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
        public string GenerateTokenVinculoAdmin(int clinicaId, Dictionary<string, bool> perfisAtivos, string usuarioIp, string usuarioAgent)
        {
            try
            {
                using var _context = _contextFactory.CreateDbContext();
                
                var userPrincipal = _httpContextAccessor.HttpContext?.User;
                
                if (userPrincipal == null)
                {
                    throw new UnauthorizedAccessException("Usuário não autenticado.");
                }

                var isAdmin = userPrincipal.Claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
                var usuarioId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!isAdmin || string.IsNullOrEmpty(usuarioId))
                {
                    throw new UnauthorizedAccessException("Usuário não possui permissão de administrador.");
                }

                var clinica = _context.Clinicas.AsNoTracking().FirstOrDefault(c => c.Id == clinicaId);
                if (clinica == null)
                {
                    throw new Exception("Clínica não encontrada.");
                }

                var clinicaNome = clinica.NomeClinica;
                var usuarioNome = userPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? "Administrador";
                
                // Validação da chave JWT
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
                    new Claim(ClaimTypes.Name, usuarioNome),
                    new Claim("UsuarioIp", usuarioIp),
                    new Claim("UsuarioAgent", usuarioAgent),
                    new Claim("clinicaNome", clinicaNome),
                    new Claim("clinicaId", clinicaId.ToString()),
                    new Claim("isAdmin", "true"),
                    new Claim("Perfis", JsonSerializer.Serialize(perfisAtivos)) // Dicionário de perfis ativos
                };
                
                var tokenHandler = new JwtSecurityTokenHandler();
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

                _logger.LogInformation("Token de vínculo gerado com sucesso para o administrador {UsuarioId}, clínica {ClinicaId}.", usuarioId, clinicaId);
                return tokenHandler.WriteToken(token);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de entrada ao gerar token de vínculo do administrador: {Message}", ex.Message);
                throw new BadHttpRequestException("Requisição inválida: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token de vínculo para o administrador.");
                throw new Exception("Erro interno ao gerar token. Tente novamente mais tarde.");
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
