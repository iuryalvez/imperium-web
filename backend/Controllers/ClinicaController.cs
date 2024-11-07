using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Data;
using backend.Dtos.Clinica;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class ClinicaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public ClinicaController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // atualizar clinica
        [HttpPut("{clinicaId}/dados")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> UpdateClinica(int clinicaId, [FromBody] UpdateClinicaDto updateClinicaDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
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
    }
}