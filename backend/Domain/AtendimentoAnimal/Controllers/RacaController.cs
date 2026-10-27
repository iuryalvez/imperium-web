using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Auth.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Domain.AtendimentoAnimal.Controllers
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
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAll([FromQuery] EspecieAnimal? especie)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");

            var racas = await _racaService.ListAvailableForClinicaAndEspecieAsync(clinicaId, especie);

            return Ok(racas);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetById(int id)
        {
            var raca = await _racaService.GetByIdAsync(id);
            if (raca == null) return NotFound();
            return Ok(raca);
        }

        [HttpPost]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Create([FromBody] CadastrarRacaDto criarRacaDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var racaCriada = await _racaService.CreateAsync(criarRacaDto, clinicaId);
            return CreatedAtAction(nameof(GetById), new { id = racaCriada.Id }, racaCriada);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Update([FromBody] UpdateRacaDto atualizarRacaDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var sucesso = await _racaService.CreateOrUpdateAsync(atualizarRacaDto, clinicaId);
            if (!sucesso) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await _racaService.DeleteAsync(id);
            if (!sucesso) return NotFound();
            return NoContent();
        }
    }
}