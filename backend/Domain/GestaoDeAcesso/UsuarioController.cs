using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Infra.Data;
using System.Security.Claims;
using backend.Domain.GestaoDeAcesso.DTOs;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.GestaoDeAcesso.DTOs.PerfilPessoal;

namespace backend.Domain.GestaoDeAcesso
{
    [ApiController]
    [Route("api")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(AppDbContext context,
                                 IUsuarioService usuarioService,
                                 ILogger<UsuarioController> logger)
        {
            _context = context;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        // --- Endpoints de SISTEMA (ADMIN) --- 

        [HttpGet("sistema/usuarios")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> ListarUsuariosSistema()
        {
            _logger.LogInformation("Recebida requisição para listar usuários do sistema.");
            var usuarioAtualId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);
            var result = await _usuarioService.ListarUsuariosSistemaAsync(usuarioAtualId);

            return Ok(result);
        }

        [HttpPost("sistema/usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> CadastrarUsuarioSistema([FromBody] CadastrarUsuarioSistemaDto dto)
        {
            _logger.LogInformation("Recebida requisição para cadastrar novo usuário de sistema.");
            // Validação do ModelState é automática com [ApiController], mas podemos checar se quisermos.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usuarioService.CadastrarUsuarioSistemaAsync(dto);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao cadastrar usuário de sistema: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Usuário de sistema cadastrado com sucesso.");
            return Ok(result);
        }

        [HttpPut("sistema/usuarios/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> EditarUsuarioSistema([FromBody] EditarUsuarioSistemaDto dto)
        {
            _logger.LogInformation("Recebida requisição para editar usuário de sistema ID: {UsuarioId}", dto.Id);

            var result = await _usuarioService.UpdateUsuarioSistemaAsync(dto);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao editar usuário de sistema ID {UsuarioId}: {Message}", dto.Id, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Usuário de sistema ID {UsuarioId} editado com sucesso.", dto.Id);
            return Ok(result);
        }

        // --- Endpoints de CLÍNICA (GESTOR e ADMIN)


        [HttpGet("clinica/usuarios")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> ListarUsuariosClinica()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            _logger.LogInformation("Recebida requisição para listar usuários da clínica ID: {ClinicaId}", clinicaId);
            var userId = User.GetClaimValue<string>(ClaimTypes.NameIdentifier);

            var usuarios = await _usuarioService.GetUsuariosByClinicIdAsync(clinicaId, userId);

            return Ok(usuarios);
        }

        [HttpPost("clinica/usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CadastrarUsuarioClinica([FromBody] CadastrarUsuarioClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            _logger.LogInformation("Recebida requisição para cadastrar novo usuário na clínica ID: {ClinicaId}", clinicaId);

            var result = await _usuarioService.CadastrarUsuarioClinicaAsync(dto, clinicaId);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao cadastrar usuário na clínica {ClinicaId}: {Message}", clinicaId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Usuário cadastrado com sucesso na clínica {ClinicaId}.", clinicaId);
            return Ok(result);
        }

        [HttpPut("clinica/usuarios/editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> EditarUsuarioClinica([FromBody] EditarUsuarioClinicaDto dto)
        {
            _logger.LogInformation("Recebida requisição para editar usuário (Vínculo ID: {UpcId})", dto.UpcId);

            var result = await _usuarioService.UpdateUsuarioClinicaAsync(dto);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao editar usuário (Vínculo ID: {UpcId}): {Message}", dto.UpcId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Usuário (Vínculo ID: {UpcId}) editado com sucesso.", dto.UpcId);
            return Ok(result);
        }

        // Endpoints de VÍNCULO

        [HttpGet("vinculos")]
        [Authorize(Policy = "AcessoLoginPolicy")]
        public async Task<IActionResult> GetAllVinculos()
        {
            var vinculos = await _context.UsuariosPerfisClinicas
                .AsNoTracking()
                .ToListAsync();

            if (!vinculos.Any())
            {
                return NotFound(new { Message = "Nenhum vínculo encontrado." });
            }

            return Ok(vinculos);
        }

        [HttpPut("sistema/usuarios/{usuarioId}/status")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> SetAdminUserStatus(int usuarioId, [FromBody] SetActivationPayload payload)
        {
            _logger.LogInformation("Recebida requisição para alterar status do Vínculo/Usuário ID: {Id}", usuarioId);
            var dto = new SetUserStatusDto
            {
                Id = usuarioId,
                IsAdmin = true, // Informa ao serviço que é um admin
                Activate = payload.Activate
            };

            var result = await _usuarioService.SetUserActivationStatusAsync(dto);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao alterar status para o Vínculo/Usuário ID {Id}: {Message}", dto.Id, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Status alterado com sucesso para Vínculo/Usuário ID {Id}.", dto.Id);

            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("clinica/vinculos/{upcId}/status")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> SetClinicUserStatus(int upcId, [FromBody] SetActivationPayload payload)
        {
            _logger.LogInformation("Recebida requisição para alterar status do Vínculo/Usuário ID: {Id}", upcId);
            var dto = new SetUserStatusDto
            {
                Id = upcId,
                IsAdmin = false, // Informa ao serviço que NÃO é um admin
                Activate = payload.Activate
            };

            var result = await _usuarioService.SetUserActivationStatusAsync(dto);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao alterar status para o Vínculo/Usuário ID {Id}: {Message}", dto.Id, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Status alterado com sucesso para Vínculo/Usuário ID {Id}.", dto.Id);

            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("vinculos/{upcId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> UnlinkUserFromClinic(int upcId)
        {
            _logger.LogInformation("Recebida requisição para desvincular o vínculo ID: {UpcId}", upcId);

            // 1. Delega toda a lógica para o serviço
            var result = await _usuarioService.UnlinkUserFromClinicAsync(upcId);

            // 2. Interpreta o resultado do serviço e retorna a resposta HTTP apropriada
            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao desvincular o vínculo ID {UpcId}. Motivo: {Message}", upcId, result.Message);
                return BadRequest(new { Message = result.Message });
            }

            _logger.LogInformation("Vínculo ID {UpcId} desvinculado com sucesso.", upcId);
            return Ok(result);
        }

        [HttpDelete("clinica/usuarios/{usuarioId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteUsuario(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Remove o usuário do banco de dados
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuário deletado com sucesso." });
        }

        [HttpGet("debug-claims")]
        [Authorize(Policy = "GerenciaSistema")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        // Endpoints de PERFIL PESSOAL (TODOS USUÁRIOS)

        [HttpGet("sistema/perfil")]
        [Authorize(Policy = "AcessoLoginPolicy")]
        public async Task<IActionResult> GetMeuPerfil()
        {
            var usuarioId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);
            var result = await _usuarioService.GetMeuPerfilAsync(usuarioId);

            if (!result.Succeeded) return NotFound(result);
            return Ok(result);
        }

        [HttpPut("sistema/perfil")]
        [Authorize(Policy = "AcessoLoginPolicy")]
        public async Task<IActionResult> UpdateMeuPerfil([FromBody] UpdateMeuPerfilDto dto)
        {
            var usuarioId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);
            var result = await _usuarioService.UpdateMeuPerfilAsync(usuarioId, dto);

            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}