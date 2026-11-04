using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.SaudeAnimal.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace backend.Tests.Unitarios
{
    /// <summary>
    /// Testes unitários (sem banco nem API) do cálculo de necessidade energética.
    /// Os valores esperados vêm das fórmulas do NRC (2006), calculados à parte.
    /// </summary>
    public class CalculoNecessidadeEnergeticaTests
    {
        private readonly CalculoNecessidadeEnergetica _calculo = new(NullLogger<CalculoNecessidadeEnergetica>.Instance);

        [Fact]
        public void CalcularManutencao_Cao10KgComK130EP075_Retorna731Kcal()
        {
            var animal = new Animal { Especie = EspecieAnimal.Cao, Peso = 10 };
            var dieta = new Dieta { TipoDieta = TipoDieta.Manutencao, MultiplicadorEnergetico = 130, Potencia = 0.75 };

            var resultado = _calculo.CalcularManutencao(animal, dieta);

            // 130 × 10^0,75
            Assert.Equal(731.044, resultado);
            Assert.Equal(130, dieta.FatorK);
            Assert.Equal(0.75, dieta.Expoente);
        }

        [Fact]
        public void CalcularManutencao_SemPeso_LancaArgumentException()
        {
            var animal = new Animal { Especie = EspecieAnimal.Cao, Peso = null };
            var dieta = new Dieta { TipoDieta = TipoDieta.Manutencao, MultiplicadorEnergetico = 130, Potencia = 0.75 };

            Assert.Throws<ArgumentException>(() => _calculo.CalcularManutencao(animal, dieta));
        }

        [Fact]
        public void CalcularManutencao_SemMultiplicadorOuPotencia_LancaInvalidOperationException()
        {
            var animal = new Animal { Especie = EspecieAnimal.Cao, Peso = 10 };
            var dieta = new Dieta { TipoDieta = TipoDieta.Manutencao, MultiplicadorEnergetico = null, Potencia = 0.75 };

            Assert.Throws<InvalidOperationException>(() => _calculo.CalcularManutencao(animal, dieta));
        }

        [Fact]
        public void CalcularManutencao_FatorDiferenteDoPerfil_MarcaComoCustomizado()
        {
            var animal = new Animal { Especie = EspecieAnimal.Cao, Peso = 10 };
            var dieta = new Dieta { TipoDieta = TipoDieta.Manutencao, MultiplicadorEnergetico = 999, Potencia = 0.75 };

            _calculo.CalcularManutencao(animal, dieta);

            Assert.True(dieta.IsCustomizado);
        }

        [Fact]
        public void CalcularNecessidadeTotal_CrescimentoCao_UsaFormulaDoNrc()
        {
            // Filhote de 5 kg com peso adulto de 20 kg (p = 0,25): 130 × 5^0,75 × 3,2 × (e^(−0,87p) − 0,1)
            var animal = new Animal { Especie = EspecieAnimal.Cao, Sexo = Sexo.Macho, Peso = 5 };
            var dieta = new Dieta { TipoDieta = TipoDieta.Crescimento, MultiplicadorEnergetico = 130, Potencia = 0.75, PesoObjetivo = 20 };

            Assert.Equal(979.984, _calculo.CalcularNecessidadeTotal(animal, dieta));
        }

        [Fact]
        public void CalcularNecessidadeTotal_CrescimentoGato_UsaFormulaDoNrc()
        {
            // Filhote de 1 kg com peso adulto de 4 kg (p = 0,25): 100 × 1^0,67 × 6,7 × (e^(−0,189p) − 0,66)
            var animal = new Animal { Especie = EspecieAnimal.Gato, Sexo = Sexo.Femea, Peso = 1 };
            var dieta = new Dieta { TipoDieta = TipoDieta.Crescimento, MultiplicadorEnergetico = 100, Potencia = 0.67, PesoObjetivo = 4 };

            Assert.Equal(196.879, _calculo.CalcularNecessidadeTotal(animal, dieta));
        }

        [Fact]
        public void CalcularNecessidadeTotal_CrescimentoSemPesoAdultoConhecido_LancaInvalidOperationException()
        {
            var animal = new Animal { Especie = EspecieAnimal.Cao, Sexo = Sexo.Macho, Peso = 5 };
            var dieta = new Dieta { TipoDieta = TipoDieta.Crescimento, MultiplicadorEnergetico = 130, Potencia = 0.75 };

            Assert.Throws<InvalidOperationException>(() => _calculo.CalcularNecessidadeTotal(animal, dieta));
        }
    }
}
