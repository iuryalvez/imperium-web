using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.Enums
{
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

        [Display(Name = "6 - Sobrepeso")] // até 15% acima
        PoucoAcimaDoIdeal = 6,

        [Display(Name = "7 - Sobrepeso")] // 16% a 30% acima
        AcimaDoIdeal = 7,

        [Display(Name = "8 - Obesidade grave")] // 31% a 45% acima
        MuitoAcimaDoIdeal = 8,

        [Display(Name = "9 - Obesidade grave")] // Acima de 46%
        GravementeAcimaDoIdeal = 9
    }
}
