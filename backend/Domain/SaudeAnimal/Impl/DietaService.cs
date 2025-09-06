using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
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
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IRacaoRepository _racaoRepository;
        private readonly IMapper _mapper;
        private readonly CalculoNecessidadeEnergetica _calculoService;

        public DietaService(IDietaRepository dietaRepository,
                            IRelatorioRepository relatorioRepository,
                            IRacaoRepository racaoRepository,
                            IMapper mapper)
        {
            _dietaRepository = dietaRepository;
            _relatorioRepository = relatorioRepository;
            _racaoRepository = racaoRepository;
            _mapper = mapper;
            _calculoService = new CalculoNecessidadeEnergetica();
        }

        public async Task<DietaPersonalizada> CalcularOuAtualizarDietaAsync(
            Animal animal,
            Relatorio relatorio,
            Racao racao,
            CreateDietaDto dto)
        {
            // 1. Preparar o objeto para o cálculo
            //    O serviço de cálculo precisa de um objeto 'DietaPersonalizada'
            //    para obter o TipoDieta e os fatores de customização.
            var dietaParaCalculo = new DietaPersonalizada
            {
                TipoDieta = dto.TipoDieta,
                MultiplicadorEnergeticoCustom = dto.MultiplicadorEnergeticoCustom,
                PotenciaCustom = dto.PotenciaCustom
            };

            // 2. Chamar o serviço de cálculo para obter a necessidade energética diária (kcal)
            double necessidadeKcal = _calculoService.CalcularNecessidadeTotal(animal, dietaParaCalculo);

            // 3. Validar se a ração possui a informação de energia
            if (racao.EnergiaMetabolizavel == null || racao.EnergiaMetabolizavel <= 0)
            {
                // No seu DTO de ração, o campo é EnergiaMetabolizavel, não EnergiaMetabolizavelKcalPorKg
                throw new InvalidOperationException("A ração selecionada não possui um valor de energia metabolizável válido.");
            }

            // 4. Calcular a quantidade de alimento em gramas
            //    Assumindo que 'EnergiaMetabolizavel' na ração está em kcal/kg
            double kcalPorGrama = racao.EnergiaMetabolizavel.Value / 1000.0;
            double quantidadeGramas = necessidadeKcal / kcalPorGrama;

            // 5. Buscar por uma dieta existente para este relatório (lógica de Upsert)
            var dietaExistente = await _dietaRepository.FindByRelatorioIdAsync(relatorio.Id);

            if (dietaExistente == null)
            {
                // Se não existir, cria uma nova instância
                dietaExistente = new DietaPersonalizada();
                await _dietaRepository.AddAsync(dietaExistente);
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
            dietaExistente.EnergiaMetabolizavel = Math.Round(necessidadeKcal, 2); // Renomeei para ser mais claro
            dietaExistente.QtdAlimento = Math.Round(quantidadeGramas, 2); // Quantidade em gramas/dia

            // Metadados
            dietaExistente.DataCriacao = DateOnly.FromDateTime(DateTime.UtcNow);
            // dietaExistente.CriadoPor = ... // Implementar lógica para pegar usuário logado

            // 7. Salvar as alterações no banco de dados
            await _dietaRepository.SaveChangesAsync(); // Ou _dietaRepository.SaveChangesAsync() se o método existir lá

            return dietaExistente;
        }

        public async Task<GetDietaDto?> GetByIdAsync(int dietaId, int clinicaId)
        {
            var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId); // Supõe que o repo tem um método com includes

            // Verificação de segurança
            if (dieta?.Relatorio?.Consulta?.ClinicaId != clinicaId) return null;

            return _mapper.Map<GetDietaDto>(dieta);
        }

        public async Task<IEnumerable<GetDietaDto>> GetAllByAcompanhamentoAsync(int acompanhamentoId, int clinicaId)
        {
            // Validação de segurança primeiro
            var acompanhamento = _dietaRepository.GetAllByAcompanhamentoIdAsync(acompanhamentoId);
            if (acompanhamento == null) return new List<GetDietaDto>(); // Retorna vazio se não tiver acesso

            var dietas = await _dietaRepository.GetAllByAcompanhamentoIdAsync(acompanhamentoId);
            return _mapper.Map<IEnumerable<GetDietaDto>>(dietas);
        }

        public async Task<ServiceResult> UpdateAsync(int dietaId, UpdateDietaDto dto, int clinicaId)
        {
            var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
            if (dieta?.Relatorio?.Consulta?.ClinicaId != clinicaId)
                return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };

            _mapper.Map(dto, dieta);
            await _dietaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Dieta atualizada com sucesso." };
        }

        public async Task<ServiceResult> DeleteAsync(int dietaId, int clinicaId)
        {
            var dieta = await _dietaRepository.GetByIdWithIncludesAsync(dietaId);
            if (dieta?.Relatorio?.Consulta?.ClinicaId != clinicaId)
                return new ServiceResult { Succeeded = false, Message = "Dieta não encontrada ou acesso negado." };

            _dietaRepository.Delete(dieta);
            await _dietaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Dieta excluída com sucesso." };
        }
    }
}