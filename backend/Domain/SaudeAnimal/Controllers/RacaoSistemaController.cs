using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.SaudeAnimal.DTOs.Racao;

namespace backend.Domain.SaudeAnimal.Controllers
{
    /// <summary>
    /// Gestão das rações pelo administrador do Imperium: catálogo do sistema e rações próprias das clínicas.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class RacaoSistemaController : ControllerBase
    {
        private readonly IRacaoService _racaoService;
        public RacaoSistemaController(IRacaoService racaoService) { _racaoService = racaoService; }

        [HttpGet("sistema/racoes")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetAll([FromQuery] RacaoFiltro filtro)
        {
            return Ok(await _racaoService.ListarSistemaAsync(filtro));
        }

        [HttpGet("sistema/racoes/copias")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetCopias([FromQuery] RacaoFiltro filtro)
        {
            return Ok(await _racaoService.ListarCopiasAsync(filtro));
        }

        [HttpGet("sistema/racoes/{racaoId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetById(int racaoId)
        {
            var result = await _racaoService.ObterAsync(racaoId);
            if (!result.Succeeded) return NotFound(result);
            return Ok(result.Data);
        }

        [HttpPost("sistema/racoes/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Create([FromBody] CadastrarRacaoDto dto)
        {
            var result = await _racaoService.CadastrarSistemaAsync(dto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("sistema/racoes/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Update([FromBody] AtualizarRacaoDto dto)
        {
            var result = await _racaoService.EditarAsync(dto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("sistema/racoes/{racaoId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Delete(int racaoId)
        {
            var result = await _racaoService.ExcluirAsync(racaoId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("sistema/racoes/{racaoId}/status")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> SetStatus(int racaoId, [FromQuery] bool ativo)
        {
            var result = await _racaoService.SetStatusAsync(racaoId, ativo);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
