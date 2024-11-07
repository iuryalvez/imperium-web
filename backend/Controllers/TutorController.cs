using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Tutor;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class TutorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public TutorController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("{clinicaId}/tutores")]
        [Authorize(Roles = "ADMIN,SECRETARIO")]
        public async Task<IActionResult> GetTutoresByClinica(int clinicaId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var tutores = await _context.TutoresAnimaisClinicas
                .Where(tac => tac.ClinicaId == clinicaId)
                .Select(tac => tac.Tutor)
                .ToListAsync();

            if (tutores == null || !tutores.Any())
            {
                return NotFound("Nenhuma consulta encontrada para a clínica.");
            }

            return Ok(tutores);
        }

        [HttpGet("{clinicaId}/tutores/{tutorId}")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> GetTutorById(int clinicaId, int tutorId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }
            
            var tutor = await _context.Tutores.FindAsync(tutorId);

            if (tutor == null)
            {
                return NotFound();
            }

            return Ok(tutor);
        }

        [HttpPost("{clinicaId}/tutores/criar_tutor")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> CreateTutor(int clinicaId, [FromBody] TutorDto tutorDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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

            // return CreatedAtAction(nameof(GetTutorById), new { id = tutorDto.Id }, tutorDto);
            return Ok(new { Message = "Tutor cadastrado com sucesso.", TutorId = tutor.Id });
        }

        [HttpPut("{clinicaId}/tutores/{tutorId}/editar")]
        [Authorize(Roles = "GerenciaSistema")]
        public async Task<IActionResult> UpdateTutor(int clinicaId, int tutorId, [FromBody] UpdateTutorDto updateTutorDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var existingTutor = await _context.Tutores.FindAsync(tutorId);
            if (existingTutor == null)
            {
                return NotFound();
            }

            existingTutor.Nome = updateTutorDto.Nome;
            existingTutor.Cpf = updateTutorDto.Cpf;
            existingTutor.Endereco = updateTutorDto.Endereco;
            existingTutor.Cep = updateTutorDto.Cep;
            existingTutor.Whatsapp = updateTutorDto.Whatsapp;
            existingTutor.Email = updateTutorDto.Email;
            existingTutor.DataCad = updateTutorDto.DataCad;

            _context.Entry(existingTutor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{clinicaId}/tutores/{tutorId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteTutor(int clinicaId, int tutorId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }
            
            var tutor = await _context.Tutores.FindAsync(tutorId);
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