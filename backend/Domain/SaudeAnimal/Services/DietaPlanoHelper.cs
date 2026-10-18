using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Services
{
    /// <summary>
    /// Regras auxiliares do ciclo de vida (plano) de uma dieta, reaproveitadas pelos mappers e serviços.
    /// </summary>
    public static class DietaPlanoHelper
    {
        /// <summary>
        /// Tipos de dieta que possuem acompanhamento semanal (projeções por semana).
        /// </summary>
        public static bool IsTipoSemanal(TipoDieta tipo) =>
            tipo == TipoDieta.Emagrecimento || tipo == TipoDieta.Gestacao || tipo == TipoDieta.Lactacao;

        /// <summary>
        /// Semana em que o plano começa (semana informada no formulário ou 1 no emagrecimento).
        /// </summary>
        public static int? SemanaInicial(Dieta dieta) => dieta.TipoDieta switch
        {
            TipoDieta.Emagrecimento => 1,
            TipoDieta.Gestacao => dieta.SemanaGestacao ?? 1,
            TipoDieta.Lactacao => dieta.SemanaLactacao ?? 1,
            _ => null
        };

        /// <summary>
        /// Semana atual do plano = semana inicial + semanas decorridas desde a DataInicio.
        /// Para dietas concluídas, a contagem para na DataConclusao.
        /// Retorna null se o plano não foi iniciado ou se o tipo não é semanal.
        /// </summary>
        public static int? CalcularSemanaAtual(Dieta dieta, DateOnly? hoje = null)
        {
            if (!dieta.DataInicio.HasValue || dieta.Status == StatusDieta.Aberta) return null;

            var inicial = SemanaInicial(dieta);
            if (!inicial.HasValue) return null;

            var referencia = dieta.Status == StatusDieta.Concluida && dieta.DataConclusao.HasValue
                ? dieta.DataConclusao.Value
                : (hoje ?? DateOnly.FromDateTime(DateTime.UtcNow));

            var dias = referencia.DayNumber - dieta.DataInicio.Value.DayNumber;
            if (dias < 0) dias = 0;

            return inicial.Value + (dias / 7);
        }

        /// <summary>
        /// Semana sugerida para o próximo registro de peso no emagrecimento (limitada a 1..15).
        /// </summary>
        public static int SemanaSugeridaRegistro(Dieta dieta)
        {
            var atual = CalcularSemanaAtual(dieta) ?? 1;
            return Math.Clamp(atual, 1, NivelRestricaoDados.DuracaoPlanoEmagrecimentoSemanas);
        }

        public static double? FatorRestricao(Dieta dieta) =>
            dieta.TipoDieta == TipoDieta.Emagrecimento && dieta.NivelRestricao.HasValue
                ? NivelRestricaoDados.Obter(dieta.NivelRestricao.Value).fator
                : null;

        /// <summary>
        /// Percentual (ex.: 1.5 = 1,5%) de perda de peso semanal esperada.
        /// </summary>
        public static double? PercentualPerdaSemanal(Dieta dieta) =>
            dieta.TipoDieta == TipoDieta.Emagrecimento && dieta.NivelRestricao.HasValue
                ? Math.Round(NivelRestricaoDados.Obter(dieta.NivelRestricao.Value).perdaSemanal * 100, 2)
                : null;

        /// <summary>
        /// Perda semanal estimada em kg = PesoAtual × PerdaEsperada.
        /// </summary>
        public static double? PerdaSemanalKg(Dieta dieta) =>
            dieta.TipoDieta == TipoDieta.Emagrecimento && dieta.NivelRestricao.HasValue && dieta.PesoAtual.HasValue
                ? Math.Round(dieta.PesoAtual.Value * NivelRestricaoDados.Obter(dieta.NivelRestricao.Value).perdaSemanal, 3)
                : null;
    }
}
