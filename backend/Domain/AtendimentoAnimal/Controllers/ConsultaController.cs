using Microsoft.AspNetCore.Mvc;

using backend.Domain.Auth.Extensions;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace backend.Domain.AtendimentoAnimal.Controllers
{
    [ApiController]
    [Route("api/clinica/consultas")]
    public class ConsultaController : ControllerBase
    {
        private readonly IConsultaService _consultaService;
        public ConsultaController(IConsultaService consultaService) { _consultaService = consultaService; }

        [HttpGet]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAllByClinicaId([FromQuery] ConsultaFiltro filtro)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            return Ok(await _consultaService.ListarAsync(filtro, clinicaId));
        }

        [HttpGet("{consultaId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetById(int consultaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var consulta = await _consultaService.ObterDetalhesPorIdEClinicaIdAsync(consultaId, clinicaId);
            if (consulta == null) return NotFound();
            return Ok(consulta);
        }

        [HttpPost("cadastrar")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> Create([FromBody] UpsertConsultaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            // Id do usuário (não do vínculo): quem marcou a consulta
            var usuarioId = User.GetUsuarioId();
            if (usuarioId == null) return Forbid();

            var result = await _consultaService.CreateAsync(dto, clinicaId, usuarioId.Value);
            if (!result.Succeeded) return BadRequest(result);

            var consultaDto = (DetalhesConsultaDto)result.Data;
            return CreatedAtAction(nameof(GetById), new { consultaId = consultaDto.Id }, consultaDto);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> Update([FromBody] EditarConsultaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            var result = await _consultaService.UpdateAsync(dto.Id, dto, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return NoContent();
        }

        [HttpDelete("{consultaId}")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> Delete(int consultaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _consultaService.DeleteAsync(consultaId, clinicaId);

            if (!result.Succeeded)
            {
                // O serviço retorna a mensagem de erro apropriada (ex: "Não encontrado" ou "Avaliação já preenchida")
                return BadRequest(result);
            }

            // Retorno padrão para um DELETE bem-sucedido.
            return NoContent();
        }
    }
}