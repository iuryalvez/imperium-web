using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using backend.Services;
using backend.Models;
using backend.Data;
using System.IdentityModel.Tokens.Jwt;
using backend.Dtos.UsuarioPerfilClinica;
using backend.Dtos.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == model.Email);
                    
                var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), model.Email);
                return Ok(new { tokenLogin });
            }

            _logger.LogWarning("Invalid login attempt.");
            return Unauthorized(new { Message = "Login ou senha inválidos" });
        }

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

        [Authorize(Policy = "AcessoLoginPolicy")]
        [HttpGet("sistema")]
        public async Task<IActionResult> GetPerfis()
        {
            // // Verifica se a identidade do usuário foi definida
            // var claimsIdentity = User.Identity as ClaimsIdentity;
            // if (claimsIdentity == null)
            // {
            //     _logger.LogWarning("ClaimsIdentity é null. Usuário não autenticado.");
            //     return Unauthorized("Token inválido ou claims ausentes.");
            // }

            // // Log para inspecionar as claims atuais
            // _logger.LogInformation("Claims encontradas no token:");
            // foreach (var claim in claimsIdentity.Claims)
            // {
            //     _logger.LogInformation("Tipo de Claim: {Type}, Valor: {Value}", claim.Type, claim.Value);
            // }

            // // Verifica a claim específica para o ID do usuário
            // var claimUsuarioId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // if (claimUsuarioId == null)
            // {
            //     _logger.LogWarning("Claim 'NameId' não encontrada no token.");
            //     return Unauthorized("Token inválido!");
            // }

            // // Tenta parsear o ID do usuário
            // if (!int.TryParse(claimUsuarioId.Value, out var usuarioId))
            // {
            //     _logger.LogWarning("Falha ao converter o valor da claim 'NameId' para int. Valor: {Value}", claimUsuarioId.Value);
            //     return Unauthorized("Token inválido!");
            // }

            // _logger.LogInformation("Usuário autenticado com ID: {UsuarioId}", usuarioId);
            
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claimUsuarioId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var usuarioId = int.Parse(claimUsuarioId.Value);
            // Recupera perfis para o usuário
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

            // Mapeia os perfis para DTOs
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


        [Authorize(Policy = "AcessoLoginPolicy")]
        [HttpPost("sistema/{upcId}")]
        public async Task<IActionResult> GenerateToken(int upcId)
        {
            // Busca as entidades UsuarioPerfilClinica pelo usuarioId
            var usuarioPerfilClinica = await _context.UsuariosPerfisClinicas
                .FirstOrDefaultAsync(upc => upc.Id == upcId);

            if(usuarioPerfilClinica == null)
            {
                return NotFound("Esse perfil nessa clínica não existe!");
            }

            // Gera o token usando o serviço de token com as informações necessárias
            var token = _tokenService.GenerateToken(upcId.ToString());

            // Retorna o token gerado
            return Ok(new { token });
        }
    }
}
