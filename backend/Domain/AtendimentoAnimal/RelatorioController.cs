using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.AtendimentoAnimal.Interface;
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
        public async Task<IActionResult> Get(int consultaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var relatorio = await _relatorioService.GetByConsultaIdAsync(consultaId, clinicaId);
            if (relatorio == null) return NotFound();
            return Ok(relatorio);
        }

        // Um único endpoint para Criar e Atualizar (Upsert)
        [HttpPut("consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Upsert(int consultaId, [FromBody] UpsertRelatorioDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _relatorioService.UpsertAsync(consultaId, dto, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("relatorios/{relatorioId}")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Delete(int relatorioId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _relatorioService.DeleteAsync(relatorioId, clinicaId);

            if (!result.Succeeded)
            {
                // O serviço retorna a mensagem de erro (ex: "Não encontrado" ou "Relatório já preenchido")
                return BadRequest(result);
            }
            return NoContent(); // Sucesso
        }
    }
}