using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Consulta;
using backend.Dtos.Relatorio;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class ConsultaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public ConsultaController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Cria uma nova consulta
        [HttpPost("{clinicaId}/nova-consulta")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> CreateConsulta(int clinicaId, [FromBody] ConsultaDto consultaDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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
        [HttpGet("{clinicaId}/consultas")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAllConsultas(int clinicaId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
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
        [HttpGet("{clinicaId}/consultas/{consultaId}")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> GetConsultaById(int clinicaId, int consultaId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var consulta = await _context.Consultas.FindAsync(consultaId);
            
            if (consulta == null)
            {
                return NotFound("Consulta não encontrada.");
            }

            return Ok(consulta);
        }

        // Atualiza uma consulta
        [HttpPut("{clinicaId}/consultas/{consultaId}/editar")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> UpdateConsulta(int clinicaId, int consultaId, [FromBody] ConsultaDto consultaDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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
        [HttpDelete("{clinicaId}/consultas/{consultaId}")]
        [Authorize(Policy = "GerenciaConsultas")]
        public async Task<IActionResult> DeleteConsulta(int clinicaId, int consultaId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var consulta = await _context.Consultas.FindAsync(consultaId);
            if (consulta == null)
            {
                return NotFound("Consulta não encontrada.");
            }

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Consulta deletada com sucesso." });
        }

        [HttpGet("{clinicaId}/consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetRelatorio(int clinicaId, int consultaId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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

        [HttpPost("{clinicaId}/consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CreateRelatorio(int clinicaId, int consultaId, [FromBody] RelatorioDto relatorioDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
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

        [HttpPut("{clinicaId}/consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> UpdateRelatorio(int clinicaId, int consultaId, [FromBody] RelatorioDto relatorioDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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

        [HttpDelete("{clinicaId}/consultas/{consultaId}/relatorio")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> DeleteRelatorio(int clinicaId, int consultaId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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