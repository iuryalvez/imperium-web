using Microsoft.AspNetCore.Mvc;

using backend.Services;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using System.Security.Claims;

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica/consultas")]
    public class ConsultaController : ControllerBase
    {
        private readonly IConsultaService _consultaService;
        public ConsultaController(IConsultaService consultaService) { _consultaService = consultaService; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var consultas = await _consultaService.GetAllByClinicIdAsync(clinicaId);
            return Ok(consultas);
        }

        [HttpGet("{consultaId}")]
        public async Task<IActionResult> GetById(int consultaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var consulta = await _consultaService.GetDetailsByIdAsync(consultaId, clinicaId);
            if (consulta == null) return NotFound();
            return Ok(consulta);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpsertConsultaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var usuarioId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);

            var result = await _consultaService.CreateAsync(dto, clinicaId, usuarioId);
            if (!result.Succeeded) return BadRequest(result);

            var consultaDto = (DetalhesConsultaDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { consultaId = consultaDto.Id }, consultaDto);
        }

        [HttpPut("{consultaId}")]
        public async Task<IActionResult> Update(int consultaId, [FromBody] UpsertConsultaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _consultaService.UpdateAsync(consultaId, dto, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return NoContent();
        }

        [HttpDelete("{consultaId}")]
        public async Task<IActionResult> Delete(int consultaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _consultaService.DeleteAsync(consultaId, clinicaId);

            if (!result.Succeeded)
            {
                // O serviço retorna a mensagem de erro apropriada (ex: "Não encontrado" ou "Relatório já preenchido")
                return BadRequest(result);
            }

            // Retorno padrão para um DELETE bem-sucedido.
            return NoContent();
        }
    }
}