using System.ComponentModel.DataAnnotations;

namespace backend.Domain.SaudeAnimal.Enums
{
    /// <summary>
    /// Nível de restrição alimentar aplicado na dieta de emagrecimento.
    /// </summary>
    public enum NivelRestricao
    {
        [Display(Name = "Severa")]
        Severa = 1,

        [Display(Name = "Moderada")]
        Moderada = 2,

        [Display(Name = "Branda")]
        Branda = 3
    }

    /// <summary>
    /// Parâmetros de cada nível de restrição:
    /// fator = fração da necessidade energética (EM) do perfil metabólico utilizada;
    /// perdaSemanal = fração do peso corporal que se espera perder por semana.
    /// </summary>
    public static class NivelRestricaoDados
    {
        public const int DuracaoPlanoEmagrecimentoSemanas = 15;

        public static readonly Dictionary<NivelRestricao, (double fator, double perdaSemanal)> Dados = new()
        {
            { NivelRestricao.Severa, (0.50, 0.020) },
            { NivelRestricao.Moderada, (0.60, 0.015) },
            { NivelRestricao.Branda, (0.70, 0.010) },
        };

        public static (double fator, double perdaSemanal) Obter(NivelRestricao nivel)
        {
            if (!Dados.TryGetValue(nivel, out var dados))
                throw new InvalidOperationException($"Nível de restrição '{nivel}' não suportado.");
            return dados;
        }
    }
}
