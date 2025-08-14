using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    public class UpsertConsultaDto
    {
        [Required]
        public int AnimalId { get; set; }

        [Required]
        public int TutorId { get; set; }

        [Required]
        public DateTime DataConsulta { get; set; }
    }
}