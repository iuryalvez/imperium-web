using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using backend.Domain.Auth.Extensions;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.DTOs.Racao;

namespace backend.Domain.SaudeAnimal.Controllers
{
    /// <summary>
    /// Rações vistas pela clínica (tenant): rações do sistema + rações próprias. O clinicaId vem sempre do token.
    /// </summary>
    [ApiController]
    [Route("api/clinica/racoes")]
    public class RacaoController : ControllerBase
    {
        private readonly IRacaoService _racaoService;
        public RacaoController(IRacaoService racaoService) { _racaoService = racaoService; }

        [HttpGet]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAllRacoes([FromQuery] RacaoFiltro filtro)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            return Ok(await _racaoService.ListarDaClinicaAsync(filtro, clinicaId));
        }

        [HttpGet("opcoes")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetOpcoes([FromQuery] string? busca, [FromQuery] EspecieAnimal? especie)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            return Ok(await _racaoService.ListarOpcoesDaClinicaAsync(clinicaId, busca, especie));
        }

        [HttpGet("{racaoId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetRacaoById(int racaoId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _racaoService.ObterDtoPorIdAsync(racaoId, clinicaId);
            if (!result.Succeeded) return NotFound(result);
            return Ok(result.Data);
        }

        [HttpPost("cadastrar")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> CreateRacao([FromBody] CadastrarRacaoDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _racaoService.CreateAsync(dto, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> UpdateRacao([FromBody] AtualizarRacaoDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _racaoService.UpdateAsync(dto, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{racaoId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> DeleteRacao(int racaoId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _racaoService.DeleteAsync(racaoId, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{racaoId}/status")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> SetStatus(int racaoId, [FromQuery] bool ativo)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _racaoService.SetStatusDaClinicaAsync(racaoId, clinicaId, ativo);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{racaoId}/ocultar")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> SetOculta(int racaoId, [FromQuery] bool oculta)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _racaoService.SetOcultaAsync(racaoId, clinicaId, oculta);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
