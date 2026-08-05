using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.Enums
{
    public enum FaseVidaAnimal
    {
        [Display(Name = "Desconhecida")]
        Desconhecida,

        [Display(Name = "Filhote")]
        Filhote,

        [Display(Name = "Jovem")]
        Jovem,

        [Display(Name = "Adulto")]
        Adulto,

        [Display(Name = "Sênior")]
        Senior,

        [Display(Name = "Geriátrico")]
        Geriatrico
    }
}
