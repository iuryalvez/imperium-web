using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Services
{
    public class CalculoNecessidadeEnergetica
    {
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
    }
}