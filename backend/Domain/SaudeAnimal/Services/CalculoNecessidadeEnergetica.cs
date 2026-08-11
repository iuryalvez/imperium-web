using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Services
{
    public interface ICalculoNecessidadeEnergetica
    {
        double CalcularGestacao(Animal animal, Dieta dieta);
        double CalcularLactacao(Animal animal, int numeroFilhotes, int semanaLactacao, Dieta dieta);
        double CalcularManutencao(Animal animal, Dieta dieta);
        double CalcularNecessidadeTotal(Animal animal, Dieta dieta);
    }

    public class CalculoNecessidadeEnergetica : ICalculoNecessidadeEnergetica
    {
        private readonly ILogger<CalculoNecessidadeEnergetica> _logger;

        public CalculoNecessidadeEnergetica(ILogger<CalculoNecessidadeEnergetica> logger)
        {
            _logger = logger;
        }

        public double CalcularNecessidadeTotal(Animal animal, Dieta dieta)
        {
            _logger.LogInformation(
                "Iniciando cálculo de necessidade energética total para o animal ID: {AnimalId} com tipo de dieta: {TipoDieta}.",
                animal.Id, dieta.TipoDieta);

            try
            {
                double resultado = dieta.TipoDieta switch
                {
                    TipoDieta.Manutencao => CalcularManutencao(animal, dieta),
                    TipoDieta.Crescimento => CalcularCrescimento(animal, dieta) ?? 0,
                    TipoDieta.Lactacao => CalcularLactacao(animal, animal.IsLactanteNumeroFilhotes ?? 0, animal.SemanaLactacao ?? 1, dieta),
                    TipoDieta.Gestacao => CalcularGestacao(animal, dieta),
                    _ => throw new InvalidOperationException($"Tipo de dieta '{dieta.TipoDieta}' não implementado.")
                };

                var resultadoArredondado = Math.Round(resultado, 3);

                _logger.LogInformation(
                    "Cálculo de necessidade total concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);

                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular a necessidade energética total para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        public double CalcularManutencao(Animal animal, Dieta dieta)
        {
            _logger.LogInformation("Iniciando cálculo de manutenção para o animal ID: {AnimalId}.", animal.Id);

            try
            {
                if (!animal.Peso.HasValue)
                {
                    throw new ArgumentException("O peso do animal não pode ser nulo para o cálculo de manutenção.");
                }

                var perfil = PerfilMetabolicoResolver.Resolver(animal, dieta);
                if (perfil == null)
                {
                    throw new InvalidOperationException("Não foi possível determinar um perfil metabólico automaticamente. Por favor, selecione um perfil metabólico no formulário.");
                }

                var (multiplicador, potencia, descricao) = PerfilMetabolicoDados.Dados[perfil.Value];

                // Permite o multiplicador customizado se o usuário informar (Ex: Gato Exótico varia de 55 a 260)
                if (dieta.MultiplicadorEnergeticoCustom.HasValue)
                {
                    multiplicador = dieta.MultiplicadorEnergeticoCustom.Value;
                    dieta.IsCustomizado = true;
                }
                else
                {
                    dieta.IsCustomizado = false;
                }

                _logger.LogInformation("Perfil metabólico resolvido: {Perfil}. Multiplicador final: {Multiplicador}, Potência padrão: {Potencia}.",
                    perfil, multiplicador, potencia);

                // Injeta transparência para Manutenção
                dieta.FatorK = multiplicador;
                dieta.Expoente = potencia;
                dieta.PerfilNutricional = descricao;

                var resultado = multiplicador * Math.Pow(animal.Peso.Value, potencia);
                var resultadoArredondado = Math.Round(resultado, 3);

                _logger.LogInformation(
                    "Cálculo de manutenção concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);

                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de manutenção para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        private double? CalcularCrescimento(Animal animal, Dieta dieta)
        {
            _logger.LogInformation("Iniciando cálculo para crescimento para o animal ID: {AnimalId}.", animal.Id);

            try
            {
                if (animal.Peso == null || animal.Peso <= 0)
                {
                    _logger.LogWarning("Peso do animal ID: {AnimalId} é nulo ou inválido para cálculo de crescimento.", animal.Id);
                    return null;
                }

                double? pesoMedio = animal.Sexo == Sexo.Macho ? animal.Raca?.PesoMedioMacho : animal.Raca?.PesoMedioFemea;
                double PCa = animal.Peso.Value;
                double PCm;

                if (animal.PesoMaximo.HasValue && animal.PesoMaximo > 0)
                {
                    PCm = animal.PesoMaximo.Value;
                }
                else if (animal.Raca != null && pesoMedio.HasValue)
                {
                    PCm = pesoMedio.Value;
                }
                else
                {
                    throw new InvalidOperationException("Peso adulto esperado (PCm) não pôde ser determinado para o cálculo de crescimento.");
                }

                _logger.LogInformation("Cálculo de crescimento para animal ID: {AnimalId}. Peso Atual (PCa): {PCa}, Peso Adulto (PCm): {PCm}.", animal.Id, PCa, PCm);

                double p = PCa / PCm;
                double necessidadeKcal;

                switch (animal.Especie)
                {
                    case EspecieAnimal.Gato:
                        var pesoMetabolicoGato = Math.Pow(PCa, 0.67);
                        necessidadeKcal = 100 * pesoMetabolicoGato * 6.7 * (Math.Exp(-0.189 * p) - 0.66);
                        break;
                    case EspecieAnimal.Cao:
                        var pesoMetabolicoCao = Math.Pow(PCa, 0.75);
                        necessidadeKcal = 130 * pesoMetabolicoCao * 3.2 * (Math.Exp(-0.87 * p) - 0.1);
                        break;
                    default:
                        throw new InvalidOperationException($"Espécie '{animal.Especie}' não suportada para cálculo de crescimento.");
                }

                // Injeta transparência para Crescimento
                dieta.FatorK = animal.Especie == EspecieAnimal.Cao ? 130 : 100;
                dieta.Expoente = animal.Especie == EspecieAnimal.Cao ? 0.75 : 0.67;
                dieta.PerfilNutricional = $"Crescimento ({animal.Especie} - NRC 2006)";
                dieta.IsCustomizado = false;

                var resultadoFinal = Math.Max(necessidadeKcal, 0);
                var resultadoArredondado = Math.Round(resultadoFinal, 3);

                _logger.LogInformation(
                    "Cálculo de crescimento concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);

                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de crescimento para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        public double CalcularGestacao(Animal animal, Dieta dieta)
        {
            _logger.LogInformation("Iniciando cálculo de gestação para o animal ID: {AnimalId} da espécie: {Especie}.", animal.Id, animal.Especie);

            try
            {
                var peso = animal.Peso ?? throw new InvalidOperationException("Peso não informado para cálculo de gestação.");
                var potencia = animal.Especie == EspecieAnimal.Gato ? 0.67 : 0.75;
                var pesoMetabolico = Math.Pow(peso, potencia);
                double resultado;

                if (animal.Especie == EspecieAnimal.Cao)
                {
                    resultado = (130 * pesoMetabolico) + (26 * peso);
                    _logger.LogInformation("Fórmula de gestação para cães baseada na necessidade de manutenção.");
                }
                else
                {
                    resultado = 100 * pesoMetabolico;
                    _logger.LogInformation("Fórmula de gestação para gatos baseada na necessidade de manutenção.");
                }

                // Injeta transparência para Gestação
                dieta.FatorK = animal.Especie == EspecieAnimal.Cao ? 130 : 100;
                dieta.Expoente = potencia;
                dieta.PerfilNutricional = $"Gestação ({animal.Especie} - NRC 2006)";
                dieta.IsCustomizado = false;

                var resultadoArredondado = Math.Round(resultado, 3);
                _logger.LogInformation(
                    "Cálculo de gestação concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);
                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de gestação para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        public double CalcularLactacao(Animal animal, int numeroFilhotes, int semanaLactacao, Dieta dieta)
        {
            _logger.LogInformation(
                "Iniciando cálculo de lactação para o animal ID: {AnimalId}, com {NumeroFilhotes} filhote(s) na {SemanaLactacao}ª semana.",
                animal.Id, numeroFilhotes, semanaLactacao);

            try
            {
                if (semanaLactacao < 1) throw new ArgumentException("Semana de lactação inválida.");
                var peso = animal.Peso ?? throw new InvalidOperationException("Peso não informado para cálculo de lactação.");

                double resultado = animal.Especie == EspecieAnimal.Gato
                    ? CalcularLactacaoGato(peso, numeroFilhotes, semanaLactacao)
                    : CalcularLactacaoCao(peso, numeroFilhotes, semanaLactacao);

                // Injeta transparência para Lactação
                dieta.FatorK = animal.Especie == EspecieAnimal.Cao ? 145 : 100;
                dieta.Expoente = animal.Especie == EspecieAnimal.Cao ? 0.75 : 0.67;
                dieta.PerfilNutricional = $"Lactação ({animal.Especie} - NRC 2006)";
                dieta.IsCustomizado = false;

                var resultadoArredondado = Math.Round(resultado, 3);
                _logger.LogInformation(
                    "Cálculo de lactação concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);
                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de lactação para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        private double CalcularLactacaoGato(double peso, int numeroFilhotes, int semana)
        {
            _logger.LogInformation("Executando cálculo de lactação para Gato. Peso: {Peso}, Filhotes: {NumeroFilhotes}, Semana: {Semana}.", peso, numeroFilhotes, semana);

            var pesoMetabolico = Math.Pow(peso, 0.67);
            double manutencao = 100 * pesoMetabolico;

            double[] fatores = { 0.9, 0.9, 1.2, 1.2, 1.1, 1.0, 0.8 };
            double fatorL = fatores[Math.Clamp(semana - 1, 0, fatores.Length - 1)];

            double multiplicadorFilhotes = numeroFilhotes switch
            {
                < 3 => 18,
                3 or 4 => 60,
                > 4 => 70,
            };

            _logger.LogInformation("Valores para Gato: Manutenção: {Manutencao}, Fator Lactação: {FatorL}, Multiplicador Filhotes: {Multiplicador}.",
                manutencao, fatorL, multiplicadorFilhotes);

            return manutencao + multiplicadorFilhotes * peso * fatorL;
        }

        private double CalcularLactacaoCao(double peso, int numeroFilhotes, int semana)
        {
            _logger.LogInformation("Executando cálculo de lactação para Cão. Peso: {Peso}, Filhotes: {NumeroFilhotes}, Semana: {Semana}.", peso, numeroFilhotes, semana);

            var pesoMetabolico = Math.Pow(peso, 0.75);
            double manutencao = 145 * pesoMetabolico;

            double[] fatores = { 0.75, 0.95, 1.1, 1.2 };
            double fatorL = fatores[Math.Clamp(semana - 1, 0, fatores.Length - 1)];

            int n = Math.Clamp(numeroFilhotes, 0, 4);
            int m = numeroFilhotes > 4 ? numeroFilhotes - 4 : 0;

            _logger.LogInformation("Valores para Cão: Manutenção: {Manutencao}, Fator Lactação: {FatorL}, n={n}, m={m}.",
                manutencao, fatorL, n, m);

            return manutencao + (peso * ((24 * n) + (12 * m)) * fatorL);
        }
    }
}