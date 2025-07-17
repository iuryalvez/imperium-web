using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Infra.Data;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.GestaoClinica.Interface;

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica")]
    public class AnimalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnimalController> _logger;
        private readonly ClinicaInterface _clinicaInterface;
        public AnimalController(AppDbContext context,
                                ILogger<AnimalController> logger,
                                ClinicaInterface clinicaInterface)
        {
            _context = context;
            _logger = logger;
            _clinicaInterface = clinicaInterface;
        }

        [HttpGet("animais")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAnimaisByClinicaId()
        {
            try
            {
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                if (clinicaId == 0)
                {
                    _logger.LogWarning("Usuário tentou acessar animais sem uma claim 'clinicaId' válida. Claims: {@Claims}", User.Claims.Select(c => new { c.Type, c.Value }));
                    return BadRequest("Acesso não permitido. ID da clínica não encontrado.");
                }

                _logger.LogInformation("Consultando animais da clínica {ClinicaId}", clinicaId);

                // Usando o método que inclui os tutores
                var vinculosAnimais = await _clinicaInterface.ObterVinculosAnimaisDaClinicaComTutoresAsync(clinicaId);

                if (vinculosAnimais == null || !vinculosAnimais.Any())
                {
                    _logger.LogInformation("Nenhum animal encontrado para a clínica {ClinicaId}", clinicaId);
                    return Ok(new List<GetAnimaisClinicaDto>()); // Retorna lista vazia se nenhum animal for encontrado
                }

                var getAnimaisClinicaDtos = vinculosAnimais
                    .Select(ac =>
                    {
                        var tutoresReais = ac.Animal.TutoresAnimais? // A coleção TutoresAnimais não deve ser nula se incluída corretamente, mas o '?' é uma segurança.
                            .Where(ta => ta.Tutor != null) // Garante que o objeto Tutor dentro do vínculo existe
                            .Select(ta => new TutorInfoSimplesDto { Id = ta.TutorId, Nome = ta.Tutor.Nome })
                            .ToList();

                        List<TutorInfoSimplesDto> tutoresParaDto;

                        if (tutoresReais == null || !tutoresReais.Any()) // Se a lista for nula ou vazia
                        {
                            tutoresParaDto = new List<TutorInfoSimplesDto>
                            {
                                new TutorInfoSimplesDto { Id = 0, Nome = "N/A" } // Ou "N/A", como preferir
                            };
                        }
                        else
                        {
                            tutoresParaDto = tutoresReais;
                        }

                        // 3. Retorna o DTO do animal preenchido
                        return new GetAnimaisClinicaDto
                        {
                            Id = ac.Animal!.Id, // O '!' assume que ac.Animal não é nulo aqui (garantido pelo filtro no serviço)
                            Nome = ac.Animal.Nome,
                            Especie = ac.Animal.Especie,
                            Nascimento = ac.Animal.Nascimento,
                            Sexo = ac.Animal.Sexo,
                            Castrado = ac.Animal.Castrado,
                            Peso = ac.Animal.Peso,
                            CondicaoCorporal = ac.Animal.CondicaoCorporal,
                            GrauAtividade = ac.Animal.GrauAtividade,
                            Gestacao = ac.Animal.IsGestante,
                            Lactacao = ac.Animal.IsLactanteNumeroFilhotes,
                            SemanaLactacao = ac.Animal.SemanaLactacao,
                            SemanaGestacao = ac.Animal.SemanaGestacao,
                            DataCad = ac.Animal.DataCad,
                            // Tutores = tutoresParaDto // Usa a lista definida acima
                        };
                    }).ToList();

                _logger.LogInformation("Retornados {Qtd} animais da clínica {ClinicaId}", getAnimaisClinicaDtos.Count, clinicaId);

                return Ok(getAnimaisClinicaDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar animais da clínica {ClinicaId}", User.GetClaimValue<int>("clinicaId"));
                return StatusCode(500, "Erro interno ao buscar os animais da clínica.");
            }
        }

        // Obtém um animal por ID
        [HttpGet("animais/{animalId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAnimalById(int animalId)
        {
            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }

            return Ok(animal);
        }

        [HttpGet("animalClinica/{id}")]
        [Authorize(Policy = "VisualizaTudo")] // Ou outra policy apropriada
        public async Task<IActionResult> GetAnimalDaClinicaById(int id, [FromQuery] int clinicaId) // Assumindo que o animal é sempre visualizado no contexto de uma clínica
        {
            if (clinicaId > 0)
            {
                var userClinicaId = User.GetClaimValue<int>("clinicaId");
                if (userClinicaId == 0)
                {
                    return BadRequest(new { Message = "ID da clínica é obrigatório para administradores ou não foi encontrado na claim." });
                }
                clinicaId = userClinicaId;
            }

            var animalNaClinica = await _context.AnimaisClinicas
                .Include(ac => ac.Animal!)
                    .ThenInclude(a => a.TutoresAnimais!)
                        .ThenInclude(ta => ta.Tutor)
                // .Include(ac => ac.Animal)
                // .ThenInclude(a => a!.AcompanhamentoInd) // dados de acompanhamento
                .Where(ac => ac.AnimalId == id && ac.ClinicaId == clinicaId)
                .Select(ac => ac.Animal) // Selecionar apenas o animal
                .FirstOrDefaultAsync();

            if (animalNaClinica == null)
            {
                return NotFound(new { Message = $"Animal com ID {id} não encontrado na clínica ID {clinicaId}." });
            }

            // Mapear para um DTO de retorno se necessário para não expor a entidade diretamente
            // var animalDto = _mapper.Map<AnimalDetalhesDto>(animalNaClinica);
            return Ok(animalNaClinica);
        }

        [HttpPost("animais/cadastrar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> CadastrarAnimalNaClinica([FromBody] CadastrarAnimalClinicaDto cadastrarAnimalClinicaDto)
        {
            // Obter o valor da claim como inteiro
            var clinicaId = User.GetClaimValue<int>("clinicaId");

            if (clinicaId <= 0)
            {
                _logger.LogWarning("Tentativa de cadastro de animal com claim 'clinicaId' inválida ou ausente. Valor: {ClinicaId}", clinicaId);
                return BadRequest(new { Message = "Claim 'clinicaId' não encontrada ou inválida." });
            }

            // Validar se a clínica existe e está ativa
            var clinicaExistente = await _context.Clinicas
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(c => c.Id == clinicaId && c.IsAtiva);
            if (clinicaExistente == null)
            {
                _logger.LogWarning("Tentativa de cadastro de animal em clínica inexistente ou inativa. Clínica ID: {ClinicaId}", clinicaId);
                return BadRequest(new { Message = $"Clínica com ID {clinicaId} não encontrada ou está inativa." });
            }

            // Iniciar uma transação
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Criar o Animal
                var animal = new Animal
                {
                    Nome = cadastrarAnimalClinicaDto.Nome,
                    Especie = cadastrarAnimalClinicaDto.Especie,
                    Nascimento = cadastrarAnimalClinicaDto.Nascimento.ToUniversalTime(), // Armazenar em UTC
                    DataCad = DateTime.UtcNow,
                    DataModificado = DateTime.UtcNow,
                    Sexo = cadastrarAnimalClinicaDto.Sexo,
                    Castrado = cadastrarAnimalClinicaDto.Castrado,
                    Peso = cadastrarAnimalClinicaDto.Peso,
                    CondicaoCorporal = cadastrarAnimalClinicaDto.CondicaoCorporal,
                    GrauAtividade = cadastrarAnimalClinicaDto.GrauAtividade,
                    IsGestante = cadastrarAnimalClinicaDto.Gestacao,
                    IsLactanteNumeroFilhotes = cadastrarAnimalClinicaDto.Lactacao,
                    SemanaLactacao = cadastrarAnimalClinicaDto.SemanaLactacao,
                    SemanaGestacao = cadastrarAnimalClinicaDto.SemanaGestacao
                };

                _context.Animais.Add(animal);
                await _context.SaveChangesAsync(); // Salva para obter o animal.Id
                _logger.LogInformation("Animal (ID provisório antes do commit: {AnimalId}) criado na memória.", animal.Id);


                // 2. Vincular Animal à Clínica
                var vinculoAnimalClinica = new AnimalClinica
                {
                    AnimalId = animal.Id,
                    ClinicaId = clinicaId
                };
                _context.AnimaisClinicas.Add(vinculoAnimalClinica);
                _logger.LogInformation("Vínculo AnimalClinica criado na memória para Animal ID {AnimalId} e Clínica ID {ClinicaId}.", animal.Id, clinicaId);

                // 3. Validar e Vincular Tutores ao Animal (SE FORNECIDOS)
                if (cadastrarAnimalClinicaDto.TutorId != null && cadastrarAnimalClinicaDto.TutorId.Any())
                {
                    _logger.LogInformation("Processando {Count} TutorIDs fornecidos para o animal ID {AnimalId}.", cadastrarAnimalClinicaDto.TutorId.Count, animal.Id);
                    foreach (var tutorId in cadastrarAnimalClinicaDto.TutorId.Distinct()) // Distinct para evitar processamento duplicado
                    {
                        // Verificar se o tutor existe E se ele está vinculado à clínica atual
                        var tutorValidoNaClinica = await _context.TutoresClinicas
                                                            .AnyAsync(tc => tc.TutorId == tutorId && tc.ClinicaId == clinicaId);

                        if (!tutorValidoNaClinica)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogWarning("Tutor com ID {TutorId} não encontrado ou não associado à clínica ID {ClinicaId}. Rollback da transação.", tutorId, clinicaId);
                            return BadRequest(new { Message = $"Tutor com ID {tutorId} não encontrado ou não está associado à clínica ID {clinicaId}." });
                        }

                        var vinculoTutorAnimal = new TutorAnimal
                        {
                            TutorId = tutorId,
                            AnimalId = animal.Id
                        };
                        _context.TutoresAnimais.Add(vinculoTutorAnimal);
                        _logger.LogInformation("Vínculo TutorAnimal criado na memória para Tutor ID {TutorId} e Animal ID {AnimalId}.", tutorId, animal.Id);
                    }
                }
                else
                {
                    _logger.LogInformation("Nenhum TutorId fornecido para o animal ID {AnimalId}. O animal será cadastrado sem tutor.", animal.Id);
                }

                await _context.SaveChangesAsync(); // Salva os vínculos AnimalClinica e TutorAnimal (se houver)

                // Se tudo ocorreu bem, comitar a transação
                await transaction.CommitAsync();

                _logger.LogInformation("Animal '{AnimalNome}' (ID: {AnimalId}) cadastrado e vinculado à clínica {ClinicaId}. Tutores associados (se houver): {TutoresIds}",
                    animal.Nome, animal.Id, clinicaId, cadastrarAnimalClinicaDto.TutorId != null ? string.Join(", ", cadastrarAnimalClinicaDto.TutorId) : "Nenhum");

                // Assumindo que você tem um endpoint GetAnimalDaClinicaById
                // Se não, ajuste para o nome correto do seu endpoint de busca ou use Ok()
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "DbUpdateException ao tentar cadastrar animal. Clínica ID: {ClinicaId}. DTO: {@Dto}", clinicaId, cadastrarAnimalClinicaDto);
                return StatusCode(500, new { Message = "Ocorreu um erro ao salvar os dados do animal. Tente novamente.", Details = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Exception inesperada ao tentar cadastrar animal. Clínica ID: {ClinicaId}. DTO: {@Dto}", clinicaId, cadastrarAnimalClinicaDto);
                return StatusCode(500, new { Message = "Ocorreu um erro inesperado. Tente novamente.", Details = ex.Message });
            }
        }

        [HttpPut("animais/editar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> EditarAnimalNaClinica([FromBody] EditarAnimalClinicaDto editarAnimalClinicaDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0)
            {
                // Log with more context
                // Comentário: Log com mais contexto
                _logger.LogWarning("Attempt to edit animal with invalid clinicaId claim. Value: {ClinicaId}", clinicaId);
                return BadRequest(new { Message = "Claim 'clinicaId' não encontrada ou inválida." });
            }

            // Using a transaction is a good practice for multi-step database operations
            // Comentário: Usar uma transação é uma boa prática para operações de banco de dados com múltiplos passos
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Find the animal within the context of the user's clinic and include its current tutors
                // Passo 1: Encontrar o animal no contexto da clínica do usuário e incluir seus tutores atuais
                var animal = await _context.Animais
                    .Include(a => a.TutoresAnimais)
                    .FirstOrDefaultAsync(a => a.Id == editarAnimalClinicaDto.Id &&
                                             a.AnimaisClinicas!.Any(ac => ac.ClinicaId == clinicaId));

                if (animal == null)
                {
                    await transaction.RollbackAsync();
                    _logger.LogWarning("Animal with ID {AnimalId} not found in clinic {ClinicaId}", editarAnimalClinicaDto.Id, clinicaId);
                    return NotFound(new { Message = $"Animal com ID {editarAnimalClinicaDto.Id} não encontrado nesta clínica." });
                }

                // Step 2: Update the animal's properties
                // Passo 2: Atualizar as propriedades do animal
                animal.Nome = editarAnimalClinicaDto.Nome;
                animal.Especie = editarAnimalClinicaDto.Especie;
                animal.Nascimento = editarAnimalClinicaDto.Nascimento.ToUniversalTime();
                animal.DataModificado = DateTime.UtcNow; // Set by the server
                animal.Sexo = editarAnimalClinicaDto.Sexo;
                animal.Castrado = editarAnimalClinicaDto.Castrado;
                animal.Peso = editarAnimalClinicaDto.Peso;
                animal.CondicaoCorporal = editarAnimalClinicaDto.CondicaoCorporal;
                animal.GrauAtividade = editarAnimalClinicaDto.GrauAtividade;
                animal.IsGestante = editarAnimalClinicaDto.Gestacao;
                animal.IsLactanteNumeroFilhotes = editarAnimalClinicaDto.Lactacao;
                animal.SemanaLactacao = editarAnimalClinicaDto.SemanaLactacao;
                animal.SemanaGestacao = editarAnimalClinicaDto.SemanaGestacao;

                // Step 3: Manage tutor relationships
                // Passo 3: Gerenciar os vínculos com tutores
                var idsTutoresAtuais = animal.TutoresAnimais.Select(ta => ta.TutorId).ToList();

                // This line correctly handles a null DTO list, treating it as an empty list.
                // This is the key to allowing an animal to have no tutors.
                // Comentário: Esta linha trata corretamente uma lista nula do DTO, considerando-a como uma lista vazia.
                // Esta é a chave para permitir que um animal não tenha tutores.
                var idsTutoresDesejados = editarAnimalClinicaDto.TutorId ?? new List<int>();

                // Step 3a: Validate new tutors (if any are provided)
                // Passo 3a: Validar novos tutores (se algum for fornecido)
                foreach (var tutorIdDesejado in idsTutoresDesejados.Distinct())
                {
                    var tutorValidoNaClinica = await _context.TutoresClinicas
                                                        .AnyAsync(tc => tc.TutorId == tutorIdDesejado && tc.ClinicaId == clinicaId);
                    if (!tutorValidoNaClinica)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning("Validation failed: Tutor ID {TutorId} is not valid or does not belong to clinic {ClinicaId}", tutorIdDesejado, clinicaId);
                        return BadRequest(new { Message = $"Tutor com ID {tutorIdDesejado} não é válido ou não pertence à clínica." });
                    }
                }

                // Step 3b: Identify and remove unneeded tutor links
                // Passo 3b: Identificar e remover vínculos de tutores desnecessários
                var tutoresParaRemoverIds = idsTutoresAtuais.Except(idsTutoresDesejados).ToList();
                if (tutoresParaRemoverIds.Any())
                {
                    var vinculosParaRemover = animal.TutoresAnimais
                                                .Where(ta => tutoresParaRemoverIds.Contains(ta.TutorId))
                                                .ToList();
                    _context.TutoresAnimais.RemoveRange(vinculosParaRemover);
                    _logger.LogInformation("Removing {Count} tutor links for animal {AnimalId}. Tutor IDs: {TutorIds}", vinculosParaRemover.Count, animal.Id, string.Join(", ", tutoresParaRemoverIds));
                }

                // Step 3c: Identify and add new tutor links
                // Passo 3c: Identificar e adicionar novos vínculos de tutores
                var tutoresParaAdicionarIds = idsTutoresDesejados.Except(idsTutoresAtuais).ToList();
                if (tutoresParaAdicionarIds.Any())
                {
                    foreach (var tutorId in tutoresParaAdicionarIds)
                    {
                        _context.TutoresAnimais.Add(new TutorAnimal { AnimalId = animal.Id, TutorId = tutorId });
                    }
                    _logger.LogInformation("Adding {Count} tutor links for animal {AnimalId}. Tutor IDs: {TutorIds}", tutoresParaAdicionarIds.Count, animal.Id, string.Join(", ", tutoresParaAdicionarIds));
                }

                // Log the case where there are no changes to tutor links
                // Comentário: Log para o caso em que não há alterações nos vínculos de tutores
                if (!tutoresParaAdicionarIds.Any() && !tutoresParaRemoverIds.Any())
                {
                    _logger.LogInformation("No changes to tutor links for animal {AnimalId}.", animal.Id);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Animal atualizado com sucesso.", AnimalId = animal.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating animal with ID {AnimalId}. DTO: {@Dto}", editarAnimalClinicaDto.Id, editarAnimalClinicaDto);
                return StatusCode(500, new { Message = "Ocorreu um erro interno ao atualizar o animal." });
            }
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
