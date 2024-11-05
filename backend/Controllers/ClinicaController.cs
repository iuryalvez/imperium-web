using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using backend.Data;
using backend.Models;

using backend.Dtos.Clinica;
using backend.Dtos.UsuarioPerfilClinica;
using backend.Dtos.Auth;
using backend.Dtos.Animal;
using backend.Dtos.Consulta;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class ClinicaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicaController(AppDbContext context)
        {
            _context = context;
        }

        // atualizar clinica
        [HttpPut("{clinicaId}/dados")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> UpdateClinica(int clinicaId, [FromBody] UpdateClinicaDto updateClinicaDto)
        {
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
    }
}