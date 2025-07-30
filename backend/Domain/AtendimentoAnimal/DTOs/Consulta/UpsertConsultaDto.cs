using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    public class UpsertConsultaDto
    {
        [Required]
        public int AnimalId { get; set; }

        [Required]
        public int TutorId { get; set; }

        // O frontend pode enviar o nome e CPF para registro histórico,
        // mesmo que o tutor já seja cadastrado.
        public string? TutorNome { get; set; }
        public string? TutorCpf { get; set; }

        [Required]
        public DateTime DataConsulta { get; set; }
    }
}