using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/sistema/racas")]
    public class RacaController : ControllerBase
    {
        private readonly IRacaService _racaService;

        public RacaController(IRacaService racaService)
        {
            _racaService = racaService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarRacas([FromQuery] EspecieAnimal? especie)
        {
            var racas = await _racaService.ListAllAsync(especie);
            return Ok(racas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterRacaPorId(int id)
        {
            var raca = await _racaService.GetByIdAsync(id);
            if (raca == null) return NotFound();
            return Ok(raca);
        }

        [HttpPost]
        [Authorize(Policy = "GerenciaSistema")] // Example of more specific policy
        public async Task<IActionResult> CriarRaca([FromBody] CadastrarRacaoDto criarRacaDto)
        {
            var racaCriada = await _racaService.CreateAsync(criarRacaDto);
            return CreatedAtAction(nameof(ObterRacaPorId), new { id = racaCriada.Id }, racaCriada);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> AtualizarRaca([FromBody] UpdateRacaDto atualizarRacaDto)
        {
            var sucesso = await _racaService.UpdateAsync(atualizarRacaDto);
            if (!sucesso) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeletarRaca(int id)
        {
            var sucesso = await _racaService.DeleteAsync(id);
            if (!sucesso) return NotFound();
            return NoContent();
        }
    }
}