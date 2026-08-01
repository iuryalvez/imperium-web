using backend.Domain.Auth.DTOs;
using backend.Domain.Usuarios.DTOs;
using backend.Domain.Auth.Interfaces;
using backend.Domain.Auth.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using System.Text.Json;

namespace backend.Domain.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
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

        // Método auxiliar privado para centralizar a lógica de extração do IP Real (Clean Code)
        private string GetRealIpAddress()
        {
            var forwardedHeader = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            return !string.IsNullOrEmpty(forwardedHeader)
                ? forwardedHeader.Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        [HttpGet("check")]
        [Authorize]
        public IActionResult CheckStatus()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized(new { status = "invalido" });

                if (!User.GetClaimValue<bool>("isAtivo")) return Unauthorized(new { status = "inativo" });

                bool isAdmin = User.GetClaimValue<bool>("isAdmin");
                var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                var nomeClinicaClaim = User.FindFirst("nomeClinica")?.Value;
                var perfisClaim = User.FindFirst("Perfis")?.Value;

                if (User.FindFirst("acessoLogin") != null)
                {
                    return Ok(new { status = "logado", isAdmin, name = userNameClaim });
                }

                return Ok(new
                {
                    status = "autenticado",
                    isAdmin,
                    name = userNameClaim,
                    nomeClinica = nomeClinicaClaim,
                    perfis = JsonSerializer.Deserialize<List<string>>(perfisClaim)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "erro", message = "Erro ao verificar status.", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var remoteIp = GetRealIpAddress();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginAsync(model, remoteIp, userAgent);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = result.Message });
            }

            // O result.Data já é a string do token gerado.
            string token = result.Data.ToString();

            // Lemos o RememberMe direto do model recebido pelo endpoint
            _tokenService.SetCookieToken(HttpContext, token, model.RememberMe);

            return Ok(new { tokenLogin = token });
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
                _logger.LogError(ex, "Erro ao efetuar logout.");
                return StatusCode(500, "Erro ao efetuar logout: " + ex.Message);
            }
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpGet("sistema")]
        public async Task<IActionResult> GetVinculos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _authService.GetVinculosAsync(userIdClaim);

            if (!result.Succeeded)
                return NotFound(new { mensagem = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpPost("sistema/{upcId}")]
        public async Task<IActionResult> SelecionarVinculo(int upcId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Usuário não autenticado.");

            var isAcessoLogin = User.GetClaimValue<bool>("acessoLogin");

            var remoteIp = GetRealIpAddress();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.SelecionarVinculoAsync(upcId, userIdClaim, isAcessoLogin, remoteIp, userAgent);

            if (!result.Succeeded) return NotFound(result.Message);

            var responseData = (dynamic)result.Data;

            var isRememberMe = User.GetClaimValue<bool>("rememberMe");

            // Pass the extracted state to keep the 7-day expiration if applicable
            _tokenService.SetCookieToken(HttpContext, responseData.Token, isRememberMe);

            return Ok(responseData.Vinculo);
        }

        [Authorize(Policy = "GerenciaSistema")]
        [HttpPost("sistema/admin")]
        public async Task<IActionResult> SelecionarVinculoAdmin([FromBody] GetVinculoAdminDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Usuário não autenticado.");

            var isAdmin = User.Claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
            if (!isAdmin) return Forbid("Apenas administradores podem selecionar vínculo administrativo.");

            var remoteIp = GetRealIpAddress();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var usuarioNome = User.FindFirst(ClaimTypes.Name)?.Value ?? "Administrador";

            var isRememberMe = User.GetClaimValue<bool>("rememberMe");

            var result = await _authService.SelecionarVinculoAdminAsync(dto, userIdClaim, remoteIp, userAgent, usuarioNome);

            if (!result.Succeeded)
                return NotFound(result.Message);

            var responseData = (dynamic)result.Data;
            _tokenService.SetCookieToken(HttpContext, responseData.Token, isRememberMe);

            return Ok(new { clinicaId = responseData.ClinicaId, perfisAtivos = responseData.PerfisAtivos });
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpPost("sistema/trocar-vinculo")]
        public async Task<IActionResult> TrocarVinculo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Usuário não autenticado.");

            var isAcessoLogin = User.GetClaimValue<bool>("acessoLogin");
            var isAdmin = User.GetClaimValue<bool>("isAdmin");

            var remoteIp = GetRealIpAddress();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.TrocarVinculoAsync(userIdClaim, isAcessoLogin, isAdmin, remoteIp, userAgent);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            var isRememberMe = User.GetClaimValue<bool>("rememberMe");

            _tokenService.SetCookieToken(HttpContext, result.Data.ToString(), isRememberMe);
            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var frontendUrl = _configuration["FrontendUrl"];
            var result = await _authService.ForgotPasswordAsync(model, frontendUrl);

            return Ok(new { message = result.Message });
        }

        [HttpPost("reset-password")]
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