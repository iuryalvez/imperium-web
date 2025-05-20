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
        private readonly ILogger<TutorController> _logger;
        private readonly IClinicaService _clinicaService;

        public TutorController(AppDbContext context,
                               ILogger<TutorController> logger,
                               IClinicaService clinicaService)
        {
            _context = context;
            _logger = logger;
            _clinicaService = clinicaService;
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
                    _logger.LogWarning("Usuário sem permissão tentou acessar tutores de clínica. Claims: {@Claims}", User.Claims.Select(c => new { c.Type, c.Value }));
                    return BadRequest("Acesso não permitido a esta clínica.");
                }

                _logger.LogInformation("Consultando tutores da clínica {ClinicaId}", clinicaId);

                var vinculosTutores = await _clinicaService.ObterVinculosTutoresDaClinicaAsync(clinicaId);

                var getTutoresClinicaDtos = vinculosTutores
                    .Where(tc => tc.Tutor != null)
                    .Select(tc => new GetTutoresClinicaDto
                    {
                        Id = tc.Tutor.Id,
                        Nome = tc.Tutor.Nome,
                        Cpf = tc.Tutor.Cpf,
                        Endereco = tc.Tutor.Endereco,
                        EndNumero = tc.Tutor.EndNumero,
                        EndEstado = tc.Tutor.EndEstado,
                        EndCidade = tc.Tutor.EndCidade,
                        Cep = tc.Tutor.Cep,
                        Telefone1 = tc.Tutor.Telefone1,
                        Telefone2 = tc.Tutor.Telefone2,
                        Whatsapp = tc.Tutor.Whatsapp,
                        Email = tc.Tutor.Email,
                        DataCad = tc.Tutor.DataCad == default ? DateTime.UtcNow : tc.Tutor.DataCad
                    }).ToList();

                _logger.LogInformation("Retornados {Qtd} tutores da clínica {ClinicaId}", getTutoresClinicaDtos.Count, clinicaId);

                return Ok(getTutoresClinicaDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tutores da clínica.");
                return StatusCode(500, "Erro interno ao buscar os tutores da clínica.");
            }
        }

        // get tutor by tutorId
        // [HttpGet("tutores/{tutorId}")]
        // [Authorize(Policy = "GerenciaTutores")]
        // public async Task<ActionResult<GetTutoresClinicaDto>> GetTutorById(int tutorId)
        // {
        //     var tutor = await _clinicaService.ObterTutorAsync(tutorId);

        //     if (tutor == null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(tutor);
        // }

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
                    Telefone2 = dto.Telefone2?.Trim(),
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
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido: {@Errors}", ModelState);
                return BadRequest(ModelState);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Iniciando edição de tutor ID: {TutorId}", dto.Id);

                var clinicaId = User.GetClaimValue<int>("clinicaId");
                if (clinicaId <= 0)
                {
                    _logger.LogWarning("Claim clinicaId ausente/inválida");
                    return Unauthorized("Acesso restrito à clínica.");
                }

                // Valida associação tutor x clínica
                _logger.LogInformation("Validando vínculo tutor {TutorId} com clínica {ClinicaId}", dto.Id, clinicaId);
                var vinculosTutores = await _clinicaService.ObterVinculosTutoresDaClinicaAsync(clinicaId);

                var tutorClinica = vinculosTutores.FirstOrDefault(tc => tc.TutorId == dto.Id);

                if (tutorClinica == null)
                {
                    _logger.LogWarning("Relação TutorClinica não encontrada para tutor de Id: {dto.Id} na clínica de Id: {clinicaId}", dto.Id, clinicaId);
                    return NotFound("Tutor não encontrado nesta clíncia.");
                }

                var tutor = tutorClinica.Tutor;

                // Verifica se o CPF foi alterado e se já existe outro tutor com o novo CPF
                if (!string.Equals(tutor.Cpf, dto.Cpf.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Verificando duplicidade de CPF: {Cpf}", dto.Cpf);
                    if (await _clinicaService.TutorCpfExisteNaClinicaAsync(dto.Cpf, clinicaId))
                    {
                        _logger.LogWarning("CPF já em uso: {Cpf}", dto.Cpf);
                        return Conflict("Já existe um tutor com este CPF nesta clínica.");
                    }
                }

                // Atualiza campos
                tutor.Nome = dto.Nome.Trim();
                tutor.Cpf = dto.Cpf.Trim();
                tutor.Endereco = dto.Endereco?.Trim();
                tutor.EndNumero = dto.EndNumero;
                tutor.EndEstado = dto.EndEstado?.Trim();
                tutor.EndCidade = dto.EndCidade?.Trim();
                tutor.Cep = dto.Cep?.Trim();
                tutor.Telefone1 = dto.Telefone1?.Trim();
                tutor.Telefone2 = dto.Telefone2?.Trim();
                tutor.Whatsapp = dto.Whatsapp?.Trim();
                tutor.Email = dto.Email.Trim();

                _context.Tutores.Update(tutor);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Tutor atualizado com sucesso: {TutorId}", tutor.Id);

                return Ok(new { tutor.Id });
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                _logger.LogError(dbEx, "Erro de banco ao editar tutor");
                return StatusCode(500, "Erro ao atualizar os dados do tutor.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro inesperado ao editar tutor");
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpDelete("tutores/{tutorId}")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> DeleteTutor(int tutorId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0)
            {
                _logger.LogWarning("Claim clinicaId inválida ou ausente.");
                return BadRequest("Acesso não permitido à clínica.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Iniciando exclusão do tutor {TutorId} da clínica {ClinicaId}", tutorId, clinicaId);

                var tutorClinica = await _context.TutoresClinicas
                    .FirstOrDefaultAsync(tc => tc.TutorId == tutorId && tc.ClinicaId == clinicaId);

                if (tutorClinica == null)
                {
                    _logger.LogWarning("Relação TutorClinica não encontrada para tutor {TutorId} na clínica {ClinicaId}", tutorId, clinicaId);
                    return NotFound("Tutor não encontrado nesta clínica.");
                }

                // Remove animais vinculados ao tutor e à clínica
                var animaisClinicas = await _context.AnimaisClinicas
                    .Include(ac => ac.Animal)
                    .Where(ac => ac.ClinicaId == clinicaId && ac.Animal.TutorId == tutorId)
                    .ToListAsync();

                if (animaisClinicas.Any())
                {
                    _logger.LogInformation("Removendo {Qtd} animal(is) do tutor {TutorId} na clínica {ClinicaId}", animaisClinicas.Count, tutorId, clinicaId);
                    _context.AnimaisClinicas.RemoveRange(animaisClinicas);
                }

                // Remove vínculo com a clínica
                _logger.LogInformation("Removendo vínculo do tutor {TutorId} com clínica {ClinicaId}", tutorId, clinicaId);
                _context.TutoresClinicas.Remove(tutorClinica);

                await _context.SaveChangesAsync();

                // Verifica se o tutor ainda possui vínculo com outra clínica
                bool possuiOutrosVinculos = await _context.TutoresClinicas
                    .AnyAsync(tc => tc.TutorId == tutorId);

                if (!possuiOutrosVinculos)
                {
                    var tutor = await _context.Tutores.FindAsync(tutorId);
                    if (tutor != null)
                    {
                        _logger.LogInformation("Removendo tutor {TutorId}, sem vínculos restantes com clínicas", tutorId);
                        _context.Tutores.Remove(tutor);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Exclusão concluída para tutor {TutorId} na clínica {ClinicaId}", tutorId, clinicaId);

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                _logger.LogError(dbEx, "Erro de banco ao deletar tutor {TutorId} da clínica {ClinicaId}", tutorId, clinicaId);
                return StatusCode(500, "Erro ao deletar dados do tutor.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro inesperado ao deletar tutor {TutorId}", tutorId);
                return StatusCode(500, "Erro interno no servidor.");
            }
        }
    }
}