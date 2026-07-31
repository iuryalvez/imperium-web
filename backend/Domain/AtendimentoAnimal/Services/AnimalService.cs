using AutoMapper;
using backend.Domain.AtendimentoAnimal;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.Usuarios.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Serviço responsável por gerenciar as regras de negócio relacionadas a animais.
/// </summary>
public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly ITutorService _tutorService;
    private readonly IAcompanhamentoRepository _acompanhamentoRepository;
    private readonly AppDbContext _context; // Usado exclusivamente para controle de transações.
    private readonly IMapper _mapper;
    private readonly ILogger<AnimalService> _logger;

    public AnimalService(
        IAnimalRepository animalRepository,
        ITutorService tutorService,
        IAcompanhamentoRepository acompanhamentoRepository,
        IMapper mapper,
        ILogger<AnimalService> logger,
        AppDbContext context)
    {
        _animalRepository = animalRepository;
        _tutorService = tutorService;
        _acompanhamentoRepository = acompanhamentoRepository;
        _mapper = mapper;
        _logger = logger;
        _context = context;
    }

    #region Métodos de Leitura (Core & DTO)

    /// <summary>
    /// Obtém a entidade Animal pelo seu ID, sem validação de clínica.
    /// ATENÇÃO: Use com cuidado, preferencialmente para operações internas que não requerem validação de acesso.
    /// </summary>
    /// <param name="animalId">ID do animal a ser buscado.</param>
    /// <returns>ServiceResult contendo a entidade Animal ou uma mensagem de erro.</returns>
    public async Task<ServiceResult> ObterPorIdAsync(int animalId)
    {
        var animal = await _animalRepository.GetByIdAsync(animalId);

        if (animal == null)
        {
            _logger.LogWarning("Busca por animal com ID {AnimalId} não encontrou resultados.", animalId);
            return new ServiceResult { Succeeded = false, Message = $"Animal com Id {animalId} não encontrado." };
        }
        return new ServiceResult { Succeeded = true, Data = animal };
    }

    /// <summary>
    /// Método central de busca. Retorna a ENTIDADE Animal se ele pertencer à clínica especificada.
    /// Ideal para uso interno por outros serviços que precisam da entidade completa para manipulação.
    /// </summary>
    /// <param name="animalId">ID do animal.</param>
    /// <param name="clinicaId">ID da clínica para validação de acesso.</param>
    /// <returns>ServiceResult contendo a entidade Animal completa ou uma mensagem de erro.</returns>
    public async Task<ServiceResult> ObterPorIdEClinicaIdAsync(int animalId, int clinicaId)
    {
        _logger.LogDebug("Buscando entidade do animal {AnimalId} para a clínica {ClinicaId}.", animalId, clinicaId);
        var animal = await _animalRepository.GetByIdAndClinicaIdWithIncludesAsync(animalId, clinicaId);
        if (animal == null)
        {
            _logger.LogWarning("Animal {AnimalId} não encontrado ou não pertence à clínica {ClinicaId}.", animalId, clinicaId);
            return new ServiceResult { Succeeded = false, Message = $"Animal com ID {animalId} não encontrado nesta clínica." };
        }
        return new ServiceResult { Succeeded = true, Data = animal };
    }

    /// <summary>
    /// Obtém uma lista de DTOs de todos os animais ativos de uma clínica.
    /// </summary>
    /// <param name="clinicaId">ID da clínica.</param>
    /// <returns>ServiceResult contendo a lista de animais.</returns>
    public async Task<ServiceResult> GetAllDtosByClinicIdAsync(int clinicaId)
    {
        _logger.LogInformation("Listando todos os animais da clínica {ClinicaId}.", clinicaId);
        var animais = await _animalRepository.GetAllByClinicaIdWithDetailsAsync(clinicaId);
        var dtos = _mapper.Map<IEnumerable<GetAnimaisClinicaDto>>(animais);
        _logger.LogInformation("Encontrados {Count} animais para a clínica {ClinicaId}.", dtos.Count(), clinicaId);
        return new ServiceResult { Succeeded = true, Data = dtos };
    }

    /// <summary>
    /// Obtém os detalhes de um animal específico em formato DTO, validando o acesso pela clínica.
    /// </summary>
    /// <param name="animalId">ID do animal.</param>
    /// <param name="clinicaId">ID da clínica para validação.</param>
    /// <returns>ServiceResult contendo o DTO do animal ou uma mensagem de erro.</returns>
    public async Task<ServiceResult> GetDtoDetailsByIdAsync(int animalId, int clinicaId)
    {
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(animalId, clinicaId);
        if (!resultadoBusca.Succeeded)
        {
            return resultadoBusca;
        }
        var dto = _mapper.Map<GetAnimaisClinicaDto>((Animal)resultadoBusca.Data);
        return new ServiceResult { Succeeded = true, Data = dto };
    }

    /// <summary>
    /// Obtém um DTO com o histórico completo de um animal, incluindo acompanhamento, relatórios e dietas.
    /// </summary>
    /// <param name="animalId">ID do animal.</param>
    /// <param name="clinicaId">ID da clínica para validação.</param>
    /// <returns>ServiceResult contendo o DTO do histórico completo do animal ou uma mensagem de erro.</returns>
    public async Task<ServiceResult> ObterHistoricoCompletoAnimalAsync(int animalId, int clinicaId)
    {
        _logger.LogInformation("Buscando histórico completo do animal {AnimalId} na clínica {ClinicaId}.", animalId, clinicaId);
        var animalEntity = await _animalRepository.GetHistoricoCompletoAsync(animalId, clinicaId);
        if (animalEntity == null)
        {
            _logger.LogWarning("Histórico do animal {AnimalId} não encontrado para a clínica {ClinicaId}.", animalId, clinicaId);
            return new ServiceResult { Succeeded = false, Message = "Animal não encontrado nesta clínica." };
        }
        var animalDto = _mapper.Map<AnimalCompletoDto>(animalEntity);
        return new ServiceResult { Succeeded = true, Data = animalDto };
    }

    #endregion

    #region Métodos de Escrita

    /// <summary>
    /// Cria um novo animal, vincula-o à clínica, sincroniza seus tutores e cria seu acompanhamento inicial.
    /// </summary>
    /// <param name="dto">Dados para criação do animal.</param>
    /// <param name="clinicaId">ID da clínica onde o animal será cadastrado.</param>
    /// <returns>ServiceResult indicando sucesso ou falha na operação.</returns>
    public async Task<ServiceResult> CreateAsync(CadastrarAnimalClinicaDto dto, int clinicaId)
    {
        _logger.LogInformation("Iniciando cadastro de novo animal '{NomeAnimal}' para a clínica {ClinicaId}.", dto.Nome, clinicaId);
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var animal = _mapper.Map<Animal>(dto);
            animal.DataCadastro = DateTime.UtcNow;
            animal.DataModificado = DateTime.UtcNow;

            await _animalRepository.AddAsync(animal);
            await _animalRepository.SaveChangesAsync(); // Salva para gerar o ID do animal
            _logger.LogDebug("Animal (ID: {AnimalId}) criado no banco.", animal.Id);

            await _animalRepository.AddVinculoClinicaAsync(new AnimalClinica { AnimalId = animal.Id, ClinicaId = clinicaId, IsAtivo = true });

            var resultadoTutores = await SincronizarTutoresAsync(animal, dto.TutorIds, clinicaId);
            if (!resultadoTutores.Succeeded)
            {
                _logger.LogWarning("Falha na sincronização de tutores para o novo animal. Mensagem: {MensagemErro}", resultadoTutores.Message);
                await transaction.RollbackAsync();
                return resultadoTutores;
            }

            var acompanhamento = new Acompanhamento
            {
                AnimalId = animal.Id,
                DataInicio = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _acompanhamentoRepository.AddAsync(acompanhamento);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Animal '{NomeAnimal}' (ID: {AnimalId}) cadastrado com sucesso na clínica {ClinicaId}.", animal.Nome, animal.Id, clinicaId);
            return new ServiceResult { Succeeded = true, Message = "Animal cadastrado com sucesso.", Data = new { AnimalId = animal.Id } };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro catastrófico ao cadastrar animal para a clínica {ClinicaId}. DTO: {@Dto}", clinicaId, dto);
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro ao cadastrar o animal." };
        }
    }

    /// <summary>
    /// Atualiza os dados de um animal existente e sincroniza seus tutores.
    /// </summary>
    /// <param name="dto">Dados para edição do animal.</param>
    /// <param name="clinicaId">ID da clínica para validação de acesso.</param>
    /// <returns>ServiceResult indicando sucesso ou falha na operação.</returns>
    public async Task<ServiceResult> UpdateAsync(EditarAnimalClinicaDto dto, int clinicaId)
    {
        _logger.LogInformation("Iniciando atualização do animal {AnimalId} na clínica {ClinicaId}.", dto.Id, clinicaId);
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var resultadoBusca = await ObterPorIdEClinicaIdAsync(dto.Id, clinicaId);
            if (!resultadoBusca.Succeeded) return resultadoBusca;

            var animal = (Animal)resultadoBusca.Data;

            _mapper.Map(dto, animal);
            animal.DataModificado = DateTime.UtcNow;

            if (dto.TutorIds != null)
            {
                var resultadoTutores = await SincronizarTutoresAsync(animal, dto.TutorIds, clinicaId);
                if (!resultadoTutores.Succeeded)
                {
                    _logger.LogWarning("Falha na sincronização de tutores para o animal {AnimalId}. Mensagem: {MensagemErro}", dto.Id, resultadoTutores.Message);
                    await transaction.RollbackAsync();
                    return resultadoTutores;
                }
            }

            await _animalRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Animal {AnimalId} atualizado com sucesso.", animal.Id);
            return new ServiceResult { Succeeded = true, Message = "Animal atualizado com sucesso.", Data = new { AnimalId = animal.Id } };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro catastrófico ao atualizar animal {AnimalId}. DTO: {@Dto}", dto.Id, dto);
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro interno ao atualizar o animal." };
        }
    }

    /// <summary>
    /// Realiza um soft delete no vínculo entre um animal e uma clínica.
    /// </summary>
    /// <param name="animalId">ID do animal.</param>
    /// <param name="clinicaId">ID da clínica.</param>
    /// <returns>ServiceResult indicando sucesso ou falha.</returns>
    public async Task<ServiceResult> DesativarVinculoAnimalAsync(int animalId, int clinicaId)
    {
        _logger.LogInformation("Tentando desativar vínculo do animal {AnimalId} com a clínica {ClinicaId}.", animalId, clinicaId);
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var vinculo = await _animalRepository.GetVinculoAnimalClinicaAsync(animalId, clinicaId);
            if (vinculo == null)
            {
                _logger.LogWarning("Vínculo Animal-Clínica não encontrado para animal {AnimalId} e clínica {ClinicaId}.", animalId, clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Vínculo do animal com a clínica não encontrado." };
            }

            if (!vinculo.IsAtivo)
            {
                _logger.LogInformation("Vínculo para animal {AnimalId} na clínica {ClinicaId} já se encontra inativo.", animalId, clinicaId);
                return new ServiceResult { Succeeded = true, Message = "O vínculo já estava inativo." };
            }

            vinculo.IsAtivo = false;
            vinculo.DataDesativacao = DateTime.UtcNow;

            _animalRepository.UpdateVinculo(vinculo);
            await _animalRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Vínculo do animal {AnimalId} com a clínica {ClinicaId} foi desativado com sucesso.", animalId, clinicaId);
            return new ServiceResult { Succeeded = true, Message = "Vínculo do animal desativado com sucesso." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro ao desativar vínculo do animal {AnimalId} com a clínica {ClinicaId}.", animalId, clinicaId);
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro ao desativar o vínculo." };
        }
    }

    #endregion

    #region Métodos de Validação e Auxiliares

    /// <summary>
    /// Verifica se um animal tem um vínculo ativo com uma determinada clínica.
    /// </summary>
    /// <param name="animalId">ID do animal.</param>
    /// <param name="clinicaId">ID da clínica.</param>
    /// <returns>Verdadeiro se o vínculo ativo existir, falso caso contrário.</returns>
    public async Task<bool> AnimalExisteNaClinicaAsync(int animalId, int clinicaId)
    {
        return await _animalRepository.IsAnimalInClinicAsync(animalId, clinicaId);
    }

    public async Task<bool> AnimaisExistemNaClinicaAsync(List<int> animaisIds, int clinicaId)
    {
        return await _animalRepository.AllAnimalsExistInClinicAsync(animaisIds, clinicaId);
    }

    /// <summary>
    /// Método privado que centraliza a lógica de adicionar e remover vínculos de tutores a um animal.
    /// </summary>
    /// <param name="animal">A entidade Animal a ser modificada.</param>
    /// <param name="idsDesejados">A lista final de IDs de tutores que devem estar vinculados.</param>
    /// <param name="clinicaId">O ID da clínica para validar os tutores.</param>
    /// <returns>ServiceResult indicando o sucesso da operação.</returns>
    private async Task<ServiceResult> SincronizarTutoresAsync(Animal animal, List<int> idsDesejados, int clinicaId)
    {
        // Garante que a lista de desejados não seja nula para evitar erros.
        idsDesejados ??= new List<int>();

        var resultadoValidacao = await _tutorService.ValidarTutoresDaClinicaAsync(idsDesejados, clinicaId);
        if (!resultadoValidacao.Succeeded)
        {
            return resultadoValidacao;
        }

        var idsAtuais = animal.TutoresAnimais?.Select(ta => ta.TutorId).ToList() ?? new List<int>();

        // Remove vínculos que não estão mais na lista de desejados
        var paraRemover = animal.TutoresAnimais?.Where(ta => !idsDesejados.Contains(ta.TutorId)).ToList();
        if (paraRemover?.Any() == true)
        {
            _logger.LogDebug("Removendo {Count} vínculo(s) de tutor(es) do animal {AnimalId}.", paraRemover.Count, animal.Id);
            _animalRepository.RemoveTutorLinks(paraRemover);
        }

        // Adiciona novos vínculos que não existiam antes
        var paraAdicionar = idsDesejados.Except(idsAtuais).ToList();
        if (paraAdicionar.Any())
        {
            _logger.LogDebug("Adicionando {Count} novo(s) vínculo(s) de tutor(es) ao animal {AnimalId}.", paraAdicionar.Count, animal.Id);
            foreach (var tutorId in paraAdicionar)
            {
                await _animalRepository.AddTutorLinkAsync(new TutorAnimal { AnimalId = animal.Id, TutorId = tutorId });
            }
        }

        return new ServiceResult { Succeeded = true };
    }

    #endregion
}