using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Infra.Data;

using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.GestaoClinica
{
    [ApiController]
    [Route("api/clinica")]
    public class ClinicaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClinicaController> _logger;
        public ClinicaController(AppDbContext context,
                                 ILogger<ClinicaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> EditarClinica([FromBody] EditarClinicaDto editarClinicaDto)
        {
            try
            {
                // Validação do DTO
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!User.HasClaim(c => c.Type == "clinicaId"))
                {
                    return BadRequest("Token não contém informação da clínica.");
                }

                var clinicaId = User.GetClaimValue<int?>("clinicaId");
                if (!clinicaId.HasValue || clinicaId == 0) // simular um comportamento de null 
                {
                    return BadRequest("Acesso não permitido a esta clínica.");
                }

                var clinica = await _context.Clinicas.FindAsync(clinicaId);
                if (clinica == null)
                {
                    return NotFound("Clínica não encontrada.");
                }

                // Atualização apenas dos campos permitidos
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
                clinica.Instagram = editarClinicaDto.Instagram;

                // TODO: Validação adicional do CNPJ - 
                // if (!ValidarCnpj(clinica.Cnpj))
                // {
                //     return BadRequest("CNPJ inválido");
                // }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Clínica atualizada com sucesso.",
                    Data = clinica
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Conflito de concorrência ao editar clínica {editarClinicaDto.NomeClinica}");
                return StatusCode(409, "Os dados foram modificados por outro usuário. Recarregue e tente novamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao editar clínica {editarClinicaDto.NomeClinica}");
                return StatusCode(500, "Erro interno ao editar clínica.");
            }
        }
    }
}