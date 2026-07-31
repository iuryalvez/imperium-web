using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.Enums
{
    public enum CondicaoCorporal
    {
        [Display(Name = "1 - Caquético / Gravemente abaixo do ideal")]
        GravementeAbaixoDoIdeal = 1,

        [Display(Name = "2 - Muito magro / Muito abaixo do ideal")]
        MuitoAbaixoDoIdeal = 2,

        [Display(Name = "3 - Magro / Abaixo do ideal")]
        AbaixoDoIdeal = 3,

        [Display(Name = "4 - Abaixo do ideal (Leve)")]
        PoucoAbaixoDoIdeal = 4,

        [Display(Name = "5 - Ideal / Peso ótimo")]
        Ideal = 5,

        [Display(Name = "6 - Sobrepeso leve / Pouco acima do ideal")]
        PoucoAcimaDoIdeal = 6,

        [Display(Name = "7 - Sobrepeso / Acima do ideal")]
        AcimaDoIdeal = 7,

        [Display(Name = "8 - Obeso / Muito acima do ideal")]
        MuitoAcimaDoIdeal = 8,

        [Display(Name = "9 - Obesidade grave / Extrema")]
        GravementeAcimaDoIdeal = 9
    }
}
