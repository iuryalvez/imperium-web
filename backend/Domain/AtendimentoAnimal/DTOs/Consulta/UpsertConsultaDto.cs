using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    public class UpsertConsultaDto
    {
        [Required]
        public int AnimalId { get; set; }

        public int? TutorId { get; set; }

        [Required]
        public DateTime DataConsulta { get; set; }
    }
}