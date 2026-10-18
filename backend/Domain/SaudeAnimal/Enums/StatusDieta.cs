using System.ComponentModel.DataAnnotations;

namespace backend.Domain.SaudeAnimal.Enums
{
    /// <summary>
    /// Ciclo de vida de uma dieta.
    /// ABERTA: recém-criada, editável e recalculável.
    /// EM ANDAMENTO: plano iniciado (DataInicio definida); ainda editável, exceto o TipoDieta.
    /// CONCLUÍDA: somente leitura.
    /// </summary>
    public enum StatusDieta
    {
        [Display(Name = "Aberta")]
        Aberta = 1,

        [Display(Name = "Em Andamento")]
        EmAndamento = 2,

        [Display(Name = "Concluída")]
        Concluida = 3
    }
}
