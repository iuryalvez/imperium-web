using System.ComponentModel.DataAnnotations;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.DTOs.Dieta
{
    public class UpdateDietaDto
    {
        [Required]
        public int Id { get; set; } // ID da dieta a ser alterada

        public int? RacaoId { get; set; }
        public TipoDieta? TipoDieta { get; set; }
        public float? QtdAlimento { get; set; }
        public float? EnergiaMetabolizavel { get; set; }
        public float? PesoAtual { get; set; }
        public float? PesoObjetivo { get; set; }
        public float? PesoIdeal { get; set; }
    }
}