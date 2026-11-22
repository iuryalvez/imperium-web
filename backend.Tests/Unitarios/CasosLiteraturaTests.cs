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
    /// Casos da literatura (exemplos resolvidos) para o cálculo da necessidade energética.
    /// Cada caso tem o ID de .claude/skills/teste-visual/casos-literatura.md, onde estão a fonte,
    /// a página e o roteiro para conferir o mesmo caso na tela. O esperado é o valor publicado,
    /// e a tolerância é de 1 % (as fontes arredondam). Se um caso falhar, confira o documento
    /// antes de mudar o teste: a divergência pode ser da fonte ou do sistema.
    /// </summary>
    public class CasosLiteraturaTests
    {
        private const double Tolerancia = 0.01;

        private readonly CalculoNecessidadeEnergetica _calculo = new(NullLogger<CalculoNecessidadeEnergetica>.Instance);

        private static void AssertProximo(double esperado, double obtido, string caso)
        {
            var diferenca = Math.Abs(obtido - esperado) / esperado;
            Assert.True(diferenca <= Tolerancia,
                $"{caso}: esperado {esperado} (literatura), sistema deu {obtido:F2} ({diferenca:P2} de diferença).");
        }

        // nrc:2007 p.13/15 e carciofi:apostila p.16/102 (cães); nrc:2007 p.14/16 e carciofi:apostila p.17/103 (gatos);
        // D-NRCD-01..04: NRC (2006), "Your Dog's Nutritional Needs", 50 lb = 22,68 kg.
        [Theory]
        [InlineData("A-MAN-01", EspecieAnimal.Cao, 7.0, 180.0, 0.75, 774.6)]
        [InlineData("A-MAN-02", EspecieAnimal.Cao, 35.0, 130.0, 0.75, 1870.7)]
        [InlineData("A-MAN-03", EspecieAnimal.Cao, 12.5, 105.0, 0.75, 698.0)]
        [InlineData("A-MAN-04", EspecieAnimal.Cao, 32.0, 105.0, 0.75, 1412.7)]
        [InlineData("A-MAN-05", EspecieAnimal.Cao, 53.0, 95.0, 0.75, 1866.0)]
        [InlineData("A-MAN-06", EspecieAnimal.Cao, 2.8, 95.0, 0.75, 205.6)]
        [InlineData("A-MAN-07", EspecieAnimal.Gato, 5.1, 100.0, 0.67, 298.0)]
        [InlineData("A-MAN-08", EspecieAnimal.Gato, 6.8, 130.0, 0.40, 280.0)]
        [InlineData("A-MAN-09", EspecieAnimal.Gato, 4.5, 130.0, 0.40, 237.0)]
        [InlineData("A-MAN-10", EspecieAnimal.Gato, 4.2, 130.0, 0.40, 231.0)]
        [InlineData("A-MAN-11", EspecieAnimal.Gato, 3.2, 100.0, 0.67, 218.0)]
        [InlineData("A-MAN-12", EspecieAnimal.Gato, 3.1, 100.0, 0.67, 213.0)]
        [InlineData("D-NRCD-01", EspecieAnimal.Cao, 22.68, 95.0, 0.75, 989.0)]
        [InlineData("D-NRCD-02", EspecieAnimal.Cao, 22.68, 130.0, 0.75, 1353.0)]
        [InlineData("D-NRCD-03", EspecieAnimal.Cao, 22.68, 140.0, 0.75, 1451.0)]
        [InlineData("D-NRCD-04", EspecieAnimal.Cao, 22.68, 105.0, 0.75, 1093.0)]
        public void Manutencao_ExemploDaLiteratura_BateComAFonte(string caso, EspecieAnimal especie, double peso, double k, double p, double esperado)
        {
            var animal = new Animal { Especie = especie, Peso = peso };
            var dieta = new Dieta { TipoDieta = TipoDieta.Manutencao, MultiplicadorEnergetico = k, Potencia = p };

            AssertProximo(esperado, _calculo.CalcularManutencao(animal, dieta), caso);
        }

        // nrc:2007 p.7–8 e carciofi:apostila p.10–11; ogoshi:2015 p.10 (impr. 73);
        // D-FUK-01/02: Fukuda (2024), Labrador nas semanas 8 e 24.
        [Theory]
        [InlineData("A-CRE-01", EspecieAnimal.Gato, 0.8, 5.0, 100.0, 0.67, 179.0)]
        [InlineData("A-CRE-02", EspecieAnimal.Cao, 15.0, 30.0, 130.0, 0.75, 1735.0)]
        [InlineData("A-CRE-03", EspecieAnimal.Cao, 17.0, 35.0, 130.0, 0.75, 1934.0)]
        [InlineData("D-FUK-01", EspecieAnimal.Cao, 5.330, 28.1, 130.0, 0.75, 1091.29)]
        [InlineData("D-FUK-02", EspecieAnimal.Cao, 19.022, 28.1, 130.0, 0.75, 1723.64)]
        public void Crescimento_ExemploDaLiteratura_BateComAFonte(string caso, EspecieAnimal especie, double peso, double pesoAdulto, double k, double p, double esperado)
        {
            var animal = new Animal { Especie = especie, Sexo = Sexo.Macho, Peso = peso };
            var dieta = new Dieta { TipoDieta = TipoDieta.Crescimento, MultiplicadorEnergetico = k, Potencia = p, PesoObjetivo = pesoAdulto };

            AssertProximo(esperado, _calculo.CalcularNecessidadeTotal(animal, dieta), caso);
        }

        // nrc:2007 p.8–9 e carciofi:apostila p.12; ogoshi:2015 p.11 (impr. 74); ogoshi:2015 p.6 (gata);
        // D-NRCD-05: NRC (2006), folheto do cão, 50 lb, da 4ª semana ao parto.
        [Theory]
        [InlineData("A-GES-01", EspecieAnimal.Cao, 30.0, 130.0, 0.75, true, 2446.0)]
        [InlineData("A-GES-02", EspecieAnimal.Cao, 22.0, 130.0, 0.75, true, 1892.0)]
        [InlineData("A-GES-04", EspecieAnimal.Gato, 3.5, 140.0, 0.67, false, 324.0)]
        [InlineData("D-NRCD-05", EspecieAnimal.Cao, 22.68, 130.0, 0.75, true, 1940.0)]
        public void Gestacao_ExemploDaLiteratura_BateComAFonte(string caso, EspecieAnimal especie, double peso, double k, double p, bool faseFinal, double esperado)
        {
            var animal = new Animal { Especie = especie, Sexo = Sexo.Femea, Peso = peso };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Gestacao, MultiplicadorEnergetico = k, Potencia = p, PesoAtual = peso,
                SemanaGestacao = 6, HasAdicionalGestacaoCao = faseFinal,
            };

            AssertProximo(esperado, _calculo.CalcularGestacao(animal, dieta), caso);
        }

        // nrc:2007 p.9–10 e carciofi:apostila p.12–13; ogoshi:2015 p.11–12;
        // D-NRCD-08: folheto do cão, 33 lb (15 kg), 6 filhotes, semana 4; D-NRCG-03: folheto do gato, 20 lb, 4 filhotes, pico.
        [Theory]
        [InlineData("A-LAC-01", EspecieAnimal.Gato, 5.0, 5, 3, 714.0)]
        [InlineData("A-LAC-02", EspecieAnimal.Cao, 27.0, 7, 1, 4390.0)]
        [InlineData("A-LAC-03", EspecieAnimal.Cao, 22.0, 6, 3, 4377.0)]
        [InlineData("A-LAC-04", EspecieAnimal.Gato, 3.5, 4, 3, 483.0)]
        [InlineData("D-NRCD-08", EspecieAnimal.Cao, 15.0, 6, 4, 3265.0)]
        [InlineData("D-NRCG-03", EspecieAnimal.Gato, 9.072, 4, 3, 1091.0)]
        public void Lactacao_ExemploDaLiteratura_ComKEPDoPerfil_BateComAFonte(string caso, EspecieAnimal especie, double peso, int filhotes, int semana, double esperado)
        {
            var perfil = especie == EspecieAnimal.Gato ? PerfilMetabolico.GataEmLactacao : PerfilMetabolico.CadelaEmLactacao;
            var (k, p, _, _, _) = PerfilMetabolicoDados.Dados[perfil];
            var animal = new Animal { Especie = especie, Sexo = Sexo.Femea, Peso = peso };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Lactacao, PerfilMetabolico = perfil, MultiplicadorEnergetico = k, Potencia = p,
                NumeroFilhotes = filhotes, SemanaLactacao = semana,
            };

            AssertProximo(esperado, _calculo.CalcularLactacao(animal, dieta), caso);
        }

        // neves:2006 p.9 (Tabelas 2 e 3, exemplos do método usado no sistema); carciofi:obesidade p.12–13 (K 72).
        [Theory]
        [InlineData("B-EMA-01", EspecieAnimal.Cao, 35.0, 28.0, 132.0, 0.75, NivelRestricao.Moderada, 960.0)]
        [InlineData("B-EMA-02", EspecieAnimal.Gato, 8.0, 6.0, 60.0, 1.0, NivelRestricao.Moderada, 216.0)]
        [InlineData("B-EMA-11", EspecieAnimal.Cao, 30.0, 25.5, 72.0, 0.75, NivelRestricao.Severa, 408.5)]
        [InlineData("B-EMA-11", EspecieAnimal.Cao, 30.0, 25.5, 72.0, 0.75, NivelRestricao.Moderada, 490.2)]
        [InlineData("B-EMA-11", EspecieAnimal.Cao, 30.0, 25.5, 72.0, 0.75, NivelRestricao.Branda, 571.9)]
        public void Emagrecimento_ExemploDaLiteratura_BateComAFonte(string caso, EspecieAnimal especie, double peso, double alvo, double k, double p, NivelRestricao nivel, double esperado)
        {
            var animal = new Animal { Especie = especie, Peso = peso };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Emagrecimento, MultiplicadorEnergetico = k, Potencia = p, PesoObjetivo = alvo, NivelRestricao = nivel,
            };

            AssertProximo(esperado, _calculo.CalcularEmagrecimento(animal, dieta), caso);
        }

        // B-EMA-04: neves:2006 p.8–9. A perda semanal é uma porcentagem do peso inicial (kg constantes).
        // Cão 35 → 28 kg: Moderada perde 0,525 kg/semana (33,95 kg ao fim da semana 2) e chega ao alvo na semana 14;
        // Severa perde 0,70 kg/semana e chega na semana 10; Branda perde 0,35 kg/semana e não chega em 15 semanas.
        [Theory]
        [InlineData(NivelRestricao.Moderada, 33.95, 15)]
        [InlineData(NivelRestricao.Severa, 33.60, 11)]
        [InlineData(NivelRestricao.Branda, 34.30, null)]
        public void Emagrecimento_ProjecaoDoPeso_PerdeSobreOPesoInicialComoNaFonte(NivelRestricao nivel, double pesoFimSemana2, int? primeiraSemanaNoAlvo)
        {
            var animal = new Animal { Especie = EspecieAnimal.Cao, Peso = 35 };
            var dieta = new Dieta
            {
                TipoDieta = TipoDieta.Emagrecimento, MultiplicadorEnergetico = 132, Potencia = 0.75, PesoObjetivo = 28, NivelRestricao = nivel,
            };

            _calculo.CalcularEmagrecimento(animal, dieta);

            var previsoes = JsonDocument.Parse(dieta.PrevisoesJson!).RootElement.EnumerateArray().ToList();
            AssertProximo(pesoFimSemana2, previsoes[1].GetProperty("pesoProjetado").GetDouble(), "B-EMA-04 (semana 2)");
            var noAlvo = previsoes.FirstOrDefault(p => p.GetProperty("alvoAtingido").GetBoolean());
            int? semanaNoAlvo = noAlvo.ValueKind == JsonValueKind.Undefined ? null : noAlvo.GetProperty("semana").GetInt32();
            Assert.Equal(primeiraSemanaNoAlvo, semanaNoAlvo);
        }
    }
}
