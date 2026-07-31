using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.Enums
{
    public enum GrauAtividade
    {
        [Display(Name = "Baixo / Inativo")]
        Inativo = 1,

        [Display(Name = "Moderado / Normal")]
        Moderado = 2,

        [Display(Name = "Alto / Ativo")]
        Ativo = 3
    }
}
