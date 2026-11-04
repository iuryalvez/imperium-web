using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.Enums
{
    /// <summary>
    /// Status da consulta. Só "Cancelada" é gravado (DataCancelamento); os demais são calculados:
    /// sem avaliação = Pendente; com avaliação = EmAndamento; avaliação com dieta = Concluida.
    /// </summary>
    public enum StatusConsulta
    {
        [Display(Name = "Pendente")]
        Pendente = 1,
        [Display(Name = "Em andamento")]
        EmAndamento = 2,
        [Display(Name = "Concluída")]
        Concluida = 3,
        [Display(Name = "Cancelada")]
        Cancelada = 4,
    }
}
