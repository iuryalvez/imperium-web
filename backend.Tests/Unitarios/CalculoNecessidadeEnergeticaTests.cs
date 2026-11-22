using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.SaudeAnimal.Services;
using System.Text.Json;
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

        [Fact]
        public void CalcularEmagrecimento_Cao38KgAlvo34Moderada_UsaManutencaoDoPesoAlvo()
        {
            // Guimarães e Tudury (2006): 130 × 34^0,75 × 0,60 ≈ 1098 kcal
            var animal = new Animal { Especie = EspecieAnimal.Cao, Peso = 38 };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Emagrecimento, PerfilMetabolico = PerfilMetabolico.CaoEmEmagrecimento,
                MultiplicadorEnergetico = 130, Potencia = 0.75, PesoObjetivo = 34, NivelRestricao = NivelRestricao.Moderada,
            };

            var resultado = _calculo.CalcularEmagrecimento(animal, dieta);

            Assert.Equal(Math.Round(130 * Math.Pow(34, 0.75) * 0.60, 3), resultado);
            Assert.Equal(1098, Math.Round(resultado));
        }

        [Fact]
        public void CalcularEmagrecimento_Previsoes_TemAMesmaEnergiaEmTodasAsSemanasAteOAlvo()
        {
            var animal = new Animal { Especie = EspecieAnimal.Cao, Peso = 38 };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Emagrecimento, PerfilMetabolico = PerfilMetabolico.CaoEmEmagrecimento,
                MultiplicadorEnergetico = 130, Potencia = 0.75, PesoObjetivo = 34, NivelRestricao = NivelRestricao.Moderada,
            };

            var resultado = _calculo.CalcularEmagrecimento(animal, dieta);

            var previsoes = JsonDocument.Parse(dieta.PrevisoesJson!).RootElement.EnumerateArray().ToList();
            Assert.Equal(NivelRestricaoDados.DuracaoPlanoEmagrecimentoSemanas, previsoes.Count);
            var emRestricao = previsoes.Where(p => !p.GetProperty("alvoAtingido").GetBoolean()).ToList();
            Assert.NotEmpty(emRestricao);
            Assert.All(emRestricao, p => Assert.Equal(resultado, p.GetProperty("kcal").GetDouble()));
            // A projeção do peso é linear: perde os mesmos kg toda semana (1,5% de 38 kg = 0,57 kg)
            Assert.Equal(38 - 0.57, emRestricao[1].GetProperty("pesoInicio").GetDouble(), 3);
            Assert.Equal(38 - 2 * 0.57, emRestricao[2].GetProperty("pesoInicio").GetDouble(), 3);
        }

        [Fact]
        public void CalcularGestacao_Gata4KgComKEPDoPerfil_Retorna140xPC067SemPrevisoes()
        {
            var (k, p, _, _, _) = PerfilMetabolicoDados.Dados[PerfilMetabolico.GataEmGestacao];
            var animal = new Animal { Especie = EspecieAnimal.Gato, Sexo = Sexo.Femea, Peso = 4 };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Gestacao, PerfilMetabolico = PerfilMetabolico.GataEmGestacao,
                MultiplicadorEnergetico = k, Potencia = p, PesoAtual = 4, SemanaGestacao = 5, PrevisoesJson = "[]",
            };

            var resultado = _calculo.CalcularGestacao(animal, dieta);

            Assert.Equal(140, k);
            Assert.Equal(0.67, p);
            Assert.Equal(Math.Round(140 * Math.Pow(4, 0.67), 3), resultado);
            Assert.Equal(354, Math.Round(resultado));
            Assert.Null(dieta.PrevisoesJson);
            Assert.False(dieta.IsCustomizado);
        }

        [Fact]
        public void CalcularGestacao_GataNaSemana9EOutraNa1_TemAMesmaEnergia()
        {
            var animal = new Animal { Especie = EspecieAnimal.Gato, Sexo = Sexo.Femea, Peso = 4 };
            Dieta Nova(int semana) => new()
            {
                TipoDieta = TipoDieta.Gestacao, PerfilMetabolico = PerfilMetabolico.GataEmGestacao,
                MultiplicadorEnergetico = 140, Potencia = 0.67, PesoAtual = 4, SemanaGestacao = semana,
            };

            Assert.Equal(_calculo.CalcularGestacao(animal, Nova(1)), _calculo.CalcularGestacao(animal, Nova(9)));
        }

        [Theory]
        [InlineData(EspecieAnimal.Gato)]
        [InlineData(EspecieAnimal.Cao)]
        public void CalcularGestacao_Semana10_LancaInvalidOperationException(EspecieAnimal especie)
        {
            var animal = new Animal { Especie = especie, Sexo = Sexo.Femea, Peso = 4 };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Gestacao, MultiplicadorEnergetico = 140, Potencia = 0.67, PesoAtual = 4, SemanaGestacao = 10,
            };

            var erro = Assert.Throws<InvalidOperationException>(() => _calculo.CalcularGestacao(animal, dieta));
            Assert.Contains("entre 1 e 9", erro.Message);
        }

        [Fact]
        public void CalcularLactacao_Cadela27Kg7FilhotesSemana1ComPerfil_Retorna4390Kcal()
        {
            // Exemplo do Carciofi: 145 × 27^0,75 + 27 × (24 × 4 + 12 × 3) × 0,75 ≈ 4390 kcal
            var (k, p, _, _, _) = PerfilMetabolicoDados.Dados[PerfilMetabolico.CadelaEmLactacao];
            var animal = new Animal { Especie = EspecieAnimal.Cao, Sexo = Sexo.Femea, Peso = 27 };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Lactacao, PerfilMetabolico = PerfilMetabolico.CadelaEmLactacao,
                MultiplicadorEnergetico = k, Potencia = p, NumeroFilhotes = 7, SemanaLactacao = 1,
            };

            var resultado = _calculo.CalcularLactacao(animal, dieta);

            Assert.Equal(145, k);
            Assert.Equal(0.75, p);
            Assert.Equal(4390, Math.Round(resultado));
            Assert.False(dieta.IsCustomizado);
        }

        [Fact]
        public void PerfilGataEmLactacao_PreencheK100EP067()
        {
            var (k, p, _, _, _) = PerfilMetabolicoDados.Dados[PerfilMetabolico.GataEmLactacao];

            Assert.Equal(100, k);
            Assert.Equal(0.67, p);
        }

        [Fact]
        public void CalcularSemanaAtual_GestacaoQuePassariaDe9_FicaLimitadaA9()
        {
            var inicio = new DateOnly(2030, 1, 1);
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Gestacao, SemanaGestacao = 7, Status = StatusDieta.EmAndamento, DataInicio = inicio,
            };

            Assert.Equal(8, DietaPlanoHelper.CalcularSemanaAtual(dieta, inicio.AddDays(7)));
            Assert.Equal(9, DietaPlanoHelper.CalcularSemanaAtual(dieta, inicio.AddDays(70)));
        }

        [Fact]
        public void PerfilCadelaEmGestacao_PreencheK130EP075DoNrc()
        {
            var (k, p, _, especie, sexo) = PerfilMetabolicoDados.Dados[PerfilMetabolico.CadelaEmGestacao];

            Assert.Equal(130, k);
            Assert.Equal(0.75, p);
            Assert.Equal(EspecieAnimal.Cao, especie);
            Assert.Equal(Sexo.Femea, sexo);
        }

        [Theory]
        [InlineData(EspecieAnimal.Cao, CondicaoCorporal.PoucoAbaixoDoIdeal, "4 - Ideal")]
        [InlineData(EspecieAnimal.Gato, CondicaoCorporal.PoucoAbaixoDoIdeal, "4 - Magro")]
        [InlineData(EspecieAnimal.Cao, CondicaoCorporal.Ideal, "5 - Ideal")]
        [InlineData(EspecieAnimal.Gato, CondicaoCorporal.PoucoAcimaDoIdeal, "6 - Sobrepeso")]
        public void CondicaoCorporalRotulo_DependeDaEspecieNoPonto4(EspecieAnimal especie, CondicaoCorporal condicao, string esperado)
        {
            Assert.Equal(esperado, CondicaoCorporalRotulo.Obter(condicao, especie));
        }
    }
}
