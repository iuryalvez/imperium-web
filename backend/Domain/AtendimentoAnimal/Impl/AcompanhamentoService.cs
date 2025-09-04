using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.SaudeAnimal.Entities;
using backend.Infra.Data;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public class AcompanhamentoService : IAcompanhamentoService
    {
        private readonly IAcompanhamentoRepository _repository;
        private readonly AppDbContext _context;

        public AcompanhamentoService(IAcompanhamentoRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ServiceResult> CreateAsync(int animalId)
        {
            // Evita criar acompanhamento duplicado
            if (await _repository.ExistsByAnimalIdAsync(animalId))
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

            await _repository.AddAsync(acompanhamento);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                Succeeded = true,
                Message = "Acompanhamento criado com sucesso.",
                Data = new { AcompanhamentoId = acompanhamento.Id }
            };
        }

        public async Task<AcompanhamentoIndividualizado?> GetByAnimalIdAsync(int animalId)
        {
            return await _repository.GetByAnimalIdAsync(animalId);
        }

        private double? CalcularNecessidadeEnergetica(Animal animal, DietaPersonalizada dieta)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            if (animal.Peso == null || animal.Peso <= 0)
                return null; // sem peso não há cálculo possível

            double peso = animal.Peso.Value;
            double necessidadeKcal = 0;

            // Primeiro switch: espécie
            switch (animal.Especie)
            {
                case EspecieAnimal.Gato:
                    {
                        switch (dieta.TipoDieta)
                        {
                            case TipoDieta.Manutencao:
                                if (animal.Raca != null && !animal.Raca.Nome.Contains("SRD", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Gatos exóticos - usando média (55 a 260 kcal)
                                    var pesoMetabolico = Math.Pow(peso, 0.75);
                                    necessidadeKcal = ((55 + 260) / 2.0) * pesoMetabolico;
                                }
                                else if (animal.CondicaoCorporal.HasValue && (int)animal.CondicaoCorporal <= 5)
                                {
                                    // Gatos domésticos magros ou em condição adequada
                                    var pesoMetabolico = Math.Pow(peso, 0.67);
                                    necessidadeKcal = 100 * pesoMetabolico;
                                }
                                else
                                {
                                    // Gatos obesos (ECC > 5)
                                    var pesoMetabolico = Math.Pow(peso, 0.40);
                                    necessidadeKcal = 130 * pesoMetabolico;
                                }
                                break;

                            case TipoDieta.Crescimento:

                                break;

                            case TipoDieta.Lactacao:

                                break;

                            default:
                                throw new InvalidOperationException($"Tipo de dieta '{dieta.TipoDieta}' não suportado para gatos.");
                        }
                        break;
                    }

                case EspecieAnimal.Cao:
                    {
                        var pesoMetabolicoCao = Math.Pow(peso, 0.75);
                        string? nomeRaca = animal.Raca?.Nome;

                        switch (dieta.TipoDieta)
                        {
                            case TipoDieta.Manutencao:
                                if (!string.IsNullOrEmpty(nomeRaca))
                                {
                                    if (nomeRaca.Contains("dogue alemão", StringComparison.OrdinalIgnoreCase) ||
                                        nomeRaca.Contains("grande dinamarquês", StringComparison.OrdinalIgnoreCase))
                                    {
                                        necessidadeKcal = 200 * pesoMetabolicoCao;
                                    }
                                    else if (nomeRaca.Contains("terrier", StringComparison.OrdinalIgnoreCase))
                                    {
                                        necessidadeKcal = 180 * pesoMetabolicoCao;
                                    }
                                    else
                                    {
                                        necessidadeKcal = 130 * pesoMetabolicoCao; // cão ativo/canil (padrão)
                                    }
                                }
                                else if (animal.FaseVida == FaseVidaAnimal.Senior || animal.FaseVida == FaseVidaAnimal.Geriatrico)
                                {
                                    // cães idosos podem ter necessidade reduzida
                                    necessidadeKcal = 130 * pesoMetabolicoCao;
                                }
                                else
                                {
                                    necessidadeKcal = 130 * pesoMetabolicoCao; // padrão
                                }
                                break;

                            case TipoDieta.Crescimento:

                                break;

                            case TipoDieta.Lactacao:

                                break;

                            default:
                                throw new InvalidOperationException($"Tipo de dieta '{dieta.TipoDieta}' não suportado para cães.");
                        }
                        break;
                    }

                default:
                    throw new InvalidOperationException($"Espécie '{animal.Especie}' não suportada para cálculo.");
            }

            Console.WriteLine($"[DEBUG] Necessidade energética ({animal.Especie}, {dieta.TipoDieta}, {animal.FaseVida}): {necessidadeKcal:F2} kcal/dia para {peso} kg.");
            return necessidadeKcal;
        }
    }
}
