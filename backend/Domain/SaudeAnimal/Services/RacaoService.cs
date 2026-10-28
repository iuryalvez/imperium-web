using AutoMapper;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.DTOs.Racao;

namespace backend.Domain.SaudeAnimal.Services;

/// <summary>
/// Serviço responsável pela lógica de negócio relacionada a Rações.
/// Inclui operações de CRUD e cálculos nutricionais complexos.
/// </summary>
public class RacaoService : IRacaoService
{
    // Fatores de Atwater modificados para cães e gatos (kcal/g)
    private const double AtwaterFactorProteina = 3.5;
    private const double AtwaterFactorGordura = 8.5;
    private const double AtwaterFactorCarboidrato = 3.5;

    // Fatores para cálculo de Energia Bruta (kcal/g)
    private const double EnergiaBrutaFactorProteina = 5.7;
    private const double EnergiaBrutaFactorGordura = 9.4;
    private const double EnergiaBrutaFactorCarboidratoFibra = 4.1;

    private readonly IRacaoRepository _racaoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RacaoService> _logger;

    /// <summary>
    /// Inicia uma nova instância da classe <see cref="RacaoService"/>.
    /// </summary>
    /// <param name="racaoRepository">O repositório para acesso aos dados de rações.</param>
    /// <param name="mapper">A instância do AutoMapper para mapeamento de objetos.</param>
    /// <param name="logger">O logger para registrar as operações do serviço.</param>
    public RacaoService(IRacaoRepository racaoRepository, IMapper mapper, ILogger<RacaoService> logger)
    {
        _racaoRepository = racaoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    #region Métodos de Leitura

    /// <summary>
    /// Obtém a entidade Racao pelo seu ID, validando a permissão da clínica.
    /// Retorna rações do sistema (ClinicaId nulo) ou da própria clínica; nunca de outra clínica.
    /// Este método é um bloco de construção para outros métodos do serviço.
    /// </summary>
    /// <param name="racaoId">O ID da ração a ser obtida.</param>
    /// <param name="clinicaId">O ID da clínica para validação de posse.</param>
    /// <returns>Um ServiceResult contendo a entidade Racao em caso de sucesso.</returns>
    public async Task<ServiceResult> ObterPorIdEClinicaIdAsync(int racaoId, int clinicaId)
    {
        _logger.LogInformation("Buscando entidade Ração ID {RacaoId} para a clínica ID {ClinicaId}.", racaoId, clinicaId);
        try
        {
            var racao = await _racaoRepository.GetAvailableByIdAndClinicIdAsync(racaoId, clinicaId);
            if (racao == null)
            {
                _logger.LogWarning("Ração ID {RacaoId} não encontrada ou não pertence à clínica ID {ClinicaId}.", racaoId, clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Ração não encontrada ou não pertence a esta clínica." };
            }

            _logger.LogInformation("Entidade Ração ID {RacaoId} encontrada com sucesso.", racaoId);
            return new ServiceResult { Succeeded = true, Data = racao };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar a entidade Ração ID {RacaoId}.", racaoId);
            throw;
        }
    }

    /// <summary>
    /// Obtém um DTO de uma ração específica pelo seu ID.
    /// </summary>
    /// <param name="racaoId">O ID da ração.</param>
    /// <param name="clinicaId">O ID da clínica.</param>
    /// <returns>Um ServiceResult contendo o DTO da ração em caso de sucesso.</returns>
    public async Task<ServiceResult> ObterDtoPorIdAsync(int racaoId, int clinicaId)
    {
        _logger.LogInformation("Buscando DTO da Ração ID {RacaoId} para a clínica ID {ClinicaId}.", racaoId, clinicaId);
        
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(racaoId, clinicaId);
        if (!resultadoBusca.Succeeded)
        {
            return resultadoBusca; // O log de falha já foi gerado em ObterPorIdAsync
        }

        var racao = (Racao)resultadoBusca.Data;
        var dto = _mapper.Map<GetRacoesDto>(racao);

        return new ServiceResult { Succeeded = true, Data = dto };
    }

    /// <summary>
    /// Obtém uma lista de DTOs das rações disponíveis para a clínica (sistema + próprias), com filtro opcional por espécie.
    /// </summary>
    /// <param name="clinicaId">O ID da clínica.</param>
    /// <param name="especie">A espécie do animal para filtrar os resultados (opcional).</param>
    /// <returns>Um ServiceResult contendo a lista de DTOs das rações.</returns>
    public async Task<ServiceResult> ObterTodosDtosPorClinicaIdEEspecieAsync(int clinicaId, EspecieAnimal? especie)
    {
        _logger.LogInformation("Buscando todos os DTOs de ração para a clínica ID {ClinicaId}. Filtro de espécie: {Especie}", clinicaId, especie?.ToString() ?? "Nenhum");
        try
        {
            var racoes = await _racaoRepository.GetAllAvailableByClinicIdAndEspecieAsync(clinicaId, especie);
            var dtos = _mapper.Map<IEnumerable<GetRacoesDto>>(racoes);
            
            _logger.LogInformation("Busca de rações para a clínica ID {ClinicaId} concluída com sucesso.", clinicaId);
            return new ServiceResult { Succeeded = true, Data = dtos };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todas as rações para a clínica ID {ClinicaId}.", clinicaId);
            throw;
        }
    }

    #endregion

    #region Métodos de Escrita

    /// <summary>
    /// Cria uma nova ração, executa os cálculos nutricionais e a persiste no banco de dados.
    /// </summary>
    /// <param name="dto">O DTO com os dados para a criação da ração.</param>
    /// <param name="clinicaId">O ID da clínica proprietária da ração.</param>
    /// <returns>Um ServiceResult contendo o DTO da ração criada.</returns>
    public async Task<ServiceResult> CreateAsync(CadastrarRacaoDto dto, int clinicaId)
    {
        _logger.LogInformation("Iniciando criação de nova ração '{NomeRacao}' para a clínica ID {ClinicaId}.", dto.Nome, clinicaId);
        try
        {
            var racao = _mapper.Map<Racao>(dto);
            racao.ClinicaId = clinicaId;

            _logger.LogInformation("Executando cálculos nutricionais para a nova ração '{NomeRacao}'.", racao.Nome);
            CalcularEnergiaMetabolizavel(racao);
            CalcularDistribuicaoEnergetica(racao);

            await _racaoRepository.AddAsync(racao);
            await _racaoRepository.SaveChangesAsync();

            _logger.LogInformation("Ração '{NomeRacao}' (ID: {RacaoId}) criada com sucesso.", racao.Nome, racao.Id);
            var resultDto = _mapper.Map<GetRacoesDto>(racao);
            return new ServiceResult { Succeeded = true, Message = "Ração cadastrada com sucesso.", Data = resultDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar a ração '{NomeRacao}' para a clínica ID {ClinicaId}.", dto.Nome, clinicaId);
            throw;
        }
    }

    /// <summary>
    /// Atualiza os dados de uma ração existente e recalcula seus valores nutricionais.
    /// Se a ração for do sistema (ClinicaId nulo), cria uma cópia específica para a clínica com as alterações.
    /// </summary>
    /// <param name="dto">O DTO com os dados a serem atualizados.</param>
    /// <param name="clinicaId">O ID da clínica para validação de posse.</param>
    /// <returns>Um ServiceResult contendo o DTO da ração atualizada.</returns>
    public async Task<ServiceResult> UpdateAsync(AtualizarRacaoDto dto, int clinicaId)
    {
        _logger.LogInformation("Iniciando atualização da ração ID {RacaoId} para a clínica ID {ClinicaId}.", dto.Id, clinicaId);
        
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(dto.Id, clinicaId);
        if (!resultadoBusca.Succeeded)
        {
            return resultadoBusca;
        }
        
        try
        {
            var racao = (Racao)resultadoBusca.Data;

            if (racao.ClinicaId == null)
            {
                _logger.LogInformation("A ração {RacaoId} é do sistema. Criando uma cópia específica para a clínica {ClinicaId}.", racao.Id, clinicaId);
                return await CreateClinicSpecificCopyAsync(racao, dto, clinicaId);
            }

            _mapper.Map(dto, racao);

            _logger.LogInformation("Recalculando valores nutricionais para a ração ID {RacaoId} após atualização.", dto.Id);
            CalcularEnergiaMetabolizavel(racao);
            CalcularDistribuicaoEnergetica(racao);

            await _racaoRepository.SaveChangesAsync();

            _logger.LogInformation("Ração ID {RacaoId} atualizada com sucesso.", racao.Id);
            var resultDto = _mapper.Map<GetRacoesDto>(racao);
            return new ServiceResult { Succeeded = true, Message = "Ração atualizada com sucesso.", Data = resultDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar a ração ID {RacaoId}.", dto.Id);
            throw;
        }
    }

    /// <summary>
    /// Exclui uma ração do sistema.
    /// </summary>
    /// <param name="racaoId">O ID da ração a ser excluída.</param>
    /// <param name="clinicaId">O ID da clínica para validação de posse.</param>
    /// <returns>Um ServiceResult indicando o sucesso da operação.</returns>
    public async Task<ServiceResult> DeleteAsync(int racaoId, int clinicaId)
    {
        _logger.LogInformation("Iniciando exclusão da ração ID {RacaoId} da clínica ID {ClinicaId}.", racaoId, clinicaId);
        
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(racaoId, clinicaId);
        if (!resultadoBusca.Succeeded)
        {
            return resultadoBusca;
        }
        
        try
        {
            var racao = (Racao)resultadoBusca.Data;

            if (racao.ClinicaId == null)
            {
                _logger.LogWarning("A clínica {ClinicaId} tentou excluir a ração do sistema {RacaoId}.", clinicaId, racaoId);
                return new ServiceResult { Succeeded = false, Message = "Rações do sistema não podem ser excluídas pela clínica." };
            }

            _racaoRepository.Delete(racao);
            await _racaoRepository.SaveChangesAsync();

            _logger.LogInformation("Ração ID {RacaoId} excluída com sucesso.", racaoId);
            return new ServiceResult { Succeeded = true, Message = "Ração excluída com sucesso." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir a ração ID {RacaoId}.", racaoId);
            throw;
        }
    }

    /// <summary>
    /// Cria uma ração específica para a clínica a partir de uma ração do sistema, aplicando as alterações do DTO.
    /// A ração do sistema permanece inalterada.
    /// </summary>
    private async Task<ServiceResult> CreateClinicSpecificCopyAsync(Racao racaoSistema, AtualizarRacaoDto dto, int clinicaId)
    {
        var copia = new Racao
        {
            ClinicaId = clinicaId,
            Especie = racaoSistema.Especie,
            Nome = racaoSistema.Nome,
            IsPorcentagem = racaoSistema.IsPorcentagem,
            Umidade = racaoSistema.Umidade,
            ProteinaBruta = racaoSistema.ProteinaBruta,
            ExtratoEtereo = racaoSistema.ExtratoEtereo,
            MateriaMineral = racaoSistema.MateriaMineral,
            FibraBruta = racaoSistema.FibraBruta,
            FibraDieteticaTotal = racaoSistema.FibraDieteticaTotal
        };

        _mapper.Map(dto, copia);
        copia.Id = 0; // Garante que será tratada como uma nova entidade pelo ORM.
        copia.ClinicaId = clinicaId;

        CalcularEnergiaMetabolizavel(copia);
        CalcularDistribuicaoEnergetica(copia);

        await _racaoRepository.AddAsync(copia);
        await _racaoRepository.SaveChangesAsync();

        _logger.LogInformation("Cópia da ração do sistema {RacaoSistemaId} criada para a clínica {ClinicaId} com ID {RacaoId}.", racaoSistema.Id, clinicaId, copia.Id);
        var resultDto = _mapper.Map<GetRacoesDto>(copia);
        return new ServiceResult { Succeeded = true, Message = "Ração do sistema copiada para a clínica com as alterações.", Data = resultDto };
    }

    #endregion

    #region Métodos de Lógica de Negócio (Privados)

    /// <summary>
    /// Calcula a Energia Metabolizável (EM) da ração com base em seus componentes.
    /// O cálculo segue as fórmulas do NRC (2006).
    /// </summary>
    private void CalcularEnergiaMetabolizavel(Racao racao)
    {
        _logger.LogTrace("Iniciando cálculo de EM para ração '{NomeRacao}', espécie: {Especie}", racao.Nome, racao.Especie);

        // Normaliza os valores de entrada se estiverem em g/kg
        double pb = racao.IsPorcentagem ? racao.ProteinaBruta : racao.ProteinaBruta / 10.0;
        double eea = racao.IsPorcentagem ? racao.ExtratoEtereo : racao.ExtratoEtereo / 10.0;
        double mm = racao.IsPorcentagem ? racao.MateriaMineral : racao.MateriaMineral / 10.0;
        double fb = racao.IsPorcentagem ? racao.FibraBruta : racao.FibraBruta / 10.0;
        double umidade = racao.IsPorcentagem ? racao.Umidade : racao.Umidade / 10.0;
        _logger.LogTrace("Valores normalizados (%): PB={pb}, EEA={eea}, MM={mm}, FB={fb}, Umidade={umidade}", pb, eea, mm, fb, umidade);

        // 1. Converter % para fração decimal para os cálculos
        umidade /= 100.0;
        pb /= 100.0;
        eea /= 100.0;
        fb /= 100.0;
        mm /= 100.0;

        // 2. Calcular matéria seca (MS)
        double ms = 1.0 - umidade;
        racao.MateriaSeca = Math.Round(ms * 100.0, 3); // Armazenar como porcentagem com 3 casas decimais
        _logger.LogTrace("Matéria seca (MS) = {Ms:F2}%", racao.MateriaSeca);

        // 3. Calcular Extrativo Não Nitrogenado (ENN)
        double enn = 1.0 - (umidade + pb + eea + fb + mm);
        racao.ExtrativoNaoNitrogenado = Math.Round(enn * 100.0, 3); // Armazenar como porcentagem com 3 casas decimais
        _logger.LogTrace("ENN = {Enn:F2}%", racao.ExtrativoNaoNitrogenado);

        // 4. Calcular Energia Bruta (EB)
        double eb = (EnergiaBrutaFactorProteina * pb) + (EnergiaBrutaFactorGordura * eea) + (EnergiaBrutaFactorCarboidratoFibra * (enn + fb));
        racao.EnergiaBruta = Math.Round(eb, 3); // Arredonda
        _logger.LogTrace("Energia Bruta (EB) = {Eb:F4} kcal/g", eb);

        // 5. Calcular Coeficiente de Digestibilidade da Energia (CDE)
        double cde;
        double fbMs = (ms > 0) ? (fb / ms * 100.0) : 0;
        switch (racao.Especie)
        {
            case EspecieAnimal.Cao:
                cde = 91.2 - (1.43 * fbMs);
                _logger.LogTrace("CDE (cão): FB na MS={FbMs:F2}%, CDE={Cde:F2}%", fbMs, cde);
                break;
            case EspecieAnimal.Gato:
                cde = 87.9 - (0.88 * fbMs);
                _logger.LogTrace("CDE (gato): FB na MS={FbMs:F2}%, CDE={Cde:F2}%", fbMs, cde);
                break;
            default:
                throw new InvalidOperationException($"Espécie '{racao.Especie}' não suportada para cálculo de EM.");
        }
        racao.CoeficienteDigestibilidadeEnergia = Math.Round(cde, 3); // Armazenar com 3 casas decimais
        _logger.LogTrace("Coeficiente de Digestibilidade da Energia (CDE) = {Cde:F2}%", racao.CoeficienteDigestibilidadeEnergia);

        // 6. Calcular Energia Digestível (ED)
        double ed = eb * (cde / 100.0);
        racao.EnergiaDigestivel = Math.Round(ed, 3); // Armazenar com 3 casas decimais
        _logger.LogTrace("Energia Digestível (ED) = {Ed:F4} kcal/g", ed);

        // 7. Calcular Energia Metabolizável (EM)
        double em = (racao.Especie == EspecieAnimal.Cao) ? ed - (1.04 * pb) : ed - (0.77 * pb);
        racao.EnergiaMetabolizavel = Math.Round(em, 3); // Armazenar com 3 casas decimais
        _logger.LogTrace("Energia Metabolizável (EM) = {Em:F4} kcal/g", em);
    }
    
    /// <summary>
    /// Calcula a distribuição energética da ração (kcal/100g) usando os fatores de Atwater modificados.
    /// </summary>
    private void CalcularDistribuicaoEnergetica(Racao racao)
    {
        if (!racao.ExtrativoNaoNitrogenado.HasValue)
        {
            _logger.LogError("Cálculo de distribuição energética falhou: ENN não foi calculado previamente para a ração '{NomeRacao}'.", racao.Nome);
            throw new InvalidOperationException("Não é possível calcular a distribuição energética sem o ENN calculado.");
        }
        _logger.LogTrace("Iniciando cálculo de distribuição energética para ração '{NomeRacao}'.", racao.Nome);

        // Normaliza os valores de entrada se estiverem em g/kg
        double pb = racao.IsPorcentagem ? racao.ProteinaBruta : racao.ProteinaBruta / 10.0;
        double eea = racao.IsPorcentagem ? racao.ExtratoEtereo : racao.ExtratoEtereo / 10.0;
        double enn = racao.ExtrativoNaoNitrogenado.Value; // Já está em %

        // Calcula a energia de cada componente em kcal/100g de ração
        double energiaPB = pb * AtwaterFactorProteina;
        double energiaEEA = eea * AtwaterFactorGordura;
        double energiaENN = enn * AtwaterFactorCarboidrato;
        double energiaTotal = energiaPB + energiaEEA + energiaENN;

        racao.EnergiaProteina = Math.Round(energiaPB, 3);
        racao.EnergiaExtratoEtereo = Math.Round(energiaEEA, 3);
        racao.EnergiaExtrativoNaoNitrogenado = Math.Round(energiaENN, 3);
        racao.EnergiaTotalEstimativa = Math.Round(energiaTotal, 3);

        _logger.LogTrace("Distribuição energética (kcal/100g): PB={EnergiaPb:F2}, EEA={EnergiaEea:F2}, ENN={EnergiaEnn:F2}, Total={EnergiaTotal:F2}",
            racao.EnergiaProteina, racao.EnergiaExtratoEtereo, racao.EnergiaExtrativoNaoNitrogenado, racao.EnergiaTotalEstimativa);
    }
    
    #endregion 
}