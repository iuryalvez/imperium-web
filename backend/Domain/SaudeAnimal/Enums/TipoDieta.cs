using System.ComponentModel.DataAnnotations;

namespace backend.Domain.SaudeAnimal.Entities
{
public enum TipoDieta
    {
        [Display(Name = "Manutenção")]
        Manutencao = 1,

        [Display(Name = "Crescimento")]
        Crescimento = 2,

        [Display(Name = "Lactação")]
        Lactacao = 3,

        [Display(Name = "Gestação")]
        Gestacao = 4,

        [Display(Name = "Emagrecimento")]
        Emagrecimento = 5
    }
}
