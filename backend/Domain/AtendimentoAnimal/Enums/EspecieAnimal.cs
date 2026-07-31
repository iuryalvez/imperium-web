using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.Enums
{
    public enum EspecieAnimal
    {
        [Display(Name = "Cão")]
        Cao = 1,
        [Display(Name = "Gato")]
        Gato = 2,
    }
}
