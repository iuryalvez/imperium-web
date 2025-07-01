using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/sistema/raca")]
    public class RacaController : ControllerBase
    {
        private readonly RacaInterface _racaInterface;

        public RacaController(RacaInterface racaInterface)
        {
            _racaInterface = racaInterface;
        }

        [HttpGet]
        public async Task<IActionResult> ListarRacas([FromQuery] EspecieAnimal? especie)
        {
            var racas = await _racaInterface.ListAllAsync(especie);
            return Ok(racas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterRacaPorId(int id)
        {
            var raca = await _racaInterface.GetByIdAsync(id);
            if (raca == null) return NotFound();
            return Ok(raca);
        }

        [HttpPost]
        [Authorize(Policy = "GerenciaSistema")] // Example of more specific policy
        public async Task<IActionResult> CriarRaca([FromBody] CriarRacaoDto criarRacaDto)
        {
            var racaCriada = await _racaInterface.CreateAsync(criarRacaDto);
            return CreatedAtAction(nameof(ObterRacaPorId), new { id = racaCriada.Id }, racaCriada);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> AtualizarRaca(int id, [FromBody] AtualizarRacaDto atualizarRacaDto)
        {
            var sucesso = await _racaInterface.UpdateAsync(id, atualizarRacaDto);
            if (!sucesso) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeletarRaca(int id)
        {
            var sucesso = await _racaInterface.DeleteAsync(id);
            if (!sucesso) return NotFound();
            return NoContent();
        }
    }
}