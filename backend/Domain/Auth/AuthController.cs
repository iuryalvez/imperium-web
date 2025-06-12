using backend.Infra.Data;
using backend.Interfaces;

using backend.Domain.GestaoDeAcesso.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using backend.Services;
using System.Text.Json;
using backend.Domain.Auth.DTOs;
using backend.Domain.GestaoDeAcesso.DTOs;

namespace backend.Domain.Auth
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

        public AuthController(UserManager<Usuario> userManager,
                              SignInManager<Usuario> signInManager, 
                              ILogger<AuthController> logger,
                              IEmailService emailService,
                              ITokenService tokenService, 
                              AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpGet("check")]
        public IActionResult CheckStatus()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                var nomeClinicaClaim = User.FindFirst("nomeClinica")?.Value;
                var perfisClaim = User.FindFirst("Perfis")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { status = "invalido" });
                }

                if (!User.GetClaimValue<bool>("isAtivo")) {
                    return Unauthorized(new { status = "inativo" });
                }

                bool IsAdmin = User.GetClaimValue<bool>("isAdmin");

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
                    nomeClinica = nomeClinicaClaim,
                    perfis = JsonSerializer.Deserialize<List<string>>(perfisClaim) // Lista apenas os perfis ativos
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

        /// <summary>
        /// Faz o login de um usuário.
        /// </summary>
        /// <param name="model">O login data transfer object.</param>
        /// <returns>IActionResult indicando o retorno da requisição.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            _logger.LogDebug("Login solicitado para o e-mail {Email}.", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido para login do e-mail {Email}.", model.Email);
                return BadRequest(ModelState);
            }

            var usuario = await _userManager.FindByEmailAsync(model.Email);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário com e-mail {Email} não encontrado.", model.Email);
                return Unauthorized(new { Message = "Usuário não encontrado." });
            }

            if (!usuario.EmailConfirmed)
            {
                _logger.LogWarning("Usuário {Email} tentou login sem confirmar e-mail.", model.Email);
                return Unauthorized(new { Message = "Confirme seu email antes de fazer login." });
            }

            var senhaValida = await _userManager.CheckPasswordAsync(usuario, model.Senha);

            if (!senhaValida)
            {
                _logger.LogWarning("Senha incorreta para usuário {Email}.", model.Email);
                return Unauthorized(new { Message = "Senha incorreta." });
            }

            usuario.DataUltimoAcesso = DateTime.UtcNow;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            var usuarioIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var usuarioAgent = Request.Headers["User-Agent"].ToString();
            var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), usuarioIp, usuarioAgent);
            _tokenService.SetCookieToken(HttpContext, tokenLogin);

            _logger.LogInformation("Usuário {Email} logado com sucesso. IP: {IP}, Agent: {Agent}", model.Email, usuarioIp, usuarioAgent);

            return Ok(new { tokenLogin });
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            try
            {
                _logger.LogInformation("Logout solicitado por IP {IP}.", HttpContext.Connection.RemoteIpAddress);
                HttpContext.Response.Cookies.Delete("auth_token");
                return Ok(new { message = "Logout efetuado com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao efetuar logout.");
                return StatusCode(500, "Erro ao efetuar logout: " + ex.Message);
            }
        }

        // TODO
        // [HttpPost("forgot-password")]
        // public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        // {
        //     _logger.LogDebug("Solicitação de recuperação de senha recebida para o e-mail {Email}.", model.Email);

        //     if (!ModelState.IsValid)
        //     {
        //         _logger.LogWarning("ModelState inválido em ForgotPassword para o e-mail {Email}.", model.Email);
        //         return BadRequest(ModelState);
        //     }

        //     var user = await _userManager.FindByEmailAsync(model.Email);
        //     if (user == null)
        //     {
        //         _logger.LogWarning("Usuário não encontrado ao solicitar recuperação de senha para o e-mail {Email}.", model.Email);
        //         return Ok(new { Message = "Se o e-mail estiver registrado, um link de redefinição de senha será enviado." });
        //     }

        //     var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //     var callbackUrl = Url.Action("ResetPassword", "Auth", new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);

        //     await _emailService.SendEmailAsync(user.Email, "Redefinir Senha", 
        //         $"Por favor, redefina sua senha clicando <a href='{callbackUrl}'>aqui</a>. O link expira em 30 minutos.");

        //     _logger.LogInformation("E-mail de redefinição de senha enviado para {Email}.", user.Email);

        //     return Ok(new { Message = "Se o e-mail estiver registrado, um link de redefinição de senha será enviado." });
        // }

        // TODO
        // [HttpPost("reset-password")]
        // public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        // {
        //     _logger.LogDebug("Solicitação de redefinição de senha recebida para o e-mail {Email}.", model.Email);

        //     if (!ModelState.IsValid)
        //     {
        //         _logger.LogWarning("ModelState inválido para ResetPassword do e-mail {Email}.", model.Email);
        //         return BadRequest(ModelState);
        //     }

        //     var user = await _userManager.FindByEmailAsync(model.Email);
        //     if (user == null)
        //     {
        //         _logger.LogWarning("Usuário não encontrado ao redefinir senha para o e-mail {Email}.", model.Email);
        //         return Ok(new { Message = "Senha redefinida com sucesso." });
        //     }

        //     var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        //     if (result.Succeeded)
        //     {
        //         _logger.LogInformation("Senha redefinida com sucesso para o e-mail {Email}.", user.Email);
        //         return Ok(new { Message = "Senha redefinida com sucesso." });
        //     }

        //     foreach (var error in result.Errors)
        //     {
        //         ModelState.AddModelError(string.Empty, error.Description);
        //         _logger.LogWarning("Erro ao redefinir senha para o e-mail {Email}: {Error}", user.Email, error.Description);
        //     }

        //     return BadRequest(ModelState);
        // }

        // TODO
        // [HttpPost("send-email")]
        // public async Task<IActionResult> SendEmail([FromBody] string userId)
        // {
        //     _logger.LogDebug("Solicitação de envio de e-mail recebida para o usuário ID {UserId}.", userId);

        //     var user = await _userManager.FindByIdAsync(userId);
        //     if (user == null)
        //     {
        //         _logger.LogWarning("Usuário com ID {UserId} não encontrado para envio de e-mail.", userId);
        //         return NotFound("Usuário não encontrado");
        //     }

        //     if (string.IsNullOrEmpty(user.Email))
        //     {
        //         _logger.LogWarning("Usuário com ID {UserId} possui e-mail vazio ou nulo.", userId);
        //         throw new ArgumentNullException(nameof(user.Email), "O e-mail do usuário não pode ser nulo ou vazio");
        //     }

        //     await _emailService.SendEmailAsync(user.Email, "Assunto do E-mail", "Mensagem do E-mail");

        //     _logger.LogInformation("E-mail enviado com sucesso para o usuário com ID {UserId}.", userId);

        //     return Ok("E-mail enviado com sucesso");
        // }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpGet("sistema")]
        public async Task<IActionResult> GetVinculos()
        {
            var usuarioId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);
            if (usuarioId == 0)
            {
                return BadRequest("Erro ao identificar usuário.");
            }

            var vinculos = await _context.UsuariosPerfisClinicas
                .Where(p => p.UsuarioId == usuarioId && p.Perfis.Any(perfil => perfil != "INATIVO")) // Verifica se há perfis ativos
                .Select(p => new GetVinculoDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    ClinicaId = p.ClinicaId,
                    NomeClinica = p.Clinica.NomeClinica,
                    Perfis = p.Perfis // Retorna lista de perfis ativos
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
                var isAcessoLogin = User.GetClaimValue<bool>("acessoLogin");

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
                    .AsNoTracking()
                    .Include(v => v.Usuario)
                    .Include(v => v.Clinica)
                    .FirstOrDefaultAsync(v => v.Id == upcId && v.UsuarioId == usuarioId &&
                                            !v.Perfis.Contains("INATIVO")); // Garante que "Inativo" não esteja na lista

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

                var vinculoDto = new GetVinculoDto
                {
                    Id = vinculo.Id,
                    UsuarioId = vinculo.UsuarioId,
                    ClinicaId = vinculo.ClinicaId,
                    NomeClinica = vinculo.Clinica?.NomeClinica,
                    Perfis = vinculo.Perfis
                };

                // Retorna os perfis ativos
                return Ok(vinculoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao selecionar vínculo: " + ex.Message);
            }
        }

        [Authorize(Policy = "GerenciaSistema")]
        [HttpPost("sistema/admin")] // Nova rota específica para admins
        public async Task<IActionResult> SelecionarVinculoAdmin([FromBody] GetVinculoAdminDto dto)
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

                _logger.LogInformation("Admin usuarioId: {UsuarioId} selecionou clínicaId: {ClinicaId} com perfis: {Perfis}", userIdClaim, dto.ClinicaId, string.Join(", ", dto.Perfis));

                // Retorna os dados simulados do vínculo
                return Ok(new { clinicaId = dto.ClinicaId, perfisAtivos = dto.Perfis });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao selecionar vínculo administrativo: " + ex.Message);
            }
        }

        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        [HttpPost("sistema/trocar-vinculo")]
        public async Task<IActionResult> TrocarVinculo()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Usuário não autenticado.");

                var isAcessoLogin = User.GetClaimValue<bool>("acessoLogin");
                var isAdmin = User.GetClaimValue<bool>("isAdmin");

                // Se já estiver com token de login, não precisa trocar
                if (isAcessoLogin)
                    return BadRequest("Token atual já é de login.");

                string usuarioId;

                if (isAdmin)
                {
                    // Admins com token de vínculo: o NameIdentifier ainda é o ID do admin real
                    usuarioId = userIdClaim;
                }
                else
                {
                    // Usuário comum com vínculo: o NameIdentifier é o ID do vínculo
                    var vinculoId = int.Parse(userIdClaim);

                    var vinculo = await _context.UsuariosPerfisClinicas
                        .AsNoTracking()
                        .FirstOrDefaultAsync(v => v.Id == vinculoId);

                    if (vinculo == null)
                        return Unauthorized("Vínculo não encontrado.");

                    usuarioId = vinculo.UsuarioId.ToString();
                }

                var usuario = await _userManager.FindByIdAsync(usuarioId);
                if (usuario == null)
                    return Unauthorized("Usuário não encontrado.");

                var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var tokenLogin = _tokenService.GenerateTokenLogin(usuario.Id.ToString(), userIp, userAgent);

                _tokenService.SetCookieToken(HttpContext, tokenLogin);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao trocar vínculo: {ex.Message}");
            }
        }
    }
}
