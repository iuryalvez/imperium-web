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
        private readonly ILogger<ClinicaController> _logger;
        public ClinicaController(AppDbContext context, IAuthService authService, ILogger<ClinicaController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        // atualizar clinica
        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> EditarClinica([FromBody] EditarClinicaDto editarClinicaDto)
        {
            try {
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                if (clinicaId == 0) {
                    return BadRequest("Acesso não permitido a esta clínica.");
                }

                var clinica = await _context.Clinicas.FindAsync(clinicaId);
                if (clinica == null)
                {
                    return NotFound();
                }

                clinica.NomeClinica = editarClinicaDto.NomeClinica;
                clinica.NomeFantasia = editarClinicaDto.NomeFantasia;
                clinica.Cnpj = editarClinicaDto.Cnpj;
                clinica.Endereco = editarClinicaDto.Endereco;
                clinica.EndNumero = editarClinicaDto.EndNumero;
                clinica.EndEstado = editarClinicaDto.EndEstado;
                clinica.EndCidade = editarClinicaDto.EndCidade;
                clinica.Cep = editarClinicaDto.Cep;
                clinica.Telefone1 = editarClinicaDto.Telefone1;
                clinica.Whatsapp = editarClinicaDto.Whatsapp;
                clinica.Email = editarClinicaDto.Email;

                _context.Entry(clinica).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new {Message = "Clínica atualizada com sucesso.", });
            } 
            catch (Exception ex) {
                _logger.LogError(ex, $"Erro ao editar clínica {editarClinicaDto.NomeClinica}");
                return StatusCode(500, "Erro interno ao editar clínica.");
            }
        }
    }
}