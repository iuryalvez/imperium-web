using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Infra.Data;
using backend.Interfaces;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica")]
    public class ConsultaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public ConsultaController(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Cria uma nova consulta
        [HttpPost("nova-consulta")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> CreateConsulta([FromBody] ConsultaDto consultaDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var consulta = new Consulta
            {
                ClinicaId = clinicaId, 
                UsuarioId = consultaDto.UsuarioId,
                TutorId = consultaDto.TutorId,
                DataConsulta = consultaDto.DataConsulta
            };

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Consulta agendada com sucesso.", ConsultaId = consulta.Id });
        }

        // Obtém todas as consultas por clinicaId
        [HttpGet("consultas")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAllConsultas()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

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
        [HttpGet("consultas/{consultaId}")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> GetConsultaById(int consultaId)
        {
            var consulta = await _context.Consultas.FindAsync(consultaId);
            
            if (consulta == null)
            {
                return NotFound("Consulta não encontrada.");
            }

            return Ok(consulta);
        }

        // Atualiza uma consulta
        [HttpPut("consultas/{consultaId}/editar")]
        [Authorize(Policy = "GerenciaConsultas")]
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
            consulta.DataConsulta = consultaDto.DataConsulta;

            _context.Consultas.Update(consulta);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Consulta atualizada com sucesso." });
        }

        // Deleta uma consulta
        [HttpDelete("consultas/{consultaId}")]
        [Authorize(Policy = "GerenciaConsultas")]
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

        [HttpGet("consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetRelatorio(int consultaId)
        {
            var relatorio = await _context.Relatorios
                .Where(r => r.ConsultaId == consultaId)
                .Select(r => new RelatorioDto
                {
                    Id = r.Id,
                    ConsultaId = r.ConsultaId,
                    AcompanhamentoIndId = r.AcompanhamentoIndId,
                    Idade = r.Idade,
                    Peso = r.Peso,
                    CondicaoCorporal = r.CondicaoCorporal,
                    GrauAtividade = r.GrauAtividade,
                    Racao = r.Racao,
                    DescDieta = r.DescDieta
                })
                .FirstOrDefaultAsync();

            if (relatorio == null)
            {
                return NotFound("Relatório não encontrado para a consulta especificada.");
            }

            return Ok(relatorio);
        }

        [HttpPost("consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CreateRelatorio(int consultaId, [FromBody] RelatorioDto relatorioDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var relatorio = new Relatorio
            {
                ConsultaId = consultaId,
                AcompanhamentoIndId = relatorioDto.AcompanhamentoIndId,
                Idade = relatorioDto.Idade,
                Peso = relatorioDto.Peso,
                CondicaoCorporal = relatorioDto.CondicaoCorporal,
                GrauAtividade = relatorioDto.GrauAtividade,
                Racao = relatorioDto.Racao,
                DescDieta = relatorioDto.DescDieta
            };

            _context.Relatorios.Add(relatorio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRelatorio), new { clinicaId, consultaId }, relatorioDto);
        }

        [HttpPut("consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> UpdateRelatorio(int consultaId, [FromBody] RelatorioDto relatorioDto)
        {
            var relatorio = await _context.Relatorios.FirstOrDefaultAsync(r => r.ConsultaId == consultaId);
            if (relatorio == null)
            {
                return NotFound("Relatório não encontrado para a consulta especificada.");
            }

            relatorio.AcompanhamentoIndId = relatorioDto.AcompanhamentoIndId;
            relatorio.Idade = relatorioDto.Idade;
            relatorio.Peso = relatorioDto.Peso;
            relatorio.CondicaoCorporal = relatorioDto.CondicaoCorporal;
            relatorio.GrauAtividade = relatorioDto.GrauAtividade;
            relatorio.Racao = relatorioDto.Racao;
            relatorio.DescDieta = relatorioDto.DescDieta;

            _context.Relatorios.Update(relatorio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> DeleteRelatorio(int consultaId)
        {
            var relatorio = await _context.Relatorios.FirstOrDefaultAsync(r => r.ConsultaId == consultaId);
            if (relatorio == null)
            {
                return NotFound("Relatório não encontrado para a consulta especificada.");
            }

            _context.Relatorios.Remove(relatorio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}