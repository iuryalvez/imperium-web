using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
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

        public async Task<Dieta> CalcularOuAtualizarDietaAsync(
            Animal animal,
            Avaliacao avaliacao,
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
            var dietaExistente = await _dietaRepository.FindByAvaliacaoIdAsync(avaliacao.Id);
            if (dietaExistente == null)
            {
                _logger.LogInformation("Nenhuma dieta encontrada para o relatório {AvaliacaoId}. Criando nova.", avaliacao.Id);
                dietaExistente = new Dieta();
                await _dietaRepository.AddAsync(dietaExistente);
            }
            else
            {
                _logger.LogInformation("Dieta ID {DietaId} encontrada para o relatório {AvaliacaoId}. Atualizando.", dietaExistente.Id, avaliacao.Id);
            }

            // 3. Atualizar propriedades básicas para que o cálculo saiba o que fazer
            dietaExistente.TipoDieta = dto.TipoDieta;
            dietaExistente.AvaliacaoId = avaliacao.Id;
            dietaExistente.RacaoId = racao.Id;
            dietaExistente.PerfilMetabolico = dto.PerfilMetabolico;
            dietaExistente.MultiplicadorEnergetico = dto.MultiplicadorEnergetico;
            dietaExistente.Potencia = dto.Potencia;
            dietaExistente.HasAdicionalGestacaoCao = dto.HasAdicionalGestacaoCao ?? false;
            dietaExistente.NumeroFilhotes = dto.NumeroFilhotes;
            dietaExistente.SemanaLactacao = dto.SemanaLactacao;
            dietaExistente.SemanaGestacao = dto.SemanaGestacao;
            dietaExistente.NivelRestricao = dto.NivelRestricao;
            dietaExistente.PesoAtual = dto.PesoAtual ?? animal.Peso;
            dietaExistente.PesoObjetivo = dto.PesoObjetivo;
            dietaExistente.PesoIdeal = dto.PesoIdeal;
            dietaExistente.DuracaoEstimada = dto.DuracaoEstimada;
            dietaExistente.Observacoes = dto.Observacoes;
            dietaExistente.FaseVida = animal.FaseVida;

            // 4. CHAMAR O CÁLCULO PASSANDO A ENTIDADE DO BANCO
            double necessidadeKcal = _calculoService.CalcularNecessidadeTotal(animal, dietaExistente);

            // 5. Calcular gramas diárias
            double quantidadeGramas = necessidadeKcal / racao.EnergiaMetabolizavel.Value;

            // 6. Atualizar os campos resultantes do cálculo
            dietaExistente.EnergiaMetabolizavel = Math.Round(necessidadeKcal, 2);
            dietaExistente.QtdAlimento = Math.Round(quantidadeGramas, 2);
            dietaExistente.DataCriacao = DateOnly.FromDateTime(DateTime.UtcNow);

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
                if (dieta?.Avaliacao?.Consulta?.ClinicaId != clinicaId)
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
                if (dietaExistente?.Avaliacao?.Consulta?.ClinicaId != clinicaId)
                {
                    _logger.LogWarning("Dieta {DietaId} não encontrada ou acesso negado.", dietaId);
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                var animal = dietaExistente.Avaliacao.Consulta.Animal;
                var avaliacao = dietaExistente.Avaliacao;

                if (dietaExistente.Status == backend.Domain.SaudeAnimal.Enums.StatusDieta.Concluida)
                {
                    return new ServiceResult { Succeeded = false, Message = "Esta dieta está concluída e é somente leitura." };
                }

                if (dietaExistente.Status == backend.Domain.SaudeAnimal.Enums.StatusDieta.EmAndamento && dto.TipoDieta != dietaExistente.TipoDieta)
                {
                    return new ServiceResult { Succeeded = false, Message = "Não é possível alterar o tipo de uma dieta em andamento." };
                }
                
                if (dto.TipoDieta == TipoDieta.Emagrecimento)
                {
                    double pesoParaValidar = dto.PesoAtual ?? animal.Peso ?? 0;
                    if (!dto.PesoObjetivo.HasValue || dto.PesoObjetivo >= pesoParaValidar)
                    {
                        return new ServiceResult { Succeeded = false, Message = "Para emagrecimento, o peso alvo deve ser menor que o peso atual." };
                    }
                    if (!dto.NivelRestricao.HasValue)
                    {
                        return new ServiceResult { Succeeded = false, Message = "Para emagrecimento, o nível de restrição é obrigatório." };
                    }
                }

                if (dto.TipoDieta == TipoDieta.Gestacao && animal.Especie == backend.Domain.AtendimentoAnimal.Enums.EspecieAnimal.Gato)
                {
                    if (!dto.SemanaGestacao.HasValue || dto.SemanaGestacao < 1 || dto.SemanaGestacao > 15)
                    {
                        return new ServiceResult { Succeeded = false, Message = "Para gestação de gatas, a semana de gestação é obrigatória e deve estar entre 1 e 15." };
                    }
                }

                // Prepara o DTO de criação para delegar ao método principal
                var createDto = _mapper.Map<CreateDietaDto>(dto);
                createDto.AvaliacaoId = avaliacao.Id;

                // Sincroniza o animal em memória se houver novas propriedades no DTO
                animal.Peso = createDto.PesoAtual ?? animal.Peso;
                animal.PesoIdeal = createDto.PesoIdeal ?? animal.PesoIdeal;
                if (createDto.TipoDieta == TipoDieta.Crescimento)
                {
                    animal.PesoMaximo = createDto.PesoObjetivo ?? animal.PesoMaximo;
                    animal.PesoMinimo = createDto.PesoObjetivo ?? animal.PesoMinimo;
                }
                if (createDto.TipoDieta == TipoDieta.Lactacao)
                {
                    animal.IsLactanteNumeroFilhotes = createDto.NumeroFilhotes ?? animal.IsLactanteNumeroFilhotes;
                    animal.SemanaLactacao = createDto.SemanaLactacao ?? animal.SemanaLactacao;
                }
                if (createDto.TipoDieta == TipoDieta.Gestacao)
                {
                    animal.SemanaGestacao = createDto.SemanaGestacao ?? animal.SemanaGestacao;
                }
                if (createDto.TipoDieta == TipoDieta.Emagrecimento)
                {
                    animal.PesoIdeal = createDto.PesoObjetivo ?? animal.PesoIdeal;
                }

                // Delega para o método principal (que agora atualiza a entidade corretamente)
                var dietaAtualizada = await CalcularOuAtualizarDietaAsync(animal, avaliacao, racao, createDto);
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
                if (dieta?.Avaliacao?.Consulta?.ClinicaId != clinicaId)
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

        public async Task<ServiceResult> IniciarAsync(int dietaId, int clinicaId)
        {
            _logger.LogInformation("Tentando iniciar a dieta ID {DietaId} da clínica ID {ClinicaId}.", dietaId, clinicaId);
            try
            {
                var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
                if (dieta?.Avaliacao?.Consulta?.ClinicaId != clinicaId)
                {
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                if (dieta.Status != backend.Domain.SaudeAnimal.Enums.StatusDieta.Aberta)
                {
                    return new ServiceResult { Succeeded = false, Message = "Apenas dietas 'Abertas' podem ser iniciadas." };
                }

                // Conclui dieta em andamento do mesmo acompanhamento
                int acompanhamentoId = dieta.Avaliacao!.AcompanhamentoId;
                var dietaAtiva = await _dietaRepository.FindEmAndamentoByAcompanhamentoIdAsync(acompanhamentoId);
                if (dietaAtiva != null)
                {
                    dietaAtiva.Status = backend.Domain.SaudeAnimal.Enums.StatusDieta.Concluida;
                    dietaAtiva.DataConclusao = DateOnly.FromDateTime(DateTime.UtcNow);
                    await _dietaRepository.UpdateAsync(dietaAtiva);
                }

                dieta.Status = backend.Domain.SaudeAnimal.Enums.StatusDieta.EmAndamento;
                dieta.DataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
                await _dietaRepository.UpdateAsync(dieta);
                await _dietaRepository.SaveChangesAsync();

                return new ServiceResult { Succeeded = true, Message = "Dieta iniciada com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar a dieta ID {DietaId}.", dietaId);
                throw;
            }
        }

        public async Task<ServiceResult> ConcluirAsync(int dietaId, int clinicaId)
        {
            _logger.LogInformation("Tentando concluir a dieta ID {DietaId} da clínica ID {ClinicaId}.", dietaId, clinicaId);
            try
            {
                var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
                if (dieta?.Avaliacao?.Consulta?.ClinicaId != clinicaId)
                {
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                if (dieta.Status != backend.Domain.SaudeAnimal.Enums.StatusDieta.EmAndamento)
                {
                    return new ServiceResult { Succeeded = false, Message = "Apenas dietas 'Em Andamento' podem ser concluídas." };
                }

                dieta.Status = backend.Domain.SaudeAnimal.Enums.StatusDieta.Concluida;
                dieta.DataConclusao = DateOnly.FromDateTime(DateTime.UtcNow);
                await _dietaRepository.UpdateAsync(dieta);
                await _dietaRepository.SaveChangesAsync();

                return new ServiceResult { Succeeded = true, Message = "Dieta concluída com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao concluir a dieta ID {DietaId}.", dietaId);
                throw;
            }
        }

        public async Task<ServiceResult> RegistrarPesoAsync(int dietaId, CreateRegistroPesoDto dto, int clinicaId)
        {
            _logger.LogInformation("Tentando registrar peso para dieta ID {DietaId}.", dietaId);
            try
            {
                var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
                if (dieta?.Avaliacao?.Consulta?.ClinicaId != clinicaId)
                {
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                if (dieta.TipoDieta != TipoDieta.Emagrecimento)
                {
                    return new ServiceResult { Succeeded = false, Message = "Registro de peso periódico é permitido apenas para dietas de emagrecimento." };
                }

                if (dieta.RegistrosPeso.Any(r => r.Semana == dto.Semana))
                {
                    return new ServiceResult { Succeeded = false, Message = $"Já existe um registro para a semana {dto.Semana}." };
                }

                var registro = new RegistroPesoDieta
                {
                    DietaId = dieta.Id,
                    Semana = dto.Semana,
                    DataRegistro = dto.DataRegistro ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    PesoReal = dto.PesoReal,
                    Observacao = dto.Observacao
                };

                await _dietaRepository.AddRegistroPesoAsync(registro);
                await _dietaRepository.SaveChangesAsync();

                return new ServiceResult { Succeeded = true, Message = "Peso registrado com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar peso na dieta ID {DietaId}.", dietaId);
                throw;
            }
        }

        public async Task<ServiceResult> RemoverRegistroPesoAsync(int dietaId, int registroId, int clinicaId)
        {
            _logger.LogInformation("Tentando remover o registro {RegistroId} da dieta ID {DietaId}.", registroId, dietaId);
            try
            {
                var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
                if (dieta?.Avaliacao?.Consulta?.ClinicaId != clinicaId)
                {
                    return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };
                }

                var registro = dieta.RegistrosPeso.FirstOrDefault(r => r.Id == registroId);
                if (registro == null)
                {
                    return new ServiceResult { Succeeded = false, Message = "Registro não encontrado." };
                }

                _dietaRepository.RemoveRegistroPeso(registro);
                await _dietaRepository.SaveChangesAsync();

                return new ServiceResult { Succeeded = true, Message = "Registro removido com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover registro {RegistroId} da dieta ID {DietaId}.", registroId, dietaId);
                throw;
            }
        }
    }
}