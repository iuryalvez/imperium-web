using backend.Data;
using backend.Dtos.UsuarioPerfilClinica;
using backend.Dtos.Auth;
using backend.Interfaces;
using backend.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using backend.Services;

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
        private readonly ICookieService _cookieService;

        public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, 
                              ILogger<AuthController> logger, IEmailService emailService, ITokenService tokenService, 
                              ICookieService cookieService, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
            _tokenService = tokenService;
            _cookieService = cookieService;
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
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == model.Email);
                
                // Obtém informações da requisição
                var usuarioIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var usuarioAgent = Request.Headers["User-Agent"].ToString();

                var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), usuarioIp, usuarioAgent);
                
                _cookieService.SetCookieToken(HttpContext, tokenLogin);

                return Ok(new { tokenLogin });
            }

            _logger.LogWarning("Tentativa de login inválida.");
            return Unauthorized(new { Message = "Login ou senha inválidos" });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
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
            if (usuarioId == 0) {
                return BadRequest("Claim 'usuarioId' não encontrada ou inválida.");
            }

            var perfis = await _context.UsuariosPerfisClinicas
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Perfil)
                .Include(p => p.Clinica)
                .ToListAsync();

            if (!perfis.Any())
            {
                _logger.LogWarning("Nenhum perfil encontrado para o usuário com ID: {UsuarioId}", usuarioId);
                return NotFound("Nenhum perfil encontrado para este usuário.");
            }

            var perfisDTO = perfis.Select(p => new UsuarioPerfilClinicaDto
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId,
                PerfilId = p.PerfilId,
                NomePerfil = p.Perfil.Name,
                ClinicaId = p.ClinicaId,
                NomeClinica = p.Clinica.NomeFantasia
            }).ToList();

            _logger.LogInformation("Perfis retornados para o usuário com ID: {UsuarioId}", usuarioId);
            
            return Ok(perfisDTO);
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpPost("sistema/{upcId}")]
        public async Task<IActionResult> SelecionarVinculo(int upcId)
        {
            try
            {
                // Obtem o valor de NameIdentifier do token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Usuário não autenticado.");
                }

                // Verifica o tipo de token pela presença da claim "acessoLogin"
                var isAcessoLogin = User.HasClaim("AcessoLogin", "true");

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

                // Busca o vínculo selecionado pelo ID e verifica se pertence ao mesmo usuário
                var vinculo = await _context.UsuariosPerfisClinicas
                    .Include(v => v.Perfil)
                    .Include(v => v.Usuario)
                    .Include(v => v.Clinica)
                    .FirstOrDefaultAsync(v => v.Id == upcId && v.UsuarioId == usuarioId);

                // Verifica se o vínculo foi encontrado
                if (vinculo == null)
                {
                    return NotFound("Vínculo não encontrado ou não pertence ao mesmo usuário.");
                }

                // Informações para o retorno
                var role = vinculo.Perfil?.Name ?? "!!! Role não definida !!!";

                // Extrai informações necessárias para gerar o token
                var vinculoId = vinculo.Id.ToString();
                var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // Gera o token
                var token = _tokenService.GenerateTokenVinculo(vinculoId, userIp, userAgent);

                // Configura o cookie HTTP-Only com o novo token
                _cookieService.SetCookieToken(HttpContext, token);

                // Retorna a Role escolhida
                return Ok(new { Role = role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao selecionar vínculo: " + ex.Message);
            }
        }

        [Authorize(Policy = "VisualizaTudo")]
        [HttpPost("sistema/trocar-vinculo")]
        public async Task<IActionResult> GenerateTokenTrocarVinculo()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimUPCId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claimUPCId == null)
            {
                _logger.LogWarning("Claim 'NameIdentifier' não encontrada no token.");
                return Unauthorized("Token inválido!");
            }

            int usuarioPerfilClinicaId;
            if (!int.TryParse(claimUPCId.Value, out usuarioPerfilClinicaId))
            {
                _logger.LogWarning("Falha ao converter o valor da claim 'NameIdentifier' para int. Valor: {Value}", claimUPCId.Value);
                return Unauthorized("Token inválido!");
            }

            var usuarioId = await _context.UsuariosPerfisClinicas
                .Where(upc => upc.Id == usuarioPerfilClinicaId)
                .Select(upc => upc.UsuarioId)
                .FirstOrDefaultAsync();

            if (usuarioId == 0)
            {
                _logger.LogWarning("Nenhum usuário encontrado para o ID de vínculo {UsuarioPerfilClinicaId}.", usuarioPerfilClinicaId);
                return NotFound("Nenhum usuário encontrado para este vínculo.");
            }

            var usuarioIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var usuarioAgent = Request.Headers["User-Agent"].ToString();

            var token = _tokenService.GenerateTokenLogin(usuarioId.ToString(), usuarioIp, usuarioAgent);

            _logger.LogInformation("Token de login gerado com sucesso para o usuário com ID {UsuarioId}.", usuarioId);

            return Ok();
        }

        [HttpGet("check")]
        public IActionResult CheckStatus()
        {
            try
            {
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { status = "invalido" });
                }

                // Verificando se o token contém a claim "acessoLogin"
                var acessoLoginClaim = User.FindFirst("acessoLogin");
                if (acessoLoginClaim != null)
                {
                    return Ok(new { status = "logado" });
                }

                // Caso contrário, o token é considerado um token de vínculo
                return Ok(new
                {
                    status = "autenticado",
                    role = roleClaim
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
