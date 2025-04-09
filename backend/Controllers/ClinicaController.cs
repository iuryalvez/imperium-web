using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Data;
using backend.Dtos.Clinica;
using backend.Interfaces;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class ClinicaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public ClinicaController(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // atualizar clinica
        [HttpPut("dados")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> UpdateClinica([FromBody] UpdateClinicaDto updateClinicaDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var clinica = await _context.Clinicas.FindAsync(clinicaId);
            if (clinica == null)
            {
                return NotFound();
            }

            clinica.NomeClinica = updateClinicaDto.NomeClinica;
            clinica.NomeFantasia = updateClinicaDto.NomeFantasia;
            clinica.Cnpj = updateClinicaDto.Cnpj;
            clinica.Endereco = updateClinicaDto.Endereco;
            clinica.EndNumero = updateClinicaDto.EndNumero;
            clinica.EndEstado = updateClinicaDto.EndEstado;
            clinica.EndCidade = updateClinicaDto.EndCidade;
            clinica.Cep = updateClinicaDto.Cep;
            clinica.Telefone1 = updateClinicaDto.Telefone1;
            clinica.Whatsapp = updateClinicaDto.Whatsapp;
            clinica.Email = updateClinicaDto.Email;

            _context.Entry(clinica).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarClinicasPorNome([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("O nome da clínica não pode ser vazio.");

            var clinicas = await _context.Clinicas
                .Where(c => EF.Functions.ILike(c.NomeClinica, $"%{nome}%")) // ILike é case-insensitive (PostgreSQL)
                .Select(c => new
                {
                    value = c.Id,
                    label = c.NomeClinica
                })
                .ToListAsync();

            return Ok(clinicas);
        }
    }
}