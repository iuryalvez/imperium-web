using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using backend.Infra.Data;
using System.Security.Claims;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.GestaoClinica.Interface;
using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoDeAcesso.DTOs;
using backend.Services;
using backend.Domain.AtendimentoAnimal;
using backend.Domain.GestaoDeAcesso.Interfaces;

// TODO: modularizar funções repetidas em todos os controladores, usar services ou o que for necessário
namespace backend.Domain.Gestao
{
    [ApiController]
    [Route("api/sistema")]
    public class GestaoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<GestaoController> _logger;
        private readonly IClinicaService _clinicaService;
        private readonly IUsuarioService _usuarioService;

        public GestaoController(AppDbContext context,
                                 UserManager<Usuario> userManager,
                                 IClinicaService clinicaInterface,
                                 IUsuarioService usuarioService,
                                 ILogger<GestaoController> logger)
        {
            _context = context;
            _userManager = userManager;
            _clinicaService = clinicaInterface;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpGet("clinicas/buscar")]
        [Authorize(Policy = "AcessoLoginPolicy")]
        public async Task<IActionResult> BuscarClinicasPorNome([FromQuery] string nome)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                    return BadRequest("O nome da clínica não pode ser vazio.");

                var clinicas = await _context.Clinicas
                    .Where(c => EF.Functions.ILike(c.NomeClinica, $"%{nome}%")) // ILike é case-insensitive (PostgreSQL)
                    .Select(c => new
                    {
                        value = c.Id,
                        label = c.NomeClinica
                    })
                    .ToListAsync();

                return Ok(clinicas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clínica por nome.");
                return StatusCode(500, "Erro interno ao buscar clínica por nome.");
            }
        }

        [HttpGet("clinicas")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetClinicas()
        {
            var clinicas = await _clinicaService.GetAllAsync();
            return Ok(clinicas);
        }

        // get clinica by clinicaId
        [HttpGet("clinicas/{clinicaId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<GetClinicaDto>> GetClinica(int clinicaId)
        {
            var clinica = await _clinicaService.ObterClinicaAsync(clinicaId);

            if (clinica == null)
            {
                return NotFound();
            }

            return Ok(clinica);
        }

        // criar clinica pelo sistema
        [HttpPost("clinicas/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<CadastrarClinicaDto>> CadastrarClinica([FromBody] CadastrarClinicaDto dto)
        {
            var novaClinica = new Clinica
            {
                NomeClinica = dto.NomeClinica,
                IsAtiva = dto.IsAtiva,
                DataCad = DateTime.UtcNow
            };

            _context.Clinicas.Add(novaClinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinica), new { clinicaId = novaClinica.Id }, dto);
        }

        [HttpPut("clinicas/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> EditarClinicaAdmin([FromBody] EditarClinicaAdminDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _clinicaService.UpdateClinicaAsync(dto);

            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }


        [HttpDelete("clinicas/{clinicaId}")]
        [Authorize(Policy = "GerenciaSistema")] // Descomente quando implementar a autenticação
        public async Task<IActionResult> DeleteClinica(int clinicaId)
        {
            // Verificação inicial do ID
            if (clinicaId <= 0)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "ID da clínica inválido.",
                    ErrorCode = "INVALID_ID"
                });
            }

            try
            {
                // Busca a clínica incluindo possíveis relacionamentos que precisam ser tratados
                var clinica = await _context.Clinicas
                    .Include(c => c.UsuariosPerfisClinicas) // Exemplo: incluir relacionamentos que podem impedir a deleção
                    .FirstOrDefaultAsync(c => c.Id == clinicaId);

                if (clinica == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Clínica não encontrada.",
                        ErrorCode = "CLINICA_NOT_FOUND"
                    });
                }

                // Verifica se há dependências que impedem a exclusão
                if (clinica.UsuariosPerfisClinicas?.Any() == true)
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = "Não é possível excluir a clínica pois existem usuários vinculados.",
                        ErrorCode = "HAS_DEPENDENCIES",
                        Data = new { UsersCount = clinica.UsuariosPerfisClinicas.Count }
                    });
                }

                // Inicia transação para operação atômica
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.Clinicas.Remove(clinica);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Clínica {clinicaId} deletada com sucesso.");

                    return NoContent();
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();

                    _logger.LogError(ex, $"Erro ao deletar clínica {clinicaId}");

                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Success = false,
                        Message = "Erro ao deletar clínica no banco de dados.",
                        ErrorCode = "DATABASE_ERROR",
                        Detailed = ex.InnerException?.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro geral ao processar deleção da clínica {clinicaId}");

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Erro interno no servidor.",
                    ErrorCode = "INTERNAL_ERROR",
                    Detailed = ex.Message
                });
            }
        }

        [HttpGet("usuarios/buscar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> BuscarUsuariosPorNome([FromQuery] string nome)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nome))
                    return BadRequest("O nome do usuário não pode ser vazio.");

                var usuarios = await _context.Usuarios
                    .Where(u => EF.Functions.ILike(u.Nome, $"%{nome}%")) // ILike é case-insensitive (PostgreSQL)
                    .Select(u => new
                    {
                        value = u.Id,
                        label = u.Nome
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por nome.");
                return StatusCode(500, "Erro interno ao buscar usuário por nome.");
            }
        }

        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarioAtualId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);
            var result = await _usuarioService.ListarUsuariosSistemaAsync(usuarioAtualId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> CadastrarUsuarioSistema([FromBody] CadastrarUsuarioSistemaDto dto)
        {
            var result = await _usuarioService.CadastrarUsuarioSistemaAsync(dto);

            if (!result.Succeeded)
            {
                // Se houver erros específicos do Identity, retorna eles. Senão, a mensagem geral.
                if (result.Errors.Any())
                {
                    return BadRequest(new { Errors = result.Errors });
                }
                return BadRequest(new { Message = result.Message });
            }

            return Ok(result);
        }

        [HttpPut("usuarios/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> EditarUsuarioSistema([FromBody] EditarUsuarioSistemaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _usuarioService.UpdateUsuarioSistemaAsync(dto);

            if (!result.Succeeded)
            {
                // Se houver erros específicos (ex: senha), retorna eles. Senão, a mensagem geral.
                return BadRequest(new { result.Message, result.Errors });
            }

            return Ok(result);
        }

        [HttpPut("usuarios/{usuarioId}/status")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> SetAdminUserStatus(int usuarioId, [FromBody] SetActivationPayload payload)
        {
            var dto = new SetUserStatusDto
            {
                Id = usuarioId,
                IsAdmin = true, // Informa ao serviço que é um admin
                Activate = payload.Activate
            };

            var result = await _usuarioService.SetUserActivationStatusAsync(dto);

            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}