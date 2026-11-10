using backend.Domain.Auth.DTOs;
using backend.Domain.Usuarios.DTOs.Vinculo;
using backend.Domain.Auth.Interfaces;
using backend.Domain.Auth.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

using System.Security.Claims;
using System.Text.Json;

namespace backend.Domain.Auth.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>Nome da policy de rate limit das rotas públicas de autenticação (registrada no Program.cs).</summary>
        public const string RateLimitAuth = "auth";

        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration configuration,
            IAuthService authService,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("check")]
        [Authorize]
        public IActionResult CheckStatus()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized(new { status = "invalido" });

            bool isAdmin = User.GetClaimValue<bool>("isAdmin");
            var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            if (User.FindFirst("acessoLogin") != null)
            {
                return Ok(new { status = "logado", isAdmin, name = userNameClaim });
            }

            var perfisClaim = User.FindFirst("Perfis")?.Value;

            return Ok(new
            {
                status = "autenticado",
                isAdmin,
                name = userNameClaim,
                nomeClinica = User.FindFirst("nomeClinica")?.Value,
                perfis = string.IsNullOrEmpty(perfisClaim)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(perfisClaim)
            });
        }

        [HttpPost("login")]
        [EnableRateLimiting(RateLimitAuth)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model, HttpContext.GetClienteIp());

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = result.Message });
            }

            // O token vai só no cookie HttpOnly: nunca no corpo, onde um script da página conseguiria ler
            _tokenService.SetCookieToken(HttpContext, result.Data.ToString(), model.RememberMe);

            return Ok(new { message = "Login efetuado com sucesso." });
        }

        /// <summary>
        /// Encerra a sessão: troca o carimbo de segurança do usuário (o token deixa de valer em todos os dispositivos)
        /// e apaga o cookie. É POST para um site externo não conseguir deslogar o usuário com um simples link.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var usuarioId = User.Identity?.IsAuthenticated == true ? User.GetUsuarioId() : null;
                if (usuarioId != null) await _authService.LogoutAsync(usuarioId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao encerrar as sessões no logout.");
            }

            // O cookie sai mesmo se a sessão já estava inválida
            _tokenService.ClearCookieToken(HttpContext);
            return Ok(new { message = "Logout efetuado com sucesso!" });
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpGet("sistema")]
        public async Task<IActionResult> GetVinculos()
        {
            // Id do USUÁRIO: no token de vínculo o NameIdentifier é o id do vínculo e não serve aqui
            var usuarioId = User.GetUsuarioId();
            if (usuarioId == null) return Unauthorized("Usuário não autenticado.");

            var result = await _authService.GetVinculosAsync(usuarioId.Value);

            if (!result.Succeeded)
                return NotFound(new { mensagem = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpPost("sistema/{upcId:int}")]
        public async Task<IActionResult> SelecionarVinculo(int upcId)
        {
            var usuarioId = User.GetUsuarioId();
            if (usuarioId == null) return Unauthorized("Usuário não autenticado.");

            var result = await _authService.SelecionarVinculoAsync(upcId, usuarioId.Value);

            if (!result.Succeeded) return NotFound(result.Message);

            var responseData = (dynamic)result.Data;

            _tokenService.SetCookieToken(HttpContext, responseData.Token, User.GetClaimValue<bool>("rememberMe"));

            return Ok(responseData.Vinculo);
        }

        [Authorize(Policy = "GerenciaSistema")]
        [HttpPost("sistema/admin")]
        public async Task<IActionResult> SelecionarVinculoAdmin([FromBody] GetVinculoAdminDto dto)
        {
            var result = await _authService.SelecionarVinculoAdminAsync(dto);

            // Perfis inválidos (ex.: INATIVO) = 400; clínica inexistente = 404
            if (!result.Succeeded)
                return result.Errors?.Any() == true ? BadRequest(result) : NotFound(result.Message);

            var responseData = (dynamic)result.Data;
            _tokenService.SetCookieToken(HttpContext, responseData.Token, User.GetClaimValue<bool>("rememberMe"));

            return Ok(new { clinicaId = responseData.ClinicaId, perfisAtivos = responseData.PerfisAtivos });
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpPost("sistema/trocar-vinculo")]
        public async Task<IActionResult> TrocarVinculo()
        {
            var usuarioId = User.GetUsuarioId();
            if (usuarioId == null) return Unauthorized("Usuário não autenticado.");

            var result = await _authService.TrocarVinculoAsync(usuarioId.Value, User.GetClaimValue<bool>("acessoLogin"));

            if (!result.Succeeded)
                return BadRequest(result.Message);

            _tokenService.SetCookieToken(HttpContext, result.Data.ToString(), User.GetClaimValue<bool>("rememberMe"));
            return Ok();
        }

        [HttpPost("forgot-password")]
        [EnableRateLimiting(RateLimitAuth)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(model, _configuration["FrontendUrl"], HttpContext.GetClienteIp());

            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting(RateLimitAuth)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}
