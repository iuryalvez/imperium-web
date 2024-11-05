using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Dtos.Tutor;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/tutores")]
    public class TutorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TutorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TutorDto>>> GetTutors()
        {
            var tutors = await _context.Tutores
                .Select(t => new TutorDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Cpf = t.Cpf,
                    Endereco = t.Endereco,
                    Cep = t.Cep,
                    Whatsapp = t.Whatsapp,
                    Email = t.Email,
                    DataCad = t.DataCad
                })
                .ToListAsync();

            return Ok(tutors);
        }

        [HttpGet("tutor-{id}")]
        public async Task<ActionResult<TutorDto>> GetTutor(int id)
        {
            var tutor = await _context.Tutores
                .Select(t => new TutorDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Cpf = t.Cpf,
                    Endereco = t.Endereco,
                    Cep = t.Cep,
                    Whatsapp = t.Whatsapp,
                    Email = t.Email,
                    DataCad = t.DataCad
                })
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tutor == null)
            {
                return NotFound();
            }

            return Ok(tutor);
        }

        [HttpPost("cria-tutor")]
        [Authorize(Roles = "Admin,Secretario")]
        public async Task<ActionResult<TutorDto>> CreateTutor(TutorDto tutorDto)
        {
            var tutor = new Tutor
            {
                Nome = tutorDto.Nome,
                Cpf = tutorDto.Cpf,
                Endereco = tutorDto.Endereco,
                Cep = tutorDto.Cep,
                Whatsapp = tutorDto.Whatsapp,
                Email = tutorDto.Email,
                DataCad = tutorDto.DataCad
            };

            _context.Tutores.Add(tutor);
            await _context.SaveChangesAsync();

            tutorDto.Id = tutor.Id;

            return CreatedAtAction(nameof(GetTutor), new { id = tutorDto.Id }, tutorDto);
        }

        [HttpPut("{id}/editar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTutor(int id, TutorDto tutorDto)
        {
            var existingTutor = await _context.Tutores.FindAsync(id);
            if (existingTutor == null)
            {
                return NotFound();
            }

            existingTutor.Nome = tutorDto.Nome;
            existingTutor.Cpf = tutorDto.Cpf;
            existingTutor.Endereco = tutorDto.Endereco;
            existingTutor.Cep = tutorDto.Cep;
            existingTutor.Whatsapp = tutorDto.Whatsapp;
            existingTutor.Email = tutorDto.Email;
            existingTutor.DataCad = tutorDto.DataCad;

            _context.Entry(existingTutor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("apaga-tutor/{id}")]
        [Authorize(Roles = "Admin,Secretario")]
        public async Task<IActionResult> DeleteTutor(int id)
        {
            var tutor = await _context.Tutores.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }

            _context.Tutores.Remove(tutor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}