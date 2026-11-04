using AutoMapper;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.DTOs.Racao;

namespace backend.Domain.SaudeAnimal.Services;

/// <summary>
/// Serviço responsável pela lógica de negócio relacionada a Rações.
/// Rações do sistema (ClinicaId nulo) são compartilhadas: a clínica pode usá-las, ocultá-las só para si
/// ou editá-las (o que cria uma cópia própria). Só o admin altera, ativa/desativa ou exclui rações do sistema.
/// Inclui os cálculos nutricionais.
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

    private const string MsgNaoEncontrada = "Ração não encontrada ou não pertence a esta clínica.";
    private const string MsgEmUso = "Esta ração está em uso por dietas ou avaliações e não pode ser excluída. Desative-a.";

    private readonly IRacaoRepository _racaoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RacaoService> _logger;

    public RacaoService(IRacaoRepository racaoRepository, IMapper mapper, ILogger<RacaoService> logger)
    {
        _racaoRepository = racaoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    #region Clínica

    /// <summary>
    /// Obtém a entidade Racao disponível para a clínica (do sistema ou própria; nunca de outra clínica).
    /// Bloco de construção para outros serviços.
    /// </summary>
    public async Task<ServiceResult> ObterPorIdEClinicaIdAsync(int racaoId, int clinicaId)
    {
        var racao = await _racaoRepository.GetAvailableByIdAndClinicIdAsync(racaoId, clinicaId);
        if (racao == null)
        {
            _logger.LogWarning("Ração ID {RacaoId} não encontrada ou não pertence à clínica ID {ClinicaId}.", racaoId, clinicaId);
            return ServiceResult.NaoEncontrado(MsgNaoEncontrada);
        }
        return new ServiceResult { Succeeded = true, Data = racao };
    }

    /// <summary>
    /// Igual a <see cref="ObterPorIdEClinicaIdAsync"/>, mas também exige que a ração possa ser escolhida
    /// num registro novo: ativa e não oculta pela clínica. Se <paramref name="racaoAtualId"/> for a mesma ração
    /// já usada pelo registro, a checagem de ativa/oculta é dispensada (não trava edições de registros antigos).
    /// </summary>
    public async Task<ServiceResult> ObterSelecionavelAsync(int racaoId, int clinicaId, int? racaoAtualId = null)
    {
        var resultado = await ObterPorIdEClinicaIdAsync(racaoId, clinicaId);
        // A ração vem no corpo de outro registro (dieta, avaliação): inexistente é erro de validação (400), não 404
        if (!resultado.Succeeded) return Falha("A ração selecionada não existe ou não pertence a esta clínica.");
        if (racaoAtualId == racaoId) return resultado;

        var racao = (Racao)resultado.Data;
        if (!racao.IsAtivo) return Falha("A ração selecionada está inativa.");
        if (racao.ClinicaId == null && await _racaoRepository.GetOcultaAsync(racaoId, clinicaId) != null)
            return Falha("A ração selecionada está oculta para esta clínica.");

        return resultado;
    }

    public async Task<ServiceResult> ObterDtoPorIdAsync(int racaoId, int clinicaId)
    {
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(racaoId, clinicaId);
        if (!resultadoBusca.Succeeded) return resultadoBusca;

        var dto = _mapper.Map<GetRacoesDto>((Racao)resultadoBusca.Data);
        dto.IsOculta = await _racaoRepository.GetOcultaAsync(racaoId, clinicaId) != null;
        return new ServiceResult { Succeeded = true, Data = dto };
    }

    /// <summary>Lista paginada das rações disponíveis para a clínica, marcando as do sistema que ela ocultou.</summary>
    public async Task<PaginaResultado<GetRacoesDto>> ListarDaClinicaAsync(RacaoFiltro filtro, int clinicaId)
    {
        var pagina = await _racaoRepository.ListarDaClinicaAsync(filtro, clinicaId);
        var ocultas = await _racaoRepository.GetIdsOcultasAsync(clinicaId, pagina.Itens.Select(r => r.Id));

        return pagina.Converter(itens => itens.Select(r =>
        {
            var dto = _mapper.Map<GetRacoesDto>(r);
            dto.IsOculta = ocultas.Contains(r.Id);
            return dto;
        }));
    }

    public async Task<List<OpcaoDto>> ListarOpcoesDaClinicaAsync(int clinicaId, string? busca, EspecieAnimal? especie)
    {
        return await _racaoRepository.ListarOpcoesDaClinicaAsync(clinicaId, busca, especie);
    }

    public async Task<ServiceResult> CreateAsync(CadastrarRacaoDto dto, int clinicaId)
    {
        _logger.LogInformation("Iniciando criação de nova ração '{NomeRacao}' para a clínica ID {ClinicaId}.", dto.Nome, clinicaId);
        return await CadastrarAsync(dto, clinicaId);
    }

    /// <summary>
    /// Ração da própria clínica: atualiza e recalcula. Ração do sistema: cria uma cópia da clínica com as alterações.
    /// </summary>
    public async Task<ServiceResult> UpdateAsync(AtualizarRacaoDto dto, int clinicaId)
    {
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(dto.Id, clinicaId);
        if (!resultadoBusca.Succeeded) return resultadoBusca;

        var racao = (Racao)resultadoBusca.Data;
        if (racao.ClinicaId == null)
        {
            _logger.LogInformation("A ração {RacaoId} é do sistema. Criando uma cópia específica para a clínica {ClinicaId}.", racao.Id, clinicaId);
            return await CreateClinicSpecificCopyAsync(racao, dto, clinicaId);
        }

        return await AtualizarEntidadeAsync(racao, dto);
    }

    public async Task<ServiceResult> DeleteAsync(int racaoId, int clinicaId)
    {
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(racaoId, clinicaId);
        if (!resultadoBusca.Succeeded) return resultadoBusca;

        var racao = (Racao)resultadoBusca.Data;
        if (racao.ClinicaId == null)
        {
            _logger.LogWarning("A clínica {ClinicaId} tentou excluir a ração do sistema {RacaoId}.", clinicaId, racaoId);
            return Falha("Rações do sistema não podem ser excluídas pela clínica. Você pode ocultá-la.");
        }

        return await ExcluirEntidadeAsync(racao);
    }

    public async Task<ServiceResult> SetStatusDaClinicaAsync(int racaoId, int clinicaId, bool ativo)
    {
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(racaoId, clinicaId);
        if (!resultadoBusca.Succeeded) return resultadoBusca;

        var racao = (Racao)resultadoBusca.Data;
        if (racao.ClinicaId == null)
            return Falha("Rações do sistema são ativadas/desativadas pelo administrador. Você pode ocultá-la para esta clínica.");

        return await AplicarStatusAsync(racao, ativo);
    }

    /// <summary>Oculta (ou volta a mostrar) uma ração do sistema só para a clínica.</summary>
    public async Task<ServiceResult> SetOcultaAsync(int racaoId, int clinicaId, bool oculta)
    {
        var resultadoBusca = await ObterPorIdEClinicaIdAsync(racaoId, clinicaId);
        if (!resultadoBusca.Succeeded) return resultadoBusca;

        var racao = (Racao)resultadoBusca.Data;
        if (racao.ClinicaId != null)
            return Falha("Só rações do sistema podem ser ocultadas. Para rações da clínica, use desativar.");

        var registro = await _racaoRepository.GetOcultaAsync(racaoId, clinicaId);
        if (oculta && registro == null)
            await _racaoRepository.AddOcultaAsync(new RacaoOcultaClinica { RacaoId = racaoId, ClinicaId = clinicaId, DataOcultacao = DateTime.UtcNow });
        else if (!oculta && registro != null)
            _racaoRepository.RemoveOculta(registro);

        await _racaoRepository.SaveChangesAsync();
        return new ServiceResult { Succeeded = true, Message = oculta ? "Ração ocultada para esta clínica." : "Ração voltou a aparecer para esta clínica." };
    }

    #endregion

    #region Sistema (admin)

    public async Task<PaginaResultado<GetRacoesDto>> ListarSistemaAsync(RacaoFiltro filtro)
    {
        var pagina = await _racaoRepository.ListarSistemaAsync(filtro);
        return pagina.Converter(itens => _mapper.Map<IEnumerable<GetRacoesDto>>(itens));
    }

    public async Task<PaginaResultado<GetRacoesDto>> ListarCopiasAsync(RacaoFiltro filtro)
    {
        var pagina = await _racaoRepository.ListarCopiasAsync(filtro);
        return pagina.Converter(itens => _mapper.Map<IEnumerable<GetRacoesDto>>(itens));
    }

    public async Task<ServiceResult> ObterAsync(int racaoId)
    {
        var racao = await _racaoRepository.GetByIdAsync(racaoId);
        if (racao == null) return ServiceResult.NaoEncontrado("Ração não encontrada.");
        return new ServiceResult { Succeeded = true, Data = _mapper.Map<GetRacoesDto>(racao) };
    }

    public async Task<ServiceResult> CadastrarSistemaAsync(CadastrarRacaoDto dto)
    {
        _logger.LogInformation("Admin criando a ração do sistema '{NomeRacao}'.", dto.Nome);
        return await CadastrarAsync(dto, null);
    }

    /// <summary>Admin edita qualquer ração (do sistema ou de clínica) diretamente, sem criar cópia.</summary>
    public async Task<ServiceResult> EditarAsync(AtualizarRacaoDto dto)
    {
        var racao = await _racaoRepository.GetByIdAsync(dto.Id);
        if (racao == null) return ServiceResult.NaoEncontrado("Ração não encontrada.");
        return await AtualizarEntidadeAsync(racao, dto);
    }

    public async Task<ServiceResult> ExcluirAsync(int racaoId)
    {
        var racao = await _racaoRepository.GetByIdAsync(racaoId);
        if (racao == null) return ServiceResult.NaoEncontrado("Ração não encontrada.");
        return await ExcluirEntidadeAsync(racao);
    }

    public async Task<ServiceResult> SetStatusAsync(int racaoId, bool ativo)
    {
        var racao = await _racaoRepository.GetByIdAsync(racaoId);
        if (racao == null) return ServiceResult.NaoEncontrado("Ração não encontrada.");
        return await AplicarStatusAsync(racao, ativo);
    }

    #endregion

    #region Auxiliares

    private async Task<ServiceResult> CadastrarAsync(CadastrarRacaoDto dto, int? clinicaId)
    {
        var racao = _mapper.Map<Racao>(dto);
        racao.ClinicaId = clinicaId;
        racao.IsAtivo = true;
        racao.DataCadastro = DateTime.UtcNow;

        CalcularEnergiaMetabolizavel(racao);
        CalcularDistribuicaoEnergetica(racao);

        await _racaoRepository.AddAsync(racao);
        await _racaoRepository.SaveChangesAsync();

        _logger.LogInformation("Ração '{NomeRacao}' (ID: {RacaoId}) criada com sucesso.", racao.Nome, racao.Id);
        return new ServiceResult { Succeeded = true, Message = "Ração cadastrada com sucesso.", Data = _mapper.Map<GetRacoesDto>(racao) };
    }

    private async Task<ServiceResult> AtualizarEntidadeAsync(Racao racao, AtualizarRacaoDto dto)
    {
        _mapper.Map(dto, racao);
        CalcularEnergiaMetabolizavel(racao);
        CalcularDistribuicaoEnergetica(racao);
        await _racaoRepository.SaveChangesAsync();

        _logger.LogInformation("Ração ID {RacaoId} atualizada com sucesso.", racao.Id);
        return new ServiceResult { Succeeded = true, Message = "Ração atualizada com sucesso.", Data = _mapper.Map<GetRacoesDto>(racao) };
    }

    private async Task<ServiceResult> ExcluirEntidadeAsync(Racao racao)
    {
        if (await _racaoRepository.EmUsoAsync(racao.Id)) return ServiceResult.Conflito(MsgEmUso);

        _racaoRepository.Delete(racao);
        await _racaoRepository.SaveChangesAsync();
        _logger.LogInformation("Ração ID {RacaoId} excluída com sucesso.", racao.Id);
        return new ServiceResult { Succeeded = true, Message = "Ração excluída com sucesso." };
    }

    private async Task<ServiceResult> AplicarStatusAsync(Racao racao, bool ativo)
    {
        racao.IsAtivo = ativo;
        racao.DataDesativacao = ativo ? null : DateTime.UtcNow;
        await _racaoRepository.SaveChangesAsync();
        return new ServiceResult { Succeeded = true, Message = ativo ? "Ração ativada com sucesso." : "Ração desativada com sucesso." };
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
        copia.IsAtivo = true;
        copia.DataCadastro = DateTime.UtcNow;

        CalcularEnergiaMetabolizavel(copia);
        CalcularDistribuicaoEnergetica(copia);

        await _racaoRepository.AddAsync(copia);
        await _racaoRepository.SaveChangesAsync();

        _logger.LogInformation("Cópia da ração do sistema {RacaoSistemaId} criada para a clínica {ClinicaId} com ID {RacaoId}.", racaoSistema.Id, clinicaId, copia.Id);
        return new ServiceResult { Succeeded = true, Message = "Ração do sistema copiada para a clínica com as alterações.", Data = _mapper.Map<GetRacoesDto>(copia) };
    }

    private static ServiceResult Falha(string mensagem) => new() { Succeeded = false, Message = mensagem };

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