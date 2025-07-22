using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Infra.Data;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.GestaoClinica.Interface;
using backend.Domain.AtendimentoAnimal.Interface;

// TODO: Refatorar e usar lógica de Interface + Service (impl) + Repository se houver tempo

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica")]
    public class AnimalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnimalController> _logger;
        private readonly IClinicaService _clinicaInterface;
        private readonly IAnimalService _animalService;
        public AnimalController(AppDbContext context,
                                ILogger<AnimalController> logger,
                                IClinicaService clinicaInterface,
                                IAnimalService animalService)
        {
            _context = context;
            _logger = logger;
            _clinicaInterface = clinicaInterface;
            _animalService = animalService;
        }

        [HttpGet("animais")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAnimaisByClinicaId()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) return BadRequest("ID da clínica não encontrado.");

            var animais = await _animalService.GetAllByClinicIdAsync(clinicaId);
            return Ok(animais);
        }

        [HttpGet("animais/{animalId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAnimalDetails(int animalId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var animal = await _animalService.GetDetailsByIdAsync(animalId, clinicaId);

            if (animal == null) return NotFound($"Animal com ID {animalId} não encontrado na clínica.");

            return Ok(animal);
        }

        [HttpPost("animais/cadastrar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> CadastrarAnimalNaClinica([FromBody] CadastrarAnimalClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return BadRequest("Claim 'clinicaId' não encontrada ou inválida.");

            var result = await _animalService.CadastrarAnimalAsync(dto, clinicaId);

            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("animais/editar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> EditarAnimalNaClinica([FromBody] EditarAnimalClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0)
                return BadRequest(new { Message = "Claim 'clinicaId' não encontrada ou inválida." });

            var result = await _animalService.UpdateAnimalAsync(dto, clinicaId);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        // Deleta um animal em uma clínica
        [HttpDelete("animais/{animalId}")] // Rota ajustada para clareza
        [Authorize(Policy = "GerenciaAnimais")] // Ou uma policy mais específica como "DesvinculaAnimalClinica"
        public async Task<IActionResult> DesvincularAnimalDaClinica(int animalId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0)
            {
                _logger.LogWarning("Usuário tentou desvincular animal sem uma claim 'clinicaId' válida. Claims: {@Claims}", User.Claims.Select(c => new { c.Type, c.Value }));
                return BadRequest(new { Message = "Acesso não permitido. ID da clínica não encontrado." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Encontrar e remover o vínculo AnimalClinica
                var vinculoAnimalClinica = await _context.AnimaisClinicas
                    .FirstOrDefaultAsync(ac => ac.AnimalId == animalId && ac.ClinicaId == clinicaId);

                if (vinculoAnimalClinica == null)
                {
                    await transaction.RollbackAsync(); // Não é estritamente necessário rollback aqui, pois nada foi feito
                    _logger.LogWarning("Tentativa de desvincular animal (ID: {AnimalId}) não associado à clínica (ID: {ClinicaId})", animalId, clinicaId);
                    return NotFound(new { Message = $"Animal com ID {animalId} não encontrado ou não está associado a esta clínica." });
                }

                _context.AnimaisClinicas.Remove(vinculoAnimalClinica);
                _logger.LogInformation("Vínculo AnimalClinica removido para Animal ID {AnimalId} e Clínica ID {ClinicaId}", animalId, clinicaId);

                // 2. Identificar tutores associados a ESTA clínica
                var idsTutoresDaClinica = await _context.TutoresClinicas
                    .Where(tc => tc.ClinicaId == clinicaId)
                    .Select(tc => tc.TutorId)
                    .Distinct()
                    .ToListAsync();

                if (idsTutoresDaClinica.Any())
                {
                    // 3. Encontrar e remover vínculos TutorAnimal para os tutores desta clínica e este animal
                    var vinculosTutorAnimalParaRemover = await _context.TutoresAnimais
                        .Where(ta => ta.AnimalId == animalId && idsTutoresDaClinica.Contains(ta.TutorId))
                        .ToListAsync();

                    if (vinculosTutorAnimalParaRemover.Any())
                    {
                        _context.TutoresAnimais.RemoveRange(vinculosTutorAnimalParaRemover);
                        _logger.LogInformation("{Count} vínculos TutorAnimal removidos para Animal ID {AnimalId} com tutores da Clínica ID {ClinicaId}", vinculosTutorAnimalParaRemover.Count, animalId, clinicaId);
                    }
                    else
                    {
                        _logger.LogInformation("Nenhum vínculo TutorAnimal encontrado para Animal ID {AnimalId} com tutores da Clínica ID {ClinicaId}", animalId, clinicaId);
                    }
                }
                else
                {
                    _logger.LogInformation("Nenhum tutor associado à Clínica ID {ClinicaId}. Nenhum vínculo TutorAnimal para verificar.", clinicaId);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = $"Animal com ID {animalId} desvinculado da clínica e seus tutores associados a esta clínica." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao desvincular o animal (ID: {AnimalId}) da clínica (ID: {ClinicaId}).", animalId, clinicaId);
                return StatusCode(500, new { Message = "Ocorreu um erro interno ao processar a solicitação." });
            }
        }
    }
}
