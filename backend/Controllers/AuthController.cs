using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Auth;

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
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ILogger<AuthController> logger, IEmailSender emailSender, ITokenService tokenService, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _tokenService = tokenService;
            _context = context;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The sign-up data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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
        /// Logs in a user.
        /// </summary>
        /// <param name="model">The login data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Login.");
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Senha, model.RememberMe, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in successfully.");
                return Ok(new { Message = "Login realizado com sucesso!" });
            }

            _logger.LogWarning("Invalid login attempt.");
            return Unauthorized(new { Message = "Login ou senha inválidos" });
        }

        // [HttpPost("login-swagger")]
        // public async Task<IActionResult> LoginSwagger([FromForm] LoginDto model)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         _logger.LogWarning("Invalid model state for Login.");
        //         return BadRequest(ModelState);
        //     }

        //     var result = await _signInManager.PasswordSignInAsync(model.Email, model.Senha, model.RememberMe, false);
        //     if (result.Succeeded)
        //     {
        //         var user = await _userManager.FindByEmailAsync(model.Email);
        //         if (user == null)
        //         {
        //             _logger.LogWarning("User not found.");
        //             return Unauthorized(new { Message = "Usuário não encontrado" });
        //         }

        //         var roles = await _userManager.GetRolesAsync(user);
        //         if (!roles.Contains("ADMIN"))
        //         {
        //             _logger.LogWarning("User is not an Admin.");
        //             return Forbid(); // Retorna um resultado de Forbid sem mensagem
        //         }

        //         // Gera o token JWT para o usuário autenticado
        //         var usuarioId = user.Id.ToString();
        //         var token = _tokenService.GenerateTokenLogin(usuarioId, user.Email, "ADMIN");

        //         _logger.LogInformation("User logged in successfully.");
                
        //         return Ok(new { Token = token, Message = "Login realizado com sucesso!" });
        //     }

        //     _logger.LogWarning("Invalid login attempt.");
        //     return Unauthorized(new { Message = "Login ou senha inválidos" });
        // }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Não revelar se o usuário não existe ou não está confirmado
                return Ok(new { Message = "Se o e-mail estiver registrado, um link de redefinição de senha será enviado." });
            }

            // Gera o token para redefinição de senha
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Auth", new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);

            // Envia o e-mail com o link para redefinir a senha
            await _emailSender.SendEmailAsync(user.Email, "Redefinir Senha", 
                $"Por favor, redefina sua senha clicando <a href='{callbackUrl}'>aqui</a>. O link expira em 30 minutos.");

            return Ok(new { Message = "Se o e-mail estiver registrado, um link de redefinição de senha será enviado." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Não revelar se o usuário não existe
                return Ok(new { Message = "Senha redefinida com sucesso." });
            }

            // Redefinir a senha usando o token gerado
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Senha redefinida com sucesso." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email), "O e-mail do usuário não pode ser nulo ou vazio");
            }

            await _emailSender.SendEmailAsync(user.Email, "Assunto do E-mail", "Mensagem do E-mail");

            return Ok("E-mail enviado com sucesso");
        }

        [HttpPost("token/{usuarioId}")]
        public async Task<IActionResult> GenerateToken(int usuarioId)
        {
            // Busca as entidades UsuarioPerfilClinica pelo usuarioId
            var usuarioPerfilClinicas = await _context.UsuariosPerfisClinicas
                .Where(upc => upc.UsuarioId == usuarioId)
                .ToListAsync();

            if (usuarioPerfilClinicas == null || !usuarioPerfilClinicas.Any())
            {
                return NotFound("O usuário não possui nenhum vínculo com nenhuma clínica ou tipo definido.");
            }

            // Gera o token usando o serviço de token com as informações necessárias
            var usuarioPerfilClinica = usuarioPerfilClinicas.First(); // Supondo que você queira usar a primeira associação encontrada
            var token = _tokenService.GenerateToken(usuarioPerfilClinica.Id.ToString());

            // Retorna o token gerado
            return Ok(new { token });
        }
    }
}
