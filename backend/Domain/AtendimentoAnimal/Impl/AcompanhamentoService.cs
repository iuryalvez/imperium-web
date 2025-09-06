using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public class AcompanhamentoService : IAcompanhamentoService
    {
        private readonly IAcompanhamentoRepository _acompanhamentoRepository;
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IRacaoRepository _racaoRepository;
        private readonly IDietaService _dietaService;
        private readonly IAnimalService _animalService;
        private readonly IMapper _mapper;

        public AcompanhamentoService(IAcompanhamentoRepository acompanhamentoRepository,
                                     IDietaService dietaService,
                                     IAnimalService animalService,
                                     IRelatorioRepository relatorioRepository,
                                     IRacaoRepository racaoRepository,
                                     IMapper mapper)
        {
            _acompanhamentoRepository = acompanhamentoRepository;
            _dietaService = dietaService;
            _animalService = animalService;
            _relatorioRepository = relatorioRepository;
            _racaoRepository = racaoRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResult> CreateAsync(int animalId)
        {
            // Evita criar acompanhamento duplicado
            if (await _acompanhamentoRepository.ExistsByAnimalIdAsync(animalId))
            {
                return new ServiceResult
                {
                    Succeeded = false,
                    Message = "Já existe um acompanhamento para este animal."
                };
            }

            var acompanhamento = new AcompanhamentoIndividualizado
            {
                AnimalId = animalId,
                DataInicio = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _acompanhamentoRepository.AddAsync(acompanhamento);
            await _acompanhamentoRepository.SaveChangesAsync();

            return new ServiceResult
            {
                Succeeded = true,
                Message = "Acompanhamento criado com sucesso.",
                Data = new { AcompanhamentoId = acompanhamento.Id }
            };
        }

        public async Task<AcompanhamentoIndividualizado?> GetByAnimalIdAsync(int animalId)
        {
            return await _acompanhamentoRepository.GetByAnimalIdAsync(animalId);
        }

        public async Task<ServiceResult> CriarDietaParaRelatorioAsync(int acompanhamentoId, CreateDietaDto dto, int clinicaId)
        {
            // 1. Validações de segurança e integridade

            // CORREÇÃO: Faltava 'await' aqui. Sem ele, a validação seguinte pode falhar.
            var acompanhamento = await _acompanhamentoRepository.GetByIdAsync(acompanhamentoId);

            // CORREÇÃO: Checar se o acompanhamento é nulo antes de usar suas propriedades
            if (acompanhamento == null || !await _animalService.IsAnimalInClinicAsync(acompanhamento.AnimalId, clinicaId))
            {
                return new ServiceResult { Succeeded = false, Message = "Acompanhamento não encontrado nesta clínica." };
            }

            // A validação do relatório está correta, assumindo que o método existe no repositório.
            var relatorio = await _relatorioRepository.GetByIdAndAcompanhamentoIdAsync(dto.RelatorioId, acompanhamentoId);
            if (relatorio == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Relatório não pertence a este acompanhamento." };
            }

            var racao = await _racaoRepository.GetByIdAndClinicaIdAsync(dto.RacaoId, clinicaId);
            if (racao == null || racao.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Ração não encontrada." };
            }

            // 2. Orquestração: Chame o serviço que já tem a lógica!
            try
            {
                // ATUALIZAÇÃO: Passar o DTO completo para o serviço, não apenas o TipoDieta.
                // O método agora tem acesso a todos os parâmetros necessários.
                var dieta = await _dietaService.CalcularOuAtualizarDietaAsync(
                    acompanhamento.Animal,
                    relatorio,
                    racao,
                    dto // Passando o DTO inteiro
                );

                // O mapeamento para o DTO de resposta continua o mesmo
                var dietaDto = _mapper.Map<GetDietaDto>(dieta);
                return new ServiceResult { Succeeded = true, Message = "Dieta calculada e salva com sucesso.", Data = dietaDto };
            }
            catch (InvalidOperationException ex) // Captura exceções de negócio específicas
            {
                return new ServiceResult { Succeeded = false, Message = ex.Message };
            }
            catch (Exception ex) // Captura exceções genéricas
            {
                // Logar o erro 'ex' para depuração
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro inesperado ao calcular a dieta." };
            }
        }
    }
}
