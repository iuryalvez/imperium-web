using backend.Data;
using backend.Dtos.Vinculo;
using backend.Dtos.Auth;
using backend.Interfaces;
using backend.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using backend.Services;
using System.Text.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, 
                              ILogger<AuthController> logger, IEmailService emailService, ITokenService tokenService, 
                              AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
            _tokenService = tokenService;
            _context = context;
        }

        /// <summary>
        /// Faz o registro de um novo usuário através do auto-cadastro do front-end.
        /// </summary>
        /// <param name="model">O sign-up data transfer object.</param>
        /// <returns>IActionResult indica o retorno da requisição.</returns>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for SignUp.");
                return BadRequest(ModelState);
            }

            var user = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nome = model.Nome,
                DataCad = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Senha);
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário registrado com sucesso.");
                return Ok(new { Message = "Usuário registrado com sucesso!" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("Erro ao registrar usuário: {Error}", error.Description);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Faz o login de um usuário.
        /// </summary>
        /// <param name="model">O login data transfer object.</param>
        /// <returns>IActionResult indicando o retorno da requisição.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login inválido.");
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Senha, model.RememberMe, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário logado com sucesso!.");

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (usuario == null)
                {
                    return Unauthorized(new { Message = "Usuário não encontrado." });
                }

                // atualiza a data do último acesso
                usuario.DataUltimoAcesso = DateTime.UtcNow;

                // obtém informações da requisição
                var usuarioIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var usuarioAgent = Request.Headers["User-Agent"].ToString();

                // gera o token de login
                var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), usuarioIp, usuarioAgent);

                // atualiza o banco de dados com a nova data de acesso
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                // define o token no cookie
                _tokenService.SetCookieToken(HttpContext, tokenLogin);

                return Ok(new { tokenLogin });
            }

            _logger.LogWarning("Tentativa de login inválida.");
            return Unauthorized(new { Message = "Login ou senha inválidos" });
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Response.Cookies.Delete("auth_token");
                return Ok(new { message = "Logout efetuado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao efetuar logout: " + ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for ForgotPassword.");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para o e-mail {Email}.", model.Email);
                // Não revelar se o usuário não existe ou não está confirmado
                return Ok(new { Message = "Se o e-mail estiver registrado, um link de redefinição de senha será enviado." });
            }

            // Gera o token para redefinição de senha
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Auth", new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);

            // Envia o e-mail com o link para redefinir a senha
            await _emailService.SendEmailAsync(user.Email, "Redefinir Senha", 
                $"Por favor, redefina sua senha clicando <a href='{callbackUrl}'>aqui</a>. O link expira em 30 minutos.");

            _logger.LogInformation("E-mail de redefinição de senha enviado para {Email}.", user.Email);

            return Ok(new { Message = "Se o e-mail estiver registrado, um link de redefinição de senha será enviado." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for ResetPassword.");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para o e-mail {Email}.", model.Email);
                // Não revelar se o usuário não existe
                return Ok(new { Message = "Senha redefinida com sucesso." });
            }

            // Redefinir a senha usando o token gerado
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Senha redefinida com sucesso para o usuário com e-mail {Email}.", user.Email);
                return Ok(new { Message = "Senha redefinida com sucesso." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("Erro ao redefinir senha para o usuário com e-mail {Email}: {Error}", user.Email, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para o ID {UserId}.", userId);
                return NotFound("Usuário não encontrado");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning("O e-mail do usuário com ID {UserId} é nulo ou vazio.", userId);
                throw new ArgumentNullException(nameof(user.Email), "O e-mail do usuário não pode ser nulo ou vazio");
            }

            await _emailService.SendEmailAsync(user.Email, "Assunto do E-mail", "Mensagem do E-mail");

            _logger.LogInformation("E-mail enviado com sucesso para o usuário com ID {UserId}.", userId);

            return Ok("E-mail enviado com sucesso");
        }

        [HttpGet("sistema")]
        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        public async Task<IActionResult> GetVinculos()
        {
            var usuarioId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);
            if (usuarioId == 0)
            {
                return BadRequest("Erro ao identificar usuário.");
            }

            var vinculos = await _context.UsuariosPerfisClinicas
                .Where(p => p.UsuarioId == usuarioId && p.Perfis.Any(perfil => perfil.Key != "Inativo" && perfil.Value)) // se algo der errado na busca de vínculos do usuário comum é aqui
                .Select(p => new VinculoDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    ClinicaId = p.ClinicaId,
                    NomeClinica = p.Clinica.NomeFantasia,
                    Perfis = p.Perfis
                        .Where(perfil => perfil.Key != "Inativo" && perfil.Value) // Apenas perfis ativos
                        .ToDictionary(perfil => perfil.Key, perfil => perfil.Value) // Retorna dicionário filtrado
                })
                .ToListAsync();

            if (!vinculos.Any())
            {
                _logger.LogWarning("Nenhum vínculo encontrado para o usuário {UsuarioId}", usuarioId);
                return NotFound("Nenhum vínculo disponível.");
            }

            _logger.LogInformation("Retornando {Count} vínculos para usuário {UsuarioId}", vinculos.Count, usuarioId);
            
            return Ok(vinculos);
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpPost("sistema/{upcId}")]
        public async Task<IActionResult> SelecionarVinculo(int upcId)
        {
            try
            {
                // Obtém o valor de NameIdentifier do token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Usuário não autenticado.");
                }

                // Verifica o tipo de token pela presença da claim "acessoLogin"
                var isAcessoLogin = User.HasClaim("acessoLogin", "true");

                int usuarioId;

                if (isAcessoLogin)
                {
                    // Token de acesso: o NameIdentifier é o ID do usuário
                    Console.WriteLine("Token de acesso detectado. NameIdentifier é o ID do usuário.");
                    usuarioId = int.Parse(userIdClaim);
                }
                else
                {
                    // Token de vínculo: o NameIdentifier é o ID do vínculo
                    Console.WriteLine("Token de vínculo detectado. NameIdentifier é o ID do vínculo.");
                    var vinculoIdClaim = int.Parse(userIdClaim);

                    // Busca o vínculo atual para obter o ID do usuário associado
                    var vinculoAtual = await _context.UsuariosPerfisClinicas
                        .FirstOrDefaultAsync(v => v.Id == vinculoIdClaim);

                    if (vinculoAtual == null)
                    {
                        return Unauthorized("Vínculo atual não encontrado.");
                    }

                    usuarioId = vinculoAtual.UsuarioId;
                }

                // Busca o vínculo selecionado pelo ID, garantindo que não esteja inativo
                var vinculo = await _context.UsuariosPerfisClinicas
                    .Include(v => v.Usuario)
                    .Include(v => v.Clinica)
                    .FirstOrDefaultAsync(v => v.Id == upcId && v.UsuarioId == usuarioId && 
                                            (!v.Perfis.ContainsKey("Inativo") || v.Perfis["Inativo"] == false));

                // Verifica se o vínculo foi encontrado
                if (vinculo == null)
                {
                    return NotFound("Vínculo não encontrado ou não pertence ao mesmo usuário.");
                }

                // Extrai informações necessárias para gerar o token
                var vinculoId = vinculo.Id.ToString();
                var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // Gera o token com os perfis ativos
                var token = _tokenService.GenerateTokenVinculo(vinculoId, userIp, userAgent);

                // Configura o cookie HTTP-Only com o novo token
                _tokenService.SetCookieToken(HttpContext, token);

                // Retorna os perfis ativos
                return Ok(new { vinculo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao selecionar vínculo: " + ex.Message);
            }
        }

        [Authorize(Policy = "GerenciaSistema")]
        [HttpPost("sistema/admin")] // Nova rota específica para admins
        public async Task<IActionResult> SelecionarVinculoAdmin([FromBody] VinculoAdminDto dto)
        {
            try
            {
                // Obtém o valor de NameIdentifier do token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Usuário não autenticado.");
                }

                // Verifica se o usuário tem permissão de administrador
                var isAdmin = User.Claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
                if (!isAdmin)
                {
                    return Forbid("Apenas administradores podem selecionar vínculo administrativo.");
                }

                // Obtém os dados da clínica
                var clinica = await _context.Clinicas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == dto.ClinicaId);
                if (clinica == null)
                {
                    return NotFound("Clínica não encontrada.");
                }

                // Extrai informações necessárias para gerar o token
                var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();
                var usuarioNome = User.FindFirst(ClaimTypes.Name)?.Value ?? "Administrador";

                // Gera o token como se fosse um vínculo
                var token = _tokenService.GenerateTokenVinculoAdmin(dto.ClinicaId, dto.Perfis, userIp, userAgent);

                // Configura o cookie HTTP-Only com o novo token
                _tokenService.SetCookieToken(HttpContext, token);

                // Retorna os dados simulados do vínculo
                return Ok(new { clinicaId = dto.ClinicaId, perfisAtivos = dto.Perfis });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao selecionar vínculo administrativo: " + ex.Message);
            }
        }

        [HttpGet("check")]
        public IActionResult CheckStatus()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                var clinicaNomeClaim = User.FindFirst("clinicaNome")?.Value;
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var perfisClaim = User.FindFirst("Perfis")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { status = "invalido" });
                }

                bool IsAdmin = !string.IsNullOrEmpty(isAdminClaim) && bool.TryParse(isAdminClaim, out var parsedValue) && parsedValue;

                // Deserializa a claim "Perfis" para um dicionário de perfis ativos
                Dictionary<string, bool> perfisAtivos = new();
                if (!string.IsNullOrEmpty(perfisClaim))
                {
                    perfisAtivos = JsonSerializer.Deserialize<Dictionary<string, bool>>(perfisClaim) ?? new();
                }

                // Filtra os perfis ativos (onde o valor é true)
                var perfisFiltrados = perfisAtivos.Where(p => p.Value).Select(p => p.Key).ToList();

                var acessoLoginClaim = User.FindFirst("acessoLogin");
                if (acessoLoginClaim != null)
                {
                    return Ok(new
                    {
                        status = "logado",
                        isAdmin = IsAdmin,
                        name = userNameClaim
                    });
                }

                return Ok(new
                {
                    status = "autenticado",
                    isAdmin = IsAdmin,
                    name = userNameClaim,
                    clinicaNome = clinicaNomeClaim,
                    perfis = perfisFiltrados // Lista apenas os perfis ativos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "erro",
                    message = "Erro ao verificar status de autenticação.",
                    details = ex.Message
                });
            }
        }
    }
}
