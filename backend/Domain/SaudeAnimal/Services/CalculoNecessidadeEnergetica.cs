using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Services
{
    public class CalculoNecessidadeEnergetica
    {
        public double CalcularNecessidadeTotal(Animal animal, DietaPersonalizada dieta)
        {
            switch (dieta.TipoDieta)
            {
                case TipoDieta.Manutencao:
                    return CalcularManutencao(animal, dieta);
                case TipoDieta.Crescimento:
                    return CalcularCrescimento(animal) ?? 0;
                case TipoDieta.Lactacao:
                    return CalcularLactacao(animal, animal.IsLactanteNumeroFilhotes ?? 0, animal.SemanaLactacao ?? 1);
                case TipoDieta.Gestacao:
                    return CalcularGestacao(animal);
                default:
                    throw new InvalidOperationException($"Tipo de dieta '{dieta.TipoDieta}' não implementado.");
            }
        }
        
        public double CalcularManutencao(Animal animal, DietaPersonalizada dieta)
        {
            var perfil = PerfilMetabolicoResolver.Resolver(animal, dieta);

            // Pega valores padrão
            var (multiplicador, potencia) = PerfilMetabolicoDados.Dados[perfil];

            // Verifica customização
            if (dieta.MultiplicadorEnergeticoCustom.HasValue)
                multiplicador = dieta.MultiplicadorEnergeticoCustom.Value;

            if (dieta.PotenciaCustom.HasValue)
                potencia = dieta.PotenciaCustom.Value;

            if (!animal.Peso.HasValue)
                throw new ArgumentException("O peso do animal não pode ser nulo.");

            var resultado = multiplicador * Math.Pow(animal.Peso.Value, potencia);

            Console.WriteLine($"[DEBUG] Perfil: {perfil}, Multiplicador: {multiplicador}, Potência: {potencia}, Resultado: {resultado:F2}");

            return resultado;
        }

        private double? CalcularCrescimento(Animal animal)
        {
            if (animal.Peso == null || animal.Peso <= 0)
                return null;

            double? pesoMedio = animal.Sexo == Sexo.Macho
                ? animal.Raca?.PesoMedioMacho
                : animal.Raca?.PesoMedioFemea;

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
                throw new InvalidOperationException("Peso adulto esperado (PCm) não informado.");
            }

            double p = PCa / PCm;
            double necessidadeKcal = 0;

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
                    throw new InvalidOperationException("Espécie não suportada para cálculo de crescimento.");
            }

            // Ajuste por grau de atividade
            if (animal.GrauAtividade.HasValue)
            {
                switch (animal.GrauAtividade)
                {
                    case GrauAtividade.Inativo:
                        necessidadeKcal *= 0.85; // redução 15%
                        break;
                    case GrauAtividade.Ativo:
                        necessidadeKcal *= 1.15; // aumento 15%
                        break;
                        // Grau moderado mantém resultado padrão
                }
            }

            Console.WriteLine($"[DEBUG] Crescimento: {animal.Especie} | Peso Atual: {PCa} kg | Peso Adulto: {PCm} kg | Resultado: {necessidadeKcal:F2} kcal/dia");

            return Math.Max(necessidadeKcal, 0); // garante que não seja negativo
        }
        // É importante considerar, para ambas espécies, que os valores resultantes das
        // equações propostas podem sub ou superestimar as necessidades de um indivíduo
        // em particular em mais de 50%, de forma que ajustes são importantes e necessários.

        public double CalcularGestacao(Animal animal)
        {
            var peso = animal.Peso ?? throw new InvalidOperationException("Peso não informado.");
            var pesoMetabolico = Math.Pow(peso, animal.Especie == EspecieAnimal.Gato ? 0.67 : 0.75);

            if (animal.Especie == EspecieAnimal.Cao)
            {
                // Fórmula para cães (gestação)
                return (130 * pesoMetabolico) + (26 * peso);
            }
            else
            {
                // Para gatos não há fórmula exata no NRC, então podemos:
                // - Usar apenas energia de manutenção
                // - OU permitir personalização pelo usuário
                return 100 * pesoMetabolico;
            }
        }

        public double CalcularLactacao(Animal animal, int numeroFilhotes, int semanaLactacao)
        {
            if (semanaLactacao < 1) throw new ArgumentException("Semana de lactação inválida.");
            var peso = animal.Peso ?? throw new InvalidOperationException("Peso não informado.");

            if (animal.Especie == EspecieAnimal.Gato)
            {
                return CalcularLactacaoGato(peso, numeroFilhotes, semanaLactacao);
            }
            else
            {
                return CalcularLactacaoCao(peso, numeroFilhotes, semanaLactacao);
            }
        }

        private double CalcularLactacaoGato(double peso, int numeroFilhotes, int semana)
        {
            var pesoMetabolico = Math.Pow(peso, 0.67);
            double manutencao = 100 * pesoMetabolico;

            // Fator de lactação (0,9, 0,9, 1,2, 1,2, 1,1, 1,0, 0,8)
            double[] fatores = { 0.9, 0.9, 1.2, 1.2, 1.1, 1.0, 0.8 };
            double fatorL = fatores[Math.Clamp(semana - 1, 0, fatores.Length - 1)];

            double multiplicadorFilhotes = numeroFilhotes switch
            {
                < 3 => 18,
                3 or 4 => 60,
                > 4 => 70,
            };

            return manutencao + multiplicadorFilhotes * peso * fatorL;
        }

        private double CalcularLactacaoCao(double peso, int numeroFilhotes, int semana)
        {
            var pesoMetabolico = Math.Pow(peso, 0.75);
            double manutencao = 145 * pesoMetabolico;

            // Fatores: 1ª a 4ª semana
            double[] fatores = { 0.75, 0.95, 1.1, 1.2 };
            double fatorL = fatores[Math.Clamp(semana - 1, 0, fatores.Length - 1)];

            // Cálculo para filhotes: n (1-4), m (5-8)
            int n = Math.Clamp(numeroFilhotes, 0, 4);
            int m = numeroFilhotes > 4 ? numeroFilhotes - 4 : 0;

            return manutencao + (peso * ((24 * n) + (12 * m)) * fatorL);
        }

    }
}