using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Dtos.Consulta;
using backend.Models;
using backend.Data;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica/consultas")]
    public class ConsultaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsultaController(AppDbContext context)
        {
            _context = context;
        }

        // Cria uma nova consulta
        [HttpPost("{clinicaId}/nova-consulta")]
        [Authorize(Roles = "Admin,Secretario")]
        public async Task<IActionResult> CreateConsulta(int clinicaId, [FromBody] ConsultaDto consultaDto)
        {
            var consulta = new Consulta
            {
                ClinicaId = clinicaId, 
                UsuarioId = consultaDto.UsuarioId,
                TutorId = consultaDto.TutorId,
                Date = consultaDto.Date
            };

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Consulta agendada com sucesso.", ConsultaId = consulta.Id });
        }

        // Obtém todas as consultas por clinicaId
        [HttpGet("consulta-clinica")]
        [Authorize(Roles = "Veterinario,Secretario")]
        public async Task<IActionResult> GetAllConsultas([FromQuery] int clinicaId)
        {
            var consultas = await _context.Consultas
                .Where(c => c.ClinicaId == clinicaId)
                .ToListAsync();

            if (consultas == null || !consultas.Any())
            {
                return NotFound("Nenhuma consulta encontrada para a clínica.");
            }

            return Ok(consultas);
        }

        // Obtém uma consulta por ID
        [HttpGet]
        [Authorize(Roles = "Secretario")]
        public async Task<IActionResult> GetConsultaById([FromQuery] int consultaId)
        {
            var consulta = await _context.Consultas.FindAsync(consultaId);
            if (consulta == null)
            {
                return NotFound("Consulta não encontrada.");
            }
            return Ok(consulta);
        }

        // Atualiza uma consulta
        [HttpPut("{consultaId}/editar")]
        [Authorize(Roles = "Secretario")]
        public async Task<IActionResult> UpdateConsulta(int consultaId, [FromBody] ConsultaDto consultaDto)
        {
            var consulta = await _context.Consultas.FindAsync(consultaId);
            if (consulta == null)
            {
                return NotFound("Consulta não encontrada.");
            }

            // Atualiza os campos da consulta
            consulta.UsuarioId = consultaDto.UsuarioId;
            consulta.TutorId = consultaDto.TutorId;
            consulta.Date = consultaDto.Date;

            _context.Consultas.Update(consulta);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Consulta atualizada com sucesso." });
        }

        // Deleta uma consulta
        [HttpDelete("{consultaId}")]
        [Authorize(Roles = "Secretario")]
        public async Task<IActionResult> DeleteConsulta(int consultaId)
        {
            var consulta = await _context.Consultas.FindAsync(consultaId);
            if (consulta == null)
            {
                return NotFound("Consulta não encontrada.");
            }

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Consulta deletada com sucesso." });
        }
    }
}