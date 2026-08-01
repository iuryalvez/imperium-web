using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Services
{
    /// <summary>
    /// Serviço responsável pelas regras de negócio e orquestração das operações de Acompanhamento Individualizado.
    /// </summary>
    public class AcompanhamentoService : IAcompanhamentoService
    {
        private readonly IAcompanhamentoRepository _acompanhamentoRepository;
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly IRacaoService _racaoService;
        private readonly IDietaService _dietaService;
        private readonly IAnimalService _animalService;
        private readonly IMapper _mapper;
        private readonly ILogger<AcompanhamentoService> _logger;

        /// <summary>
        /// Inicializa uma nova instância do serviço de Acompanhamento.
        /// </summary>
        public AcompanhamentoService(IAcompanhamentoRepository acompanhamentoRepository,
                                     IAvaliacaoService avaliacaoService,
                                     IRacaoService racaoService,
                                     IDietaService dietaService,
                                     IAnimalService animalService,
                                     IMapper mapper,
                                     ILogger<AcompanhamentoService> logger)
        {
            _acompanhamentoRepository = acompanhamentoRepository;
            _avaliacaoService = avaliacaoService;
            _racaoService = racaoService;
            _dietaService = dietaService;
            _animalService = animalService;
            _mapper = mapper;
            _logger = logger;
        }

        #region Métodos de Leitura

        /// <summary>
        /// Obtém a entidade Acompanhamento pelo seu ID, validando se pertence à clínica do usuário.
        /// </summary>
        /// <param name="acompanhamentoId">ID do acompanhamento.</param>
        /// <param name="clinicaId">ID da clínica para validação de acesso.</param>
        /// <returns>ServiceResult contendo a entidade Acompanhamento ou uma mensagem de erro.</returns>
        public async Task<ServiceResult> ObterPorIdAsync(int acompanhamentoId, int clinicaId)
        {
            _logger.LogInformation("Buscando acompanhamento {AcompanhamentoId} para a clínica {ClinicaId}.", acompanhamentoId, clinicaId);
            var acompanhamento = await _acompanhamentoRepository.GetByIdAsync(acompanhamentoId);
            if (acompanhamento == null)
            {
                _logger.LogWarning("Acompanhamento {AcompanhamentoId} não encontrado no banco de dados.", acompanhamentoId);

                return new ServiceResult { Succeeded = false, Message = "Acompanhamento não encontrado." };
            }

            if (!await _animalService.AnimalExisteNaClinicaAsync(acompanhamento.AnimalId, clinicaId))
            {
                _logger.LogWarning("Acesso negado: Acompanhamento {AcompanhamentoId} não pertence à clínica {ClinicaId}.", acompanhamentoId, clinicaId);

                return new ServiceResult { Succeeded = false, Message = "Acompanhamento não encontrado nesta clínica." };
            }

            _logger.LogInformation("Acompanhamento {AcompanhamentoId} encontrado com sucesso.", acompanhamentoId);

            return new ServiceResult { Succeeded = true, Message = "Acompanhamento encontrado.", Data = acompanhamento };
        }

        /// <summary>
        /// Obtém a entidade Acompanhamento pelo ID do animal, validando o acesso pela clínica.
        /// </summary>
        /// <param name="animalId">ID do animal.</param>
        /// <param name="clinicaId">ID da clínica para validação.</param>
        /// <returns>ServiceResult contendo a entidade Acompanhamento ou uma mensagem de erro.</returns>
        public async Task<ServiceResult> ObterPorAnimalIdAsync(int animalId, int clinicaId)
        {
            _logger.LogInformation("Buscando acompanhamento pelo ID do animal {AnimalId} na clínica {ClinicaId}.", animalId, clinicaId);
            if (!await _animalService.AnimalExisteNaClinicaAsync(animalId, clinicaId))
            {
                _logger.LogWarning("Acesso negado: Animal {AnimalId} não pertence à clínica {ClinicaId}.", animalId, clinicaId);

                return new ServiceResult { Succeeded = false, Message = "O animal especificado não pertence a esta clínica." };
            }

            var acompanhamento = await _acompanhamentoRepository.GetByAnimalIdAsync(animalId);
            if (acompanhamento == null)
            {
                _logger.LogWarning("Nenhum acompanhamento encontrado para o animal {AnimalId}.", animalId);

                return new ServiceResult { Succeeded = false, Message = "Nenhum acompanhamento encontrado para este animal." };
            }


            return new ServiceResult { Succeeded = true, Message = "Acompanhamento encontrado.", Data = acompanhamento };
        }

        /// <summary>
        /// Obtém o histórico completo de um animal (incluindo relatórios e dietas) a partir do ID do acompanhamento.
        /// </summary>
        /// <param name="acompanhamentoId">ID do acompanhamento.</param>
        /// <param name="clinicaId">ID da clínica para validação.</param>
        /// <returns>ServiceResult contendo o DTO do histórico completo do animal ou uma mensagem de erro.</returns>
        public async Task<ServiceResult> ObterHistoricoCompletoAnimalAsync(int acompanhamentoId, int clinicaId)
        {
            var resultadoBusca = await ObterPorIdAsync(acompanhamentoId, clinicaId);
            if (!resultadoBusca.Succeeded)
            {
                return resultadoBusca;
            }
            var acompanhamento = (Acompanhamento)resultadoBusca.Data;

            _logger.LogInformation("Delegando para AnimalService a busca do histórico completo do animal {AnimalId}.", acompanhamento.AnimalId);
            return await _animalService.ObterHistoricoCompletoAnimalAsync(acompanhamento.AnimalId, clinicaId);
        }

        #endregion

        #region Métodos de Escrita

        /// <summary>
        /// Cria um novo acompanhamento para um animal.
        /// </summary>
        /// <param name="animalId">ID do animal para o qual o acompanhamento será criado.</param>
        /// <returns>ServiceResult indicando sucesso ou falha na operação.</returns>
        public async Task<ServiceResult> CreateAsync(int animalId)
        {
            _logger.LogInformation("Tentando criar novo acompanhamento para o animal {AnimalId}.", animalId);
            if (await _acompanhamentoRepository.ExistsByAnimalIdAsync(animalId))
            {
                _logger.LogWarning("Criação de acompanhamento falhou: Já existe um para o animal {AnimalId}.", animalId);

                return new ServiceResult { Succeeded = false, Message = "Já existe um acompanhamento para este animal." };
            }

            var acompanhamento = new Acompanhamento
            {
                AnimalId = animalId,
                DataInicio = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _acompanhamentoRepository.AddAsync(acompanhamento);
            await _acompanhamentoRepository.SaveChangesAsync();

            _logger.LogInformation("Acompanhamento {AcompanhamentoId} criado com sucesso para o animal {AnimalId}.", acompanhamento.Id, animalId);

            return new ServiceResult { Succeeded = true, Message = "Acompanhamento criado com sucesso.", Data = new { AcompanhamentoId = acompanhamento.Id } };
        }

        /// <summary>
        /// Define uma data de término para um acompanhamento ativo.
        /// </summary>
        /// <param name="acompanhamentoId">ID do acompanhamento a ser encerrado.</param>
        /// <param name="clinicaId">ID da clínica para validação.</param>
        /// <returns>ServiceResult indicando sucesso ou falha.</returns>
        public async Task<ServiceResult> EncerrarAcompanhamentoAsync(int acompanhamentoId, int clinicaId)
        {
            _logger.LogInformation("Tentando encerrar o acompanhamento {AcompanhamentoId} na clínica {ClinicaId}.", acompanhamentoId, clinicaId);
            var resultadoBusca = await ObterPorIdAsync(acompanhamentoId, clinicaId);
            if (!resultadoBusca.Succeeded)
            {
                return resultadoBusca;
            }
            var acompanhamento = (Acompanhamento)resultadoBusca.Data;

            if (acompanhamento.DataFim.HasValue)
            {
                _logger.LogWarning("Falha ao encerrar acompanhamento {AcompanhamentoId}: já estava encerrado em {DataFim}.", acompanhamentoId, acompanhamento.DataFim.Value);

                return new ServiceResult { Succeeded = false, Message = "Este acompanhamento já foi encerrado." };
            }

            acompanhamento.DataFim = DateOnly.FromDateTime(DateTime.UtcNow);
            await _acompanhamentoRepository.UpdateAsync(acompanhamento);
            await _acompanhamentoRepository.SaveChangesAsync();

            _logger.LogInformation("Acompanhamento {AcompanhamentoId} encerrado com sucesso.", acompanhamentoId);

            return new ServiceResult { Succeeded = true, Message = "Acompanhamento encerrado com sucesso." };
        }

        /// <summary>
        /// Orquestra a criação ou atualização de uma dieta personalizada para um relatório específico.
        /// </summary>
        /// <param name="acompanhamentoId">ID do acompanhamento pai.</param>
        /// <param name="dto">Dados para a criação da dieta.</param>
        /// <param name="clinicaId">ID da clínica para validação de acesso.</param>
        /// <returns>ServiceResult contendo o DTO da dieta criada ou uma mensagem de erro.</returns>
        public async Task<ServiceResult> CriarDietaParaAvaliacaoAsync(int acompanhamentoId, CreateDietaDto dto, int clinicaId)
        {
            _logger.LogInformation("Iniciando orquestração para criar dieta para o relatório {AvaliacaoId} no acompanhamento {AcompanhamentoId}.", dto.AvaliacaoId, acompanhamentoId);

            try
            {
                var resultadoAcompanhamento = await ObterPorIdAsync(acompanhamentoId, clinicaId);
                if (!resultadoAcompanhamento.Succeeded) return resultadoAcompanhamento;
                var acompanhamento = (Acompanhamento)resultadoAcompanhamento.Data;

                var animalResult = await _animalService.ObterPorIdAsync(acompanhamento.AnimalId);
                if (!animalResult.Succeeded)
                {
                    _logger.LogError("Inconsistência de dados: Acompanhamento {AcompanhamentoId} encontrado, mas o animal associado {AnimalId} não.", acompanhamentoId, acompanhamento.AnimalId);

                    return new ServiceResult { Succeeded = false, Message = "Animal associado ao acompanhamento não foi encontrado." };
                }
                var animal = (Animal)animalResult.Data;

                _logger.LogDebug("Animal {AnimalId} obtido com sucesso.", animal.Id);

                var resultadoAvaliacao = await _avaliacaoService.ObterPorIdEIdAcompanhamentoAsync(dto.AvaliacaoId, acompanhamentoId, clinicaId);
                if (!resultadoAvaliacao.Succeeded)
                {
                    _logger.LogWarning("Falha ao obter relatório {AvaliacaoId}. Motivo: {Motivo}", dto.AvaliacaoId, resultadoAvaliacao.Message);
                    return resultadoAvaliacao;
                }
                var avaliacao = (Avaliacao)resultadoAvaliacao.Data;
                _logger.LogDebug("Avaliação {AvaliacaoId} obtida com sucesso.", avaliacao.Id);

                var resultadoRacao = await _racaoService.ObterPorIdEClinicaIdAsync(dto.RacaoId, clinicaId);
                if (!resultadoRacao.Succeeded)
                {
                    _logger.LogWarning("Falha ao obter ração {RacaoId}. Motivo: {Motivo}", dto.RacaoId, resultadoRacao.Message);
                    return resultadoRacao;
                }
                var racao = (Racao)resultadoRacao.Data;
                _logger.LogDebug("Ração '{NomeRacao}' (ID: {RacaoId}) obtida com sucesso.", racao.Nome, racao.Id);

                _logger.LogInformation("Sincronizando entidade 'animal' em memória com dados do CreateDietaDto.");
                _logger.LogDebug("Valores do DTO para sincronização -> PesoAtual: {PesoDto}, Nro Filhotes: {FilhotesDto}, Semana Lactação: {SemanaLactacaoDto}",
                    dto.PesoAtual, dto.IsLactanteNumeroFilhotes, dto.SemanaLactacao);

                animal.Peso = dto.PesoAtual ?? animal.Peso;
                animal.PesoIdeal = dto.PesoObjetivo ?? animal.PesoIdeal;

                if (dto.TipoDieta == TipoDieta.Crescimento)
                {
                    animal.PesoMaximo = dto.PesoObjetivo ?? animal.PesoMaximo;
                    animal.PesoMinimo = dto.PesoObjetivo ?? animal.PesoMinimo;
                }

                // Atualiza os dados de lactação se o tipo de dieta for Lactação
                if (dto.TipoDieta == TipoDieta.Lactacao)
                {
                    animal.IsLactanteNumeroFilhotes = dto.IsLactanteNumeroFilhotes ?? animal.IsLactanteNumeroFilhotes;
                    animal.SemanaLactacao = dto.SemanaLactacao ?? animal.SemanaLactacao;
                }

                _logger.LogDebug("Animal (após atualização) pronto para cálculo. ID: {AnimalId}, Peso: {Peso}, Nro Filhotes: {NumFilhotes}, Semana Lactação: {SemanaLactacao}",
            animal.Id, animal.Peso, animal.IsLactanteNumeroFilhotes, animal.SemanaLactacao);

                _logger.LogInformation("Delegando cálculo da dieta para o DietaService.");
                var dieta = await _dietaService.CalcularOuAtualizarDietaAsync(animal, avaliacao, racao, dto);
                var dietaDto = _mapper.Map<GetDietaDto>(dieta);

                _logger.LogInformation("Dieta para o relatório {AvaliacaoId} calculada com sucesso.", dto.AvaliacaoId);

                return new ServiceResult { Succeeded = true, Message = "Dieta calculada e salva com sucesso.", Data = dietaDto };
            }
            catch (InvalidCastException ex)
            {
                _logger.LogCritical(ex, "Falha de conversão de tipo (cast) durante a criação da dieta. Verifique os tipos de retorno dos serviços.");

                return new ServiceResult { Succeeded = false, Message = "Erro de sistema: Inconsistência nos tipos de dados." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar dieta para o relatório {AvaliacaoId}.", dto.AvaliacaoId);

                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro inesperado ao calcular a dieta." };
            }
        }

        public async Task<ServiceResult> UpdateDietaAsync(int dietaId, UpdateDietaDto dto, int clinicaId)
        {
            _logger.LogInformation("Iniciando atualização da dieta {DietaId}.", dietaId);

            try
            {
                var resultadoRacao = await _racaoService.ObterPorIdEClinicaIdAsync(dto.RacaoId, clinicaId);
                if (!resultadoRacao.Succeeded)
                {
                    return resultadoRacao;
                }
                var racao = (Racao)resultadoRacao.Data;

                var resultadoBusca = await _dietaService.UpdateAsync(dietaId, dto, clinicaId, racao);
                if (!resultadoBusca.Succeeded)
                {
                    _logger.LogWarning("Falha ao atualizar a dieta {DietaId}. Motivo: {Motivo}", dietaId, resultadoBusca.Message);
                    return resultadoBusca;
                }

                _logger.LogInformation("Dieta {DietaId} atualizada com sucesso.", dietaId);
                return resultadoBusca;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar a dieta {DietaId}.", dietaId);
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro inesperado ao atualizar a dieta." };
            }
        }

        #endregion

        public async Task<ServiceResult> DeleteDietaAsync(int dietaId, int clinicaId)
        {
            _logger.LogInformation("Iniciando processo de exclusão da dieta {DietaId}.", dietaId);

            try
            {
                await _dietaService.DeleteAsync(dietaId, clinicaId);
                _logger.LogInformation("Dieta {DietaId} excluída com sucesso.", dietaId);
                return new ServiceResult { Succeeded = true, Message = "Dieta excluída com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir a dieta {DietaId}.", dietaId);
                return new ServiceResult { Succeeded = false, Message = "Erro ao excluir a dieta." };
            }
        }
    }
}