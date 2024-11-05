using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using backend.Data;
using backend.Models;

using backend.Dtos.Clinica;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/sistema")]
    public class SistemaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public SistemaController(AppDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // get all clinicas
        [HttpGet("clinicas")]
        public async Task<ActionResult<IEnumerable<ClinicaDto>>> GetClinicas()
        {
            var clinicas = await _context.Clinicas
                .Select(c => new ClinicaDto
                {
                    Id = c.Id,
                    NomeClinica = c.NomeClinica
                })
                .ToListAsync();

            return Ok(clinicas);
        }

        // get clinica by id
        [HttpGet("clinica/{id}")]
        public async Task<ActionResult<ClinicaDto>> GetClinica(int id)
        {
            var clinica = await _context.Clinicas
                .Where(c => c.Id == id)
                .Select(c => new ClinicaDto
                {
                    Id = c.Id,
                    NomeClinica = c.NomeClinica
                })
                .FirstOrDefaultAsync();

            if (clinica == null)
            {
                return NotFound();
            }

            return Ok(clinica);
        }

        // criar clinica pelo sistema
        [HttpPost("nova-clinica")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClinicaDto>> CreateClinica([FromBody] ClinicaDto clinicaDto)
        {
            var clinica = new Clinica
            {
                NomeClinica = clinicaDto.NomeClinica
            };

            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinica), new { id = clinica.Id }, clinicaDto);
        }

        // deleta clínica
        [HttpDelete("apaga-clinica/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);
            if (clinica == null)
            {
                return NotFound();
            }

            _context.Clinicas.Remove(clinica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Obtém todos os animais cadastrados
        [HttpGet("animais/all")]
        public async Task<IActionResult> GetAllAnimais()
        {
            var animais = await _context.Animais.ToListAsync();
            return Ok(animais);
        }
    }
}