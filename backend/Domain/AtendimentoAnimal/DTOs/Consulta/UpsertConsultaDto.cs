using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    public class UpsertConsultaDto
    {
        [Required(ErrorMessage = "O animal da consulta é obrigatório.")]
        public int? AnimalId { get; set; } // anulável para o [Required] disparar quando o campo não vem

        public int? TutorId { get; set; }

        [Required(ErrorMessage = "A data da consulta é obrigatória.")]
        public DateTime? DataConsulta { get; set; } // anulável para o [Required] disparar quando o campo não vem
    }
}