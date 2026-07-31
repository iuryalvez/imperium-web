using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.Enums
{
    public enum FaseVidaAnimal
    {
        [Display(Name = "Desconhecida")]
        Desconhecida,

        [Display(Name = "Filhote (Em aleitamento / Crescimento inicial)")]
        Filhote,

        [Display(Name = "Jovem (Crescimento tardio / Maturidade sexual)")]
        Jovem,

        [Display(Name = "Adulto")]
        Adulto,

        [Display(Name = "Sênior")]
        Senior,

        [Display(Name = "Geriátrico")]
        Geriatrico
    }
}
