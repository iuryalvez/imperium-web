using System.ComponentModel.DataAnnotations;
using backend.Domain.General.Extensions;

namespace backend.Domain.AtendimentoAnimal.Enums
{
    /// <summary>
    /// Escore de condição corporal (escala de 9 pontos de Laflamme, fichas da WSAVA).
    /// O rótulo do ponto 4 depende da espécie (ideal no cão, magro no gato): use <see cref="CondicaoCorporalRotulo"/>.
    /// Excesso aproximado sobre o peso ideal nos pontos 7 a 9: Flanagan et al. (2017, 2018),
    /// que estimam o peso-alvo como peso atual ÷ 1,2 / 1,3 / 1,4.
    /// </summary>
    public enum CondicaoCorporal
    {
        [Display(Name = "1 - Emaciado")]
        GravementeAbaixoDoIdeal = 1,

        [Display(Name = "2 - Muito magro")]
        MuitoAbaixoDoIdeal = 2,

        [Display(Name = "3 - Magro")]
        AbaixoDoIdeal = 3,

        [Display(Name = "4 - Magro")]
        PoucoAbaixoDoIdeal = 4,

        [Display(Name = "5 - Ideal")]
        Ideal = 5,

        [Display(Name = "6 - Sobrepeso")]
        PoucoAcimaDoIdeal = 6,

        [Display(Name = "7 - Sobrepeso")] // cerca de 20% acima do ideal (Flanagan et al., 2017, 2018)
        AcimaDoIdeal = 7,

        [Display(Name = "8 - Obesidade grave")] // cerca de 30% acima do ideal (Flanagan et al., 2017, 2018)
        MuitoAcimaDoIdeal = 8,

        [Display(Name = "9 - Obesidade grave")] // 40% ou mais acima do ideal (Flanagan et al., 2017, 2018)
        GravementeAcimaDoIdeal = 9
    }

    public static class CondicaoCorporalRotulo
    {
        /// <summary>
        /// Rótulo do ECC para a espécie. Na WSAVA, o cão é ideal em 4 e 5; o gato, só em 5.
        /// </summary>
        public static string Obter(CondicaoCorporal condicao, EspecieAnimal? especie) =>
            condicao == CondicaoCorporal.PoucoAbaixoDoIdeal && especie == EspecieAnimal.Cao
                ? "4 - Ideal"
                : condicao.GetDisplayName();
    }
}
