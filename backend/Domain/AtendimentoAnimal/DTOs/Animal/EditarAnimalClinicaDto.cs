using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal
{
    public class EditarAnimalClinicaDto
    {
        [Required]
        public int Id { get; set; }
        public List<int>? TutorIds { get; set; }

        // Campos que podem ser atualizados
        public string? Nome { get; set; }
        public int? RacaId { get; set; }
        public EspecieAnimal? Especie { get; set; }
        public DateTime? Nascimento { get; set; }
        public Sexo? Sexo { get; set; }
        public bool? Castrado { get; set; }
    }
}