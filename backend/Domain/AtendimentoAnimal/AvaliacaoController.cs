using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Auth.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.AtendimentoAnimal.Services;

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica")]
    [Authorize]
    public class AvaliacaoController : ControllerBase
    {
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly IAcompanhamentoService _acompanhamentoService;
        public AvaliacaoController(IAvaliacaoService avaliacaoService, IAcompanhamentoService acompanhamentoService)
        {
            _avaliacaoService = avaliacaoService;
            _acompanhamentoService = acompanhamentoService;
        }

        [HttpGet("consultas/{consultaId}/avaliacao")]
        [Authorize(Policy = "VisualizaTudo")]
        [ProducesResponseType(typeof(ServiceResult), 200)]
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> Get(int consultaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");

            // O serviço agora retorna ServiceResult
            var result = await _avaliacaoService.GetDtoByConsultaIdAsync(consultaId, clinicaId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpPut("consultas/{consultaId}/avaliacao")]
        [Authorize(Policy = "GerenciaAnimais")]
        [ProducesResponseType(typeof(ServiceResult), 200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        public async Task<IActionResult> Upsert(int consultaId, [FromBody] UpsertAvaliacaoDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _avaliacaoService.UpsertAsync(consultaId, dto, clinicaId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("avaliacoes/{avaliacaoId}")]
        [Authorize(Policy = "GerenciaAnimais")]
        [ProducesResponseType(204)] // NoContent
        [ProducesResponseType(typeof(ServiceResult), 404)]
        public async Task<IActionResult> Delete(int avaliacaoId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _avaliacaoService.DeleteAsync(avaliacaoId, clinicaId);

            if (result.Succeeded)
            {
                return NoContent(); // 204 é o ideal para um delete bem-sucedido
            }

            return NotFound(result);
        }
    }
}