using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.SaudeAnimal.Services;

namespace backend.Domain.SaudeAnimal.Impl
{
    public class DietaService : IDietaService
    {
        private readonly IDietaRepository _dietaRepository;
        private readonly IMapper _mapper;
        private readonly ICalculoNecessidadeEnergetica _calculoService;
        private readonly ILogger<DietaService> _logger;

        public DietaService(IDietaRepository dietaRepository,
                            ICalculoNecessidadeEnergetica calculoService,
                            ILogger<DietaService> logger,
                            IMapper mapper)
        {
            _dietaRepository = dietaRepository;
            _calculoService = calculoService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<DietaPersonalizada> CalcularOuAtualizarDietaAsync(
            Animal animal,
            Relatorio relatorio,
            Racao racao,
            CreateDietaDto dto)
        {
            // 1. Preparar o objeto para o cálculo
            //    O serviço de cálculo precisa de um objeto 'DietaPersonalizada'
            var dietaParaCalculo = new DietaPersonalizada
            {
                TipoDieta = dto.TipoDieta,
                MultiplicadorEnergeticoCustom = dto.MultiplicadorEnergeticoCustom,
                PotenciaCustom = dto.PotenciaCustom
            };

            // 2. Chamar o serviço de cálculo para obter a necessidade energética diária (kcal)
            double necessidadeKcal = _calculoService.CalcularNecessidadeTotal(animal, dietaParaCalculo);

            _logger.LogDebug("Necessidade energética diária calculada: {NecessidadeKcal} kcal/dia.", necessidadeKcal);

            // 3. Validar a ração
            if (racao.EnergiaMetabolizavel == null || racao.EnergiaMetabolizavel <= 0)
            {
                _logger.LogError("Ração ID {RacaoId} não possui um valor de energia metabolizável válido para o cálculo.", racao.Id);
                throw new InvalidOperationException("A ração selecionada não possui um valor de energia metabolizável válido.");
            }

            // 4. Calcular a quantidade de alimento em gramas;
            double quantidadeGramas = necessidadeKcal / racao.EnergiaMetabolizavel.Value;
            _logger.LogDebug("Quantidade de alimento calculada: {QuantidadeGramas} g/dia.", quantidadeGramas);

            // 5. Buscar por uma dieta existente para este relatório (lógica de Upsert)
            var dietaExistente = await _dietaRepository.FindByRelatorioIdAsync(relatorio.Id);

            if (dietaExistente == null)
            {
                _logger.LogInformation("Nenhuma dieta encontrada para o relatório ID {RelatorioId}. Criando uma nova.", relatorio.Id);
                dietaExistente = new DietaPersonalizada();
                await _dietaRepository.AddAsync(dietaExistente);
            }
            else
            {
                _logger.LogInformation("Dieta ID {DietaId} encontrada para o relatório ID {RelatorioId}. A dieta será atualizada.", dietaExistente.Id, relatorio.Id);
            }

            // 6. Mapear todos os dados do DTO e dos cálculos para a entidade
            dietaExistente.RelatorioId = relatorio.Id;
            dietaExistente.RacaoId = racao.Id;
            dietaExistente.TipoDieta = dto.TipoDieta;

            // Dados do DTO
            dietaExistente.PesoAtual = dto.PesoAtual ?? animal.Peso; // Pega do DTO ou do animal como fallback
            dietaExistente.PesoObjetivo = dto.PesoObjetivo;
            dietaExistente.PesoIdeal = dto.PesoIdeal;
            dietaExistente.DuracaoEstimada = dto.DuracaoEstimada;
            dietaExistente.Observacoes = dto.Observacoes;
            dietaExistente.FaseVida = animal.FaseVida; // Captura a fase de vida do animal no momento da criação da dieta

            // Customizações do cálculo
            dietaExistente.MultiplicadorEnergeticoCustom = dto.MultiplicadorEnergeticoCustom;
            dietaExistente.PotenciaCustom = dto.PotenciaCustom;

            // Resultados dos cálculos
            dietaExistente.EnergiaMetabolizavel = Math.Round(necessidadeKcal, 2);
            dietaExistente.QtdAlimento = Math.Round(quantidadeGramas, 2); // Quantidade em gramas/dia

            // Metadados
            dietaExistente.DataCriacao = DateOnly.FromDateTime(DateTime.UtcNow);
            // dietaExistente.CriadoPor = ... // Implementar lógica para pegar usuário logado

            // 7. Salvar as alterações no banco de dados
            await _dietaRepository.SaveChangesAsync();
            _logger.LogInformation("Dieta ID {DietaId} foi salva com sucesso para o animal ID {AnimalId}.", dietaExistente.Id, animal.Id);

            return dietaExistente;
        }

        public async Task<GetDietaDto?> GetByIdAsync(int dietaId, int clinicaId)
        {
            _logger.LogInformation("Buscando dieta por ID {DietaId} para a clínica ID {ClinicaId}.", dietaId, clinicaId);
            try
            {
                var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);

                // Verificação de segurança
                if (dieta?.Relatorio?.Consulta?.ClinicaId != clinicaId)
                {
                    _logger.LogWarning("Acesso negado ou dieta não encontrada para o ID {DietaId} e clínica ID {ClinicaId}.", dietaId, clinicaId);
                    return null;
                }

                _logger.LogInformation("Dieta ID {DietaId} encontrada com sucesso.", dietaId);
                return _mapper.Map<GetDietaDto>(dieta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar a dieta por ID {DietaId}.", dietaId);
                throw;
            }
        }

        public async Task<IEnumerable<GetDietaDto>> GetAllByAcompanhamentoAsync(int acompanhamentoId, int clinicaId)
        {
            _logger.LogInformation("Buscando todas as dietas para o acompanhamento ID {AcompanhamentoId} na clínica ID {ClinicaId}.", acompanhamentoId, clinicaId);
            try
            {   
                var dietas = await _dietaRepository.GetAllByAcompanhamentoIdAsync(acompanhamentoId);
                _logger.LogInformation("Encontradas {Count} dietas para o acompanhamento ID {AcompanhamentoId}.", dietas.Count(), acompanhamentoId);
                
                return _mapper.Map<IEnumerable<GetDietaDto>>(dietas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dietas para o acompanhamento ID {AcompanhamentoId}.", acompanhamentoId);
                throw;
            }
        }

        public async Task<ServiceResult> UpdateAsync(int dietaId, UpdateDietaDto dto, int clinicaId)
        {
            _logger.LogInformation("Tentando atualizar a dieta ID {DietaId} para a clínica ID {ClinicaId}.", dietaId, clinicaId);
            try
            {
                var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
                if (dieta?.Relatorio?.Consulta?.ClinicaId != clinicaId)
                {
                    _logger.LogWarning("Falha ao atualizar: Dieta ID {DietaId} não encontrada ou acesso negado para a clínica ID {ClinicaId}.", dietaId, clinicaId);
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                _mapper.Map(dto, dieta);
                await _dietaRepository.SaveChangesAsync();

                _logger.LogInformation("Dieta ID {DietaId} atualizada com sucesso.", dietaId);
                return new ServiceResult { Succeeded = true, Message = "Dieta atualizada com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a dieta ID {DietaId}.", dietaId);
                throw;
            }
        }

        public async Task<ServiceResult> DeleteAsync(int dietaId, int clinicaId)
        {
            _logger.LogInformation("Tentando excluir a dieta ID {DietaId} da clínica ID {ClinicaId}.", dietaId, clinicaId);
            try
            {
                var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
                if (dieta?.Relatorio?.Consulta?.ClinicaId != clinicaId)
                {
                    _logger.LogWarning("Falha ao excluir: Dieta ID {DietaId} não encontrada ou acesso negado para a clínica ID {ClinicaId}.", dietaId, clinicaId);
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                _dietaRepository.Delete(dieta);
                await _dietaRepository.SaveChangesAsync();

                _logger.LogInformation("Dieta ID {DietaId} excluída com sucesso.", dietaId);
                return new ServiceResult { Succeeded = true, Message = "Dieta excluída com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir a dieta ID {DietaId}.", dietaId);
                throw;
            }
        }
    }
}