using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;

namespace backend.Domain.SaudeAnimal.Services
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
            // 1. Validar a ração primeiro
            if (racao.EnergiaMetabolizavel == null || racao.EnergiaMetabolizavel <= 0)
            {
                _logger.LogError("Ração ID {RacaoId} não possui energia metabolizável válida.", racao.Id);
                throw new InvalidOperationException("A ração selecionada não possui energia metabolizável válida.");
            }

            // 2. BUSCAR OU CRIAR A DIETA REAL
            var dietaExistente = await _dietaRepository.FindByRelatorioIdAsync(relatorio.Id);
            if (dietaExistente == null)
            {
                _logger.LogInformation("Nenhuma dieta encontrada para o relatório {RelatorioId}. Criando nova.", relatorio.Id);
                dietaExistente = new DietaPersonalizada();
                await _dietaRepository.AddAsync(dietaExistente);
            }
            else
            {
                _logger.LogInformation("Dieta ID {DietaId} encontrada para o relatório {RelatorioId}. Atualizando.", dietaExistente.Id, relatorio.Id);
            }

            // 3. Atualizar propriedades básicas para que o cálculo saiba o que fazer
            dietaExistente.TipoDieta = dto.TipoDieta;
            dietaExistente.RelatorioId = relatorio.Id;
            dietaExistente.RacaoId = racao.Id;

            // 4. CHAMAR O CÁLCULO PASSANDO A ENTIDADE DO BANCO
            double necessidadeKcal = _calculoService.CalcularNecessidadeTotal(animal, dietaExistente);

            // 5. Calcular gramas diárias
            double quantidadeGramas = necessidadeKcal / racao.EnergiaMetabolizavel.Value;

            // 6. Atualizar os demais campos vindos do DTO e do cálculo
            dietaExistente.PesoAtual = dto.PesoAtual ?? animal.Peso;
            dietaExistente.PesoObjetivo = dto.PesoObjetivo;
            dietaExistente.PesoIdeal = dto.PesoIdeal;
            dietaExistente.DuracaoEstimada = dto.DuracaoEstimada;
            dietaExistente.Observacoes = dto.Observacoes;
            dietaExistente.FaseVida = animal.FaseVida;
            dietaExistente.EnergiaMetabolizavel = Math.Round(necessidadeKcal, 2);
            dietaExistente.QtdAlimento = Math.Round(quantidadeGramas, 2);
            dietaExistente.DataCriacao = DateOnly.FromDateTime(DateTime.UtcNow);

            // 7. Salvar no banco (Agora os dados de transparência estão dentro de dietaExistente!)
            await _dietaRepository.SaveChangesAsync();
            _logger.LogInformation("Dieta ID {DietaId} salva com sucesso para o animal {AnimalId}.", dietaExistente.Id, animal.Id);

            return dietaExistente;
        }

        public async Task<GetDietaDto> GetByIdAsync(int dietaId, int clinicaId)
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

        public async Task<ServiceResult> UpdateAsync(int dietaId, UpdateDietaDto dto, int clinicaId, Racao racao)
        {
            _logger.LogInformation("Iniciando atualização e recálculo da dieta ID {DietaId}.", dietaId);

            try
            {
                var dietaExistente = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
                if (dietaExistente?.Relatorio?.Consulta?.ClinicaId != clinicaId)
                {
                    _logger.LogWarning("Dieta {DietaId} não encontrada ou acesso negado.", dietaId);
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                var animal = dietaExistente.Relatorio.Consulta.Animal;
                var relatorio = dietaExistente.Relatorio;

                // Prepara o DTO de criação para delegar ao método principal
                var createDto = _mapper.Map<CreateDietaDto>(dto);
                createDto.RelatorioId = relatorio.Id;

                // Sincroniza o animal em memória se houver novas propriedades no DTO
                animal.Peso = createDto.PesoAtual ?? animal.Peso;
                animal.PesoIdeal = createDto.PesoIdeal ?? animal.PesoIdeal;
                if (createDto.TipoDieta == TipoDieta.Lactacao)
                {
                    animal.IsLactanteNumeroFilhotes = createDto.IsLactanteNumeroFilhotes ?? animal.IsLactanteNumeroFilhotes;
                    animal.SemanaLactacao = createDto.SemanaLactacao ?? animal.SemanaLactacao;
                }

                // Delega para o método principal (que agora atualiza a entidade corretamente)
                var dietaAtualizada = await CalcularOuAtualizarDietaAsync(animal, relatorio, racao, createDto);
                var dietaDto = _mapper.Map<GetDietaDto>(dietaAtualizada);

                return new ServiceResult { Succeeded = true, Message = "Dieta atualizada com sucesso.", Data = dietaDto };
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