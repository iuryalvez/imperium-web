using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Infra.Data;
using backend.Domain.GestaoClinica.Interface;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;

// TODO: Refatorar e usar lógica de Interface + Service (impl) + Repository se houver tempo
// TODO: Ver possibilidade de refatorar a lógica da persistência dos dados dos tutores:
//  -> manter o tutor cadastrado no banco e no cadastro de tutor ver se já existe um cadastro
//     proveniente de outra clínica para aproveitar dados antigos/existentes de outras clínicas

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica")]
    public class TutorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TutorController> _logger;
        private readonly IClinicaService _clinicaService;
        private readonly ITutorService _tutorService;

        public TutorController(AppDbContext context,
                               ILogger<TutorController> logger,
                               IClinicaService clinicaService,
                               ITutorService tutorService)
        {
            _context = context;
            _logger = logger;
            _clinicaService = clinicaService;
            _tutorService = tutorService;
        }

        [HttpGet("tutores")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> GetTutoresClinica()
        {
            try
            {
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                if (clinicaId == 0)
                {
                    return BadRequest("Acesso não permitido a esta clínica.");
                }

                var tutoresDto = await _tutorService.GetAllByClinicIdAsync(clinicaId);

                return Ok(tutoresDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tutores da clínica.");
                return StatusCode(500, "Erro interno ao buscar os tutores da clínica.");
            }
        }

        [HttpPost("tutores/cadastrar")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> CadastrarTutorClinica([FromBody] CadastrarTutorClinicaDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido: {@Errors}", ModelState);
                return BadRequest(ModelState);
            }

            // Inicia transação única
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Iniciando cadastro de tutor na clínica");

                var clinicaId = User.GetClaimValue<int>("clinicaId");
                if (clinicaId <= 0)
                {
                    _logger.LogWarning("Claim clinicaId ausente/inválida");
                    return Unauthorized("Claim clinicaId ausente ou inválida para usuário não administrador.");
                }

                // Verifica clínica
                _logger.LogInformation("Verificando clínica ID: {ClinicaId}", clinicaId);
                var clinica = await _clinicaService.ObterClinicaAsync(clinicaId);
                if (clinica == null)
                {
                    _logger.LogWarning("Clínica não encontrada: {ClinicaId}", clinicaId);
                    return NotFound($"Clínica com ID {clinicaId} não encontrada.");
                }

                // Valida CPF
                _logger.LogInformation("Verificando CPF: {Cpf}", dto.Cpf);
                if (await _clinicaService.TutorCpfExisteNaClinicaAsync(dto.Cpf, clinicaId))
                {
                    _logger.LogWarning("CPF já cadastrado: {Cpf}", dto.Cpf);
                    return Conflict("Já existe um tutor com este CPF associado a esta clínica.");
                }

                // Cria tutor
                _logger.LogInformation("Criando entidade Tutor");
                var tutor = new Tutor
                {
                    Nome = dto.Nome.Trim(),
                    Cpf = dto.Cpf.Trim(),
                    Endereco = dto.Endereco?.Trim(),
                    EndNumero = dto.EndNumero,
                    EndEstado = dto.EndEstado?.Trim(),
                    EndCidade = dto.EndCidade?.Trim(),
                    Cep = dto.Cep?.Trim(),
                    Telefone1 = dto.Telefone1?.Trim(),
                    Whatsapp = dto.Whatsapp?.Trim(),
                    Email = dto.Email.Trim(),
                    DataCad = dto.DataCad == default ? DateTime.UtcNow : dto.DataCad
                };

                _context.Tutores.Add(tutor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tutor criado com ID: {TutorId}", tutor.Id);

                // Associa à clínica
                _logger.LogInformation("Criando associação com clínica");
                var tutorClinica = new TutorClinica
                {
                    TutorId = tutor.Id,
                    ClinicaId = clinicaId,
                    DataAssociacao = DateTime.UtcNow
                };

                _context.TutoresClinicas.Add(tutorClinica);

                if (dto.AnimalId != null && dto.AnimalId.Any())
                {
                    _logger.LogInformation("Processando {Count} AnimalIDs para o novo tutor {TutorId}.", dto.AnimalId.Count, tutor.Id);
                    foreach (var animalId in dto.AnimalId.Distinct())
                    {
                        // Valida se o animal existe e pertence à mesma clínica
                        var animalValido = await _context.AnimaisClinicas
                                                    .AnyAsync(ac => ac.AnimalId == animalId && ac.ClinicaId == clinicaId);

                        if (!animalValido)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogWarning("Animal com ID {AnimalId} não encontrado ou não pertence à clínica ID {ClinicaId}. Rollback.", animalId, clinicaId);
                            return BadRequest(new { Message = $"Animal com ID {animalId} não encontrado ou não pertence a esta clínica." });
                        }

                        var vinculoTutorAnimal = new TutorAnimal
                        {
                            TutorId = tutor.Id,
                            AnimalId = animalId,
                            DataAssociacao = DateTime.UtcNow
                        };
                        _context.TutoresAnimais.Add(vinculoTutorAnimal);
                        _logger.LogInformation("Vínculo TutorAnimal criado para Tutor ID {TutorId} e Animal ID {AnimalId}.", tutor.Id, animalId);
                    }
                }

                await _context.SaveChangesAsync();

                // Commit da transação
                await transaction.CommitAsync();
                _logger.LogInformation("Tutor cadastrado com sucesso! ID: {TutorId}", tutor.Id);

                // return CreatedAtAction(nameof(GetTutoresClinicaDto) , new { id = tutor.Id }, new { tutor.Id });
                return Ok(new { tutor.Id });
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                _logger.LogError(dbEx, "Erro de banco de dados ao cadastrar tutor");
                return StatusCode(500, "Erro ao persistir dados do tutor");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro inesperado ao cadastrar tutor");
                return StatusCode(500, "Erro interno no servidor");
            }
        }

        [HttpPut("tutores/editar")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> EditarTutorClinica([FromBody] EditarTutorClinicaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0)
                return Unauthorized("Acesso restrito à clínica.");

            var result = await _tutorService.UpdateTutorAsync(dto, clinicaId);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("tutores/{tutorId}")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> DeleteTutor(int tutorId)
        {
            // Check for a valid clinic ID from the user's claims.
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0)
            {
                _logger.LogWarning("Claim clinicaId inválida ou ausente.");
                return BadRequest("Acesso não permitido à clínica.");
            }

            // Use a transaction to ensure all database operations succeed or fail together.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Iniciando remoção do vínculo do tutor {TutorId} com a clínica {ClinicaId}", tutorId, clinicaId);

                // Find the specific link between the tutor and the clinic.
                var tutorClinicaParaRemover = await _context.TutoresClinicas
                    .FirstOrDefaultAsync(tc => tc.TutorId == tutorId && tc.ClinicaId == clinicaId);

                if (tutorClinicaParaRemover == null)
                {
                    _logger.LogWarning("Vínculo Tutor-Clínica não encontrado para tutor {TutorId} na clínica {ClinicaId}", tutorId, clinicaId);
                    return NotFound("Tutor não encontrado nesta clínica.");
                }

                // Find all Tutor-Animal links for this tutor, but only for animals registered in the current clinic.
                var tutoresAnimaisParaRemover = await _context.TutoresAnimais
                    .Where(ta => ta.TutorId == tutorId &&
                                 _context.AnimaisClinicas.Any(ac => ac.AnimalId == ta.AnimalId && ac.ClinicaId == clinicaId))
                    .ToListAsync();

                if (tutoresAnimaisParaRemover.Any())
                {
                    _logger.LogInformation("Removendo {Qtd} vínculo(s) tutor-animal na clínica {ClinicaId}", tutoresAnimaisParaRemover.Count, clinicaId);
                    _context.TutoresAnimais.RemoveRange(tutoresAnimaisParaRemover);
                }

                // Stage the removal of the main Tutor-Clinic link.
                _logger.LogInformation("Removendo vínculo do tutor {TutorId} com a clínica {ClinicaId}", tutorId, clinicaId);
                _context.TutoresClinicas.Remove(tutorClinicaParaRemover);

                // Check if the tutor has links to any *other* clinics.
                bool possuiOutrosVinculos = await _context.TutoresClinicas
                    .AnyAsync(tc => tc.TutorId == tutorId && tc.ClinicaId != clinicaId);

                if (!possuiOutrosVinculos)
                {
                    _logger.LogInformation("Tutor {TutorId} não possui vínculos com outras clínicas. Removendo entidade principal.", tutorId);
                    var tutorParaDeletar = await _context.Tutores.FindAsync(tutorId);
                    if (tutorParaDeletar != null)
                    {
                        // Also remove any associated consultations, as they have a required foreign key.
                        var consultasParaRemover = await _context.Consultas
                            .Where(c => c.TutorId == tutorId)
                            .ToListAsync();

                        if (consultasParaRemover.Any())
                        {
                            _logger.LogInformation("Removendo {Qtd} consulta(s) associada(s) ao tutor {TutorId}", consultasParaRemover.Count, tutorId);
                            _context.Consultas.RemoveRange(consultasParaRemover);
                        }

                        _context.Tutores.Remove(tutorParaDeletar);
                    }
                }

                // Execute all staged changes (removals) in a single database command.
                await _context.SaveChangesAsync();

                // Commit the transaction if all operations were successful.
                await transaction.CommitAsync();

                _logger.LogInformation("Operação de exclusão concluída para o tutor {TutorId} na clínica {ClinicaId}", tutorId, clinicaId);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of any error.
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro inesperado ao processar exclusão do tutor {TutorId} na clínica {ClinicaId}", tutorId, clinicaId);
                return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");
            }
        }
    }
}