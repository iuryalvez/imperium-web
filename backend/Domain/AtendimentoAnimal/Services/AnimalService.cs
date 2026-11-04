using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.AcompanhamentoDto;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Services;

/// <summary>
/// Serviço responsável por gerenciar as regras de negócio relacionadas a animais.
/// </summary>
public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly ITutorService _tutorService;
    private readonly IAcompanhamentoRepository _acompanhamentoRepository;
    private readonly ITutorRepository _tutorRepository;
    private readonly IRacaService _racaService;
    private readonly AppDbContext _context; // Usado exclusivamente para controle de transações.
    private readonly IMapper _mapper;
    private readonly ILogger<AnimalService> _logger;

    public AnimalService(
        IAnimalRepository animalRepository,
        ITutorService tutorService,
        IAcompanhamentoRepository acompanhamentoRepository,
        ITutorRepository tutorRepository,
        IRacaService racaService,
        IMapper mapper,
        ILogger<AnimalService> logger,
        AppDbContext context)
    {
        _animalRepository = animalRepository;
        _tutorService = tutorService;
        _acompanhamentoRepository = acompanhamentoRepository;
        _tutorRepository = tutorRepository;
        _racaService = racaService;
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
            return ServiceResult.NaoEncontrado($"Animal com Id {animalId} não encontrado.");
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
        _logger.LogInformation("Buscando entidade do animal {AnimalId} para a clínica {ClinicaId}.", animalId, clinicaId);
        var animal = await _animalRepository.GetByIdAndClinicaIdWithIncludesAsync(animalId, clinicaId);
        if (animal == null)
        {
            _logger.LogWarning("Animal {AnimalId} não encontrado ou não pertence à clínica {ClinicaId}.", animalId, clinicaId);
            return ServiceResult.NaoEncontrado($"Animal com ID {animalId} não encontrado nesta clínica.");
        }
        return new ServiceResult { Succeeded = true, Data = animal };
    }

    /// <summary>
    /// Lista paginada dos animais da clínica (ativos e inativos), mais recentes primeiro por padrão.
    /// </summary>
    /// <param name="filtro">Paginação, busca, ordenação e filtros (ativo, espécie, tutor).</param>
    /// <param name="clinicaId">ID da clínica.</param>
    public async Task<PaginaResultado<GetAnimaisClinicaDto>> ListarAsync(AnimalFiltro filtro, int clinicaId)
    {
        var pagina = await _animalRepository.ListarDaClinicaAsync(filtro, clinicaId);
        return pagina.Converter(itens => _mapper.Map<IEnumerable<GetAnimaisClinicaDto>>(itens));
    }

    /// <summary>Opções para selects: até 20 animais { id, nome, isAtivo }, ativos primeiro.</summary>
    public async Task<List<OpcaoDto>> ListarOpcoesAsync(int clinicaId, string? busca)
    {
        return await _animalRepository.ListarOpcoesAsync(clinicaId, busca);
    }

    public async Task<ServiceResult> UpdateEntityAsync(Animal animal)
    {
        await _animalRepository.UpdateAsync(animal);
        await _animalRepository.SaveChangesAsync();
        return new ServiceResult { Succeeded = true, Message = "Animal atualizado com sucesso." };
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
    /// <param name="incluirDadosDoTutor">Falso para quem só pode ver o nome do tutor (TECNICO): telefone e e-mail vão nulos.</param>
    /// <returns>ServiceResult contendo o DTO do histórico completo do animal ou uma mensagem de erro.</returns>
    public async Task<ServiceResult> ObterHistoricoCompletoAnimalAsync(int animalId, int clinicaId, bool incluirDadosDoTutor = true)
    {
        _logger.LogInformation("Buscando histórico completo do animal {AnimalId} na clínica {ClinicaId}.", animalId, clinicaId);
        var animalEntity = await _animalRepository.GetHistoricoCompletoAsync(animalId, clinicaId);
        if (animalEntity == null)
        {
            _logger.LogWarning("Histórico do animal {AnimalId} não encontrado para a clínica {ClinicaId}.", animalId, clinicaId);
            return ServiceResult.NaoEncontrado("Animal não encontrado nesta clínica.");
        }
        var animalDto = _mapper.Map<AnimalCompletoDto>(animalEntity);
        if (!incluirDadosDoTutor)
        {
            foreach (var tutor in animalDto.Tutores ?? new List<TutorResumidoDto>())
            {
                tutor.Telefone = null;
                tutor.Email = null;
            }
        }
        return new ServiceResult { Succeeded = true, Data = animalDto };
    }

    #endregion

    #region Métodos de Escrita

    /// <summary>
    /// Cria um novo animal na clínica, sincroniza seus tutores e cria seu acompanhamento inicial.
    /// </summary>
    /// <param name="dto">Dados para criação do animal.</param>
    /// <param name="clinicaId">ID da clínica onde o animal será cadastrado.</param>
    /// <returns>ServiceResult indicando sucesso ou falha na operação.</returns>
    public async Task<ServiceResult> CreateAsync(CadastrarAnimalClinicaDto dto, int clinicaId)
    {
        _logger.LogInformation("Iniciando cadastro de novo animal '{NomeAnimal}' para a clínica {ClinicaId}.", dto.Nome, clinicaId);

        // A raça precisa estar disponível para a clínica, ativa e não oculta
        var resultadoRaca = await _racaService.ValidarSelecionavelAsync(dto.RacaId, clinicaId);
        if (!resultadoRaca.Succeeded) return resultadoRaca;

        // A raça precisa ser da espécie do animal (o formulário já filtra; a API também recusa)
        var resultadoEspecie = await _racaService.ValidarEspecieAsync(dto.RacaId, clinicaId, dto.Especie);
        if (!resultadoEspecie.Succeeded) return resultadoEspecie;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var animal = _mapper.Map<Animal>(dto);
            animal.ClinicaId = clinicaId;
            animal.IsAtivo = true;
            animal.DataCadastro = DateTime.UtcNow;
            animal.DataModificado = DateTime.UtcNow;

            await _animalRepository.AddAsync(animal);
            await _animalRepository.SaveChangesAsync(); // Salva para gerar o ID do animal
            _logger.LogInformation("Animal (ID: {AnimalId}) criado no banco.", animal.Id);

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

            // Trocar a raça exige uma raça selecionável; manter a atual é sempre permitido
            if (dto.RacaId.HasValue && dto.RacaId != animal.RacaId)
            {
                var resultadoRaca = await _racaService.ValidarSelecionavelAsync(dto.RacaId.Value, clinicaId);
                if (!resultadoRaca.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoRaca;
                }
            }

            // Mudou a raça ou a espécie: a raça final precisa ser da espécie final
            var racaFinal = dto.RacaId ?? animal.RacaId;
            var especieFinal = dto.Especie ?? animal.Especie;
            if (racaFinal.HasValue && ((dto.RacaId.HasValue && dto.RacaId != animal.RacaId) || especieFinal != animal.Especie))
            {
                var resultadoEspecie = await _racaService.ValidarEspecieAsync(racaFinal.Value, clinicaId, especieFinal);
                if (!resultadoEspecie.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoEspecie;
                }
            }

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
    /// Ativa ou desativa (soft delete) o animal na clínica. Animal inativo continua no histórico,
    /// mas não pode ganhar vínculos novos nem consultas novas.
    /// </summary>
    /// <param name="animalId">ID do animal.</param>
    /// <param name="clinicaId">ID da clínica.</param>
    /// <param name="ativo">true para ativar, false para desativar.</param>
    /// <returns>ServiceResult indicando sucesso ou falha.</returns>
    public async Task<ServiceResult> SetStatusAsync(int animalId, int clinicaId, bool ativo)
    {
        _logger.LogInformation("Alterando status do animal {AnimalId} na clínica {ClinicaId} para ativo={Ativo}.", animalId, clinicaId, ativo);
        try
        {
            var animal = await _animalRepository.GetByIdAndClinicaIdWithIncludesAsync(animalId, clinicaId);
            if (animal == null)
            {
                _logger.LogWarning("Animal {AnimalId} não encontrado na clínica {ClinicaId}.", animalId, clinicaId);
                return ServiceResult.NaoEncontrado("Animal não encontrado nesta clínica.");
            }

            animal.IsAtivo = ativo;
            animal.DataDesativacao = ativo ? null : DateTime.UtcNow;
            await _animalRepository.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = ativo ? "Animal ativado com sucesso." : "Animal desativado com sucesso." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar o status do animal {AnimalId} da clínica {ClinicaId}.", animalId, clinicaId);
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro ao alterar o status do animal." };
        }
    }

    /// <summary>
    /// Exclui um animal da clínica. Animais com consultas registradas não podem ser excluídos (devem ser desativados).
    /// </summary>
    /// <param name="animalId">ID do animal.</param>
    /// <param name="clinicaId">ID da clínica.</param>
    /// <returns>ServiceResult indicando sucesso ou falha.</returns>
    public async Task<ServiceResult> DeleteAsync(int animalId, int clinicaId)
    {
        _logger.LogInformation("Tentando excluir o animal {AnimalId} da clínica {ClinicaId}.", animalId, clinicaId);
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var animal = await _animalRepository.GetByIdAndClinicaIdWithIncludesAsync(animalId, clinicaId);
            if (animal == null)
            {
                _logger.LogWarning("Tentativa de excluir animal {AnimalId} que não pertence à clínica {ClinicaId}.", animalId, clinicaId);
                return ServiceResult.NaoEncontrado($"Animal com ID {animalId} não encontrado nesta clínica.");
            }

            if (await _animalRepository.HasConsultasAsync(animalId))
            {
                return ServiceResult.Conflito("Este animal possui consultas registradas e não pode ser excluído. Desative-o.");
            }

            if (animal.TutoresAnimais != null && animal.TutoresAnimais.Any())
            {
                _animalRepository.RemoveTutorLinks(animal.TutoresAnimais);
            }
            _animalRepository.Remove(animal);

            await _animalRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Animal {AnimalId} excluído da clínica {ClinicaId}.", animalId, clinicaId);
            return new ServiceResult { Succeeded = true, Message = "Animal excluído com sucesso." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro ao excluir o animal {AnimalId} da clínica {ClinicaId}.", animalId, clinicaId);
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro interno ao excluir o animal." };
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

        // Tutor e animal precisam ser da mesma clínica
        if (animal.ClinicaId != clinicaId)
        {
            return new ServiceResult { Succeeded = false, Message = "O animal não pertence a esta clínica." };
        }

        var resultadoValidacao = await _tutorService.ValidarTutoresDaClinicaAsync(idsDesejados, clinicaId);
        if (!resultadoValidacao.Succeeded)
        {
            return resultadoValidacao;
        }

        var idsAtuais = animal.TutoresAnimais?.Select(ta => ta.TutorId).ToList() ?? new List<int>();

        // Tutores inativos não podem ganhar vínculo novo (os vínculos já existentes são mantidos)
        var idsNovos = idsDesejados.Except(idsAtuais).ToList();
        if (!await _tutorRepository.AllTutorsActiveAsync(idsNovos, clinicaId))
        {
            return new ServiceResult { Succeeded = false, Message = "Um ou mais tutores selecionados estão inativos." };
        }

        // Remove vínculos que não estão mais na lista de desejados
        var paraRemover = animal.TutoresAnimais?.Where(ta => !idsDesejados.Contains(ta.TutorId)).ToList();
        if (paraRemover?.Any() == true)
        {
            _logger.LogInformation("Removendo {Count} vínculo(s) de tutor(es) do animal {AnimalId}.", paraRemover.Count, animal.Id);
            _animalRepository.RemoveTutorLinks(paraRemover);
        }

        // Adiciona novos vínculos que não existiam antes
        var paraAdicionar = idsDesejados.Except(idsAtuais).ToList();
        if (paraAdicionar.Any())
        {
            _logger.LogInformation("Adicionando {Count} novo(s) vínculo(s) de tutor(es) ao animal {AnimalId}.", paraAdicionar.Count, animal.Id);
            foreach (var tutorId in paraAdicionar)
            {
                await _animalRepository.AddTutorLinkAsync(new TutorAnimal { AnimalId = animal.Id, TutorId = tutorId, DataAssociacao = DateTime.UtcNow });
            }
        }

        return new ServiceResult { Succeeded = true };
    }

    #endregion
}