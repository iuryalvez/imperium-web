using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using backend.Data;
using backend.Models;
using backend.Dtos.Clinica;
using backend.Dtos.Animal;
using backend.Dtos.Usuario;

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
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<ClinicaDto>>> GetAllClinicas()
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

        // get clinica by clinicaId
        [HttpGet("clinicas/{clinicaId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ClinicaDto>> GetClinica(int clinicaId)
        {
            var clinica = await _context.Clinicas
                .Where(c => c.Id == clinicaId)
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
        [HttpPost("clinicas/nova-clinica")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ClinicaDto>> CreateClinica([FromBody] ClinicaDto clinicaDto)
        {
            var clinica = new Clinica
            {
                NomeClinica = clinicaDto.NomeClinica
            };

            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinica), new { clinicaId = clinica.Id }, clinicaDto);
        }

        // deleta clínica
        [HttpDelete("clinicas/{clinicaId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteClinica(int clinicaId)
        {
            var clinica = await _context.Clinicas.FindAsync(clinicaId);
            if (clinica == null)
            {
                return NotFound();
            }

            _context.Clinicas.Remove(clinica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Obtém todos os animais cadastrados
        [HttpGet("animais")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<GetAnimaisDto>>> GetAllAnimais()
        {
            var animais = await _context.Animais
                .Select(a => new GetAnimaisDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Especie = a.Especie,
                    Sexo = a.Sexo
                })
                .ToListAsync();

            return Ok(animais);
        }
        
        // Obtém todos os usuários
        [HttpGet("usuarios")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new GetUsuariosDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Nome = u.Nome
                })
                .ToListAsync();

            return Ok(usuarios);
        }
    }
}