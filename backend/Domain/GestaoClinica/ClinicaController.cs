using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using backend.Services;

using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Interface;

namespace backend.Domain.GestaoClinica
{
    [ApiController]
    [Route("api")]
    public class ClinicaController : ControllerBase
    {
        private readonly ILogger<ClinicaController> _logger;
        private readonly IClinicaService _clinicaService;
        public ClinicaController(ILogger<ClinicaController> logger,
                                 IClinicaService clinicaService)
        {
            _clinicaService = clinicaService;
            _logger = logger;
        }

        [HttpGet("sistema/clinicas")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetAll()
        {
            var clinicas = await _clinicaService.GetAllAsync();
            return Ok(clinicas);
        }

        [HttpGet("sistema/clinicas/{clinicaId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetById(int clinicaId)
        {
            var clinica = await _clinicaService.GetByIdAsync(clinicaId);
            if (clinica == null) return NotFound();
            return Ok(clinica);
        }

        [HttpPost("sistema/clinicas/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Create([FromBody] CadastrarClinicaDto dto)
        {
            var result = await _clinicaService.CreateAsync(dto);
            if (!result.Succeeded) return BadRequest(result);

            var clinicaDto = (GetClinicaDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { clinicaId = clinicaDto.Id }, clinicaDto);
        }

        [HttpPut("sistema/clinicas/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Update([FromBody] EditarClinicaAdminDto dto)
        {
            var result = await _clinicaService.UpdateClinicaAsync(dto);
            if (!result.Succeeded) return BadRequest(result);
            return NoContent();
        }


        [HttpDelete("sistema/clinicas/{clinicaId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Delete(int clinicaId)
        {
            var result = await _clinicaService.DeleteAsync(clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return NoContent();
        }

        [HttpGet("clinica/dashboard")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetPerfilDashboard()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0)
            {
                return Forbid("Acesso negado. Informação da clínica não encontrada no token.");
            }

            var result = await _clinicaService.GetClinicaDashboardAsync(clinicaId);

            if (!result.Succeeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("clinica/editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> UpdateOwnClinica([FromBody] EditarClinicaDto dto)
        {
            // Obtém o ID da clínica de forma segura a partir do token do usuário logado
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0)
            {
                return Forbid("Acesso negado. Informação da clínica não encontrada no token.");
            }

            var result = await _clinicaService.UpdateOwnClinicAsync(clinicaId, dto);

            // Retorna a resposta com base no resultado do serviço
            if (!result.Succeeded)
            {
                // O serviço pode retornar "Não encontrado" ou erros de validação
                return BadRequest(result);
            }

            return Ok(result); // Ou NoContent() para uma resposta 204
        }
    }
}