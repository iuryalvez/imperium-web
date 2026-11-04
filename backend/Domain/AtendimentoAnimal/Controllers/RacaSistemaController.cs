using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Domain.Utils;

namespace backend.Domain.AtendimentoAnimal.Controllers
{
    /// <summary>
    /// Gestão das raças pelo administrador do Imperium: catálogo do sistema e cópias feitas pelas clínicas.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class RacaSistemaController : ControllerBase
    {
        private readonly IRacaService _racaService;

        public RacaSistemaController(IRacaService racaService)
        {
            _racaService = racaService;
        }

        [HttpGet("sistema/racas")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetAll([FromQuery] RacaFiltro filtro)
        {
            return Ok(await _racaService.ListarSistemaAsync(filtro));
        }

        [HttpGet("sistema/racas/copias")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetCopias([FromQuery] RacaFiltro filtro)
        {
            return Ok(await _racaService.ListarCopiasAsync(filtro));
        }

        [HttpGet("sistema/racas/{id}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _racaService.ObterAsync(id);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result.Data);
        }

        [HttpPost("sistema/racas/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Create([FromBody] CadastrarRacaDto dto)
        {
            var result = await _racaService.CadastrarSistemaAsync(dto);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpPut("sistema/racas/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Update([FromBody] UpdateRacaDto dto)
        {
            var result = await _racaService.EditarAsync(dto);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpDelete("sistema/racas/{id}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _racaService.ExcluirAsync(id);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpPut("sistema/racas/{id}/status")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] bool ativo)
        {
            var result = await _racaService.SetStatusAsync(id, ativo);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }
    }
}
