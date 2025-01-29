using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Racao;
using backend.Interfaces;

namespace backend.Controllers {
    [ApiController]
    [Route("api/clinica")]
    public class RacaoController : ControllerBase {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        public RacaoController(AppDbContext context, IAuthService authService) {
            _context = context;
            _authService = authService;
        }

        [HttpPost("racoes/nova-racao")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> CreateRacao([FromBody] CreateRacaoDto racaoDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var racao = new Racao
            {
                ClinicaId = clinicaId,
                Nome = racaoDto.Nome,
                Umidade = racaoDto.Umidade,
                Pb = racaoDto.Pb,
                Ee = racaoDto.Ee,
                Mm = racaoDto.Mm,
                Fb = racaoDto.Fb,
                Ms = 10 // isso é calculado ou determinado
            };

            _context.Racoes.Add(racao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRacaoById), new { clinicaId, racaoId = racao.Id }, racaoDto);
        }

        [HttpGet("racoes")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAllRacoes()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var racoes = await _context.Racoes
                .Where(r => r.ClinicaId == clinicaId)
                .Select(r => new GetRacoesDto
                {
                    Id = r.Id,
                    Nome = r.Nome
                })
                .ToListAsync();

            return Ok(racoes);
        }

        [HttpGet("racoes/{racaoId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetRacaoById(int racaoId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var racao = await _context.Racoes
                .Where(r => r.ClinicaId == clinicaId && r.Id == racaoId)
                .Select(r => new UpdateRacaoDto
                {
                    Nome = r.Nome,
                    Umidade = r.Umidade,
                    Pb = r.Pb,
                    Ee = r.Ee,
                    Mm = r.Mm,
                    Fb = r.Fb,
                    Ms = 10 // calculado ou determinado
                })
                .FirstOrDefaultAsync();

            if (racao == null)
            {
                return NotFound("Ração não encontrada.");
            }

            return Ok(racao);
        }

        [HttpPut("racoes/{racaoId}/editar")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> UpdateRacao(int racaoId, [FromBody] UpdateRacaoDto racaoDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var racao = await _context.Racoes.FirstOrDefaultAsync(r => r.Id == racaoId && r.ClinicaId == clinicaId);

            if (racao == null)
            {
                return NotFound("Ração não encontrada.");
            }

            racao.Nome = racaoDto.Nome;
            racao.Umidade = racaoDto.Umidade;
            racao.Pb = racaoDto.Pb;
            racao.Ee = racaoDto.Ee;
            racao.Mm = racaoDto.Mm;
            racao.Fb = racaoDto.Fb;
            racao.Ms = 10; // calculado ou determinado

            _context.Racoes.Update(racao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("racoes/{racaoId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> DeleteRacao(int racaoId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var racao = await _context.Racoes.FirstOrDefaultAsync(r => r.Id == racaoId && r.ClinicaId == clinicaId);

            if (racao == null)
            {
                return NotFound("Ração não encontrada.");
            }

            _context.Racoes.Remove(racao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}