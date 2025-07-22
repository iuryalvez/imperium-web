using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Infra.Data;
using System.Security.Claims;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.GestaoDeAcesso.DTOs;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.GestaoDeAcesso
{
    [ApiController]
    [Route("api/clinica")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(AppDbContext context,
                                 UserManager<Usuario> userManager,
                                 IUsuarioService usuarioService,
                                 ILogger<UsuarioController> logger)
        {
            _context = context;
            _userManager = userManager;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetUsuariosByClinicaId()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var usuarios = await _usuarioService.GetUsuariosByClinicIdAsync(clinicaId, userId);

            return Ok(usuarios);
        }

        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CadastrarUsuarioClinica([FromBody] CadastrarUsuarioClinicaDto cadastrarUsuarioClinicaDto)
        {
            // Validações iniciais (fora da transação pois são verificações rápidas)
            if (cadastrarUsuarioClinicaDto.Perfis == null || !cadastrarUsuarioClinicaDto.Perfis.Any())
            {
                return BadRequest("Pelo menos um perfil deve ser selecionado.");
            }

            // Inicia a transação atômica
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                Console.WriteLine("clinicaId que o usuário vai ser associado: " + clinicaId);
                // Verificação da clínica (dentro da transação)
                var clinicaExiste = await _context.Clinicas.AnyAsync(c => c.Id == clinicaId);
                if (!clinicaExiste)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("A clínica especificada não existe.");
                }

                // Verificação de e-mail único (dentro da transação)
                var existingUser = await _userManager.FindByEmailAsync(cadastrarUsuarioClinicaDto.Email);
                if (existingUser != null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("O e-mail já está em uso.");
                }

                // Criação do usuário (Identity)
                var usuario = new Usuario
                {
                    UserName = cadastrarUsuarioClinicaDto.UserName ?? cadastrarUsuarioClinicaDto.Email,
                    Email = cadastrarUsuarioClinicaDto.Email,
                    Nome = cadastrarUsuarioClinicaDto.Nome,
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    EmailConfirmed = true,
                    IsAdmin = false,
                    IsAtivo = true
                };

                var result = await _userManager.CreateAsync(usuario, cadastrarUsuarioClinicaDto.Senha);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
                }

                // Criação do vínculo (Entity Framework)
                var usuarioPerfilClinica = new UsuarioPerfilClinica
                {
                    UsuarioId = usuario.Id,
                    ClinicaId = clinicaId,
                    Perfis = cadastrarUsuarioClinicaDto.Perfis
                };

                _context.UsuariosPerfisClinicas.Add(usuarioPerfilClinica);
                await _context.SaveChangesAsync();

                // Confirma todas as operações
                await transaction.CommitAsync();

                return Ok(new { Message = "Usuário criado com sucesso.", UsuarioId = usuario.Id });
            }
            catch (Exception ex)
            {
                // Rollback automático em caso de erro não tratado
                await transaction.RollbackAsync();

                // Log do erro completo
                _logger.LogError(ex, "Erro durante o cadastro de usuário");

                return StatusCode(500, "Ocorreu um erro durante o cadastro. Por favor, tente novamente.");
            }
        }

        [HttpPut("usuarios/editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> EditarUsuarioClinica([FromBody] EditarUsuarioClinicaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usuarioService.UpdateUsuarioClinicaAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("vinculos/{upcId}")]
        [Authorize(Policy = "GerenciaSistema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlinkUserFromClinic(int upcId)
        {
            _logger.LogInformation("Recebida requisição para desvincular o vínculo ID: {UpcId}", upcId);

            // 1. Delega toda a lógica para o serviço
            var result = await _usuarioService.UnlinkUserFromClinicAsync(upcId);

            // 2. Interpreta o resultado do serviço e retorna a resposta HTTP apropriada
            if (!result.Succeeded)
            {
                // O serviço já tratou as regras de "não encontrado" e "último vínculo".
                // Ambas podem ser retornadas como um BadRequest para o cliente.
                _logger.LogWarning("Falha ao desvincular o vínculo ID {UpcId}. Motivo: {Message}", upcId, result.Message);
                return BadRequest(new { Message = result.Message });
            }

            _logger.LogInformation("Vínculo ID {UpcId} desvinculado com sucesso.", upcId);
            return Ok(result);
        }

        [HttpDelete("usuarios/{usuarioId}")]
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

        [HttpGet("vinculos")]
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

        // Rota age sobre o ID do VÍNCULO (upcId)
        [HttpPut("vinculos/{upcId}/status")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> SetClinicUserStatus(int upcId, [FromBody] SetActivationPayload payload)
        {
            var dto = new SetUserStatusDto
            {
                Id = upcId,
                IsAdmin = false, // Informa ao serviço que NÃO é um admin
                Activate = payload.Activate
            };

            var result = await _usuarioService.SetUserActivationStatusAsync(dto);

            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}