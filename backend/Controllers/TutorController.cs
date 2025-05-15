using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Tutor;
using backend.Interfaces;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class TutorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public TutorController(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // [HttpGet("tutores")]
        // [Authorize(Roles = "ADMIN,SECRETARIO")]
        // public async Task<IActionResult> GetTutoresByClinica()
        // {
        //     var clinicaId = User.GetClaimValue<int>("clinicaId");
        //     if (clinicaId == 0) {
        //         return BadRequest("Acesso não permitido a esta clínica.");
        //     }

        //     var tutores = await _context.TutoresAnimaisClinicas
        //         .Where(tac => tac.ClinicaId == clinicaId)
        //         .Select(tac => tac.Tutor)
        //         .ToListAsync();

        //     if (tutores == null || !tutores.Any())
        //     {
        //         return NotFound("Nenhuma consulta encontrada para a clínica.");
        //     }

        //     return Ok(tutores);
        // }

        [HttpGet("tutor_{tutorId}")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> GetTutorById(int tutorId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }
            
            var tutor = await _context.Tutores.FindAsync(tutorId);

            if (tutor == null)
            {
                return NotFound();
            }

            return Ok(tutor);
        }

        [HttpPost("tutores/cadastrar")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> CreateTutor([FromBody] CadastrarTutorClinicaDto CadastrarTutorClinicaDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var tutor = new Tutor
            {
                Nome = CadastrarTutorClinicaDto.Nome,
                Cpf = CadastrarTutorClinicaDto.Cpf,
                Endereco = CadastrarTutorClinicaDto.Endereco,
                Cep = CadastrarTutorClinicaDto.Cep,
                Whatsapp = CadastrarTutorClinicaDto.Whatsapp,
                Email = CadastrarTutorClinicaDto.Email,
                DataCad = CadastrarTutorClinicaDto.DataCad
            };

            _context.Tutores.Add(tutor);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Tutor cadastrado com sucesso.", tutor.Id });
        }

        [HttpPut("tutores/{tutorId}")]
        [Authorize(Roles = "GerenciaSistema")]
        public async Task<IActionResult> UpdateTutor(int tutorId, [FromBody] UpdateTutorDto updateTutorDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
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

        [HttpDelete("tutores/tutor_{tutorId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteTutor(int tutorId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
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