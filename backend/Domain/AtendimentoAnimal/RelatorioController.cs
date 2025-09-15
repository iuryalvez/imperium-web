using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica")]
    [Authorize]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;
        public RelatorioController(IRelatorioService relatorioService) { _relatorioService = relatorioService; }

        [HttpGet("consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "VisualizaTudo")]
        [ProducesResponseType(typeof(ServiceResult), 200)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> Get(int consultaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            
            // O serviço agora retorna ServiceResult
            var result = await _relatorioService.GetDtoByConsultaIdAsync(consultaId, clinicaId);
            
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);

        }

        [HttpPut("consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaAnimais")]
        [ProducesResponseType(typeof(ServiceResult), 200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        public async Task<IActionResult> Upsert(int consultaId, [FromBody] UpsertRelatorioDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _relatorioService.UpsertAsync(consultaId, dto, clinicaId);
            
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("relatorios/{relatorioId}")]
        [Authorize(Policy = "GerenciaAnimais")]
        [ProducesResponseType(204)] // NoContent
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> Delete(int relatorioId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _relatorioService.DeleteAsync(relatorioId, clinicaId);

            if (result.Succeeded)
            {
                return NoContent(); // 204 é o ideal para um delete bem-sucedido
            }
            
            return NotFound(result);
        }
    }
}