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
        public double? PesoAtual { get; set; }
        public double? PesoObjetivo { get; set; }
        public double? PesoIdeal { get; set; }
        public int? DuracaoEstimada { get; set; }

        // Customização opcional pelo usuário
        public double? MultiplicadorEnergeticoCustom { get; set; }
        public double? PotenciaCustom { get; set; }

        public string? Observacoes { get; set; }
    }
}