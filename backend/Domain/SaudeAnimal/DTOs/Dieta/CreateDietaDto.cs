using System.ComponentModel.DataAnnotations;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.DTOs.Dieta
{
    public class CreateDietaDto
    {
        [Required]
        public int RelatorioId { get; set; }

        [Required]
        public int RacaoId { get; set; }

        [Required]
        [EnumDataType(typeof(TipoDieta))]
        public TipoDieta TipoDieta { get; set; }

        public double? PesoAtual { get; set; }
        public double? PesoObjetivo { get; set; }
        public double? PesoIdeal { get; set; }
        public int? DuracaoEstimada { get; set; }

        // Customização opcional pelo usuário
        public double? MultiplicadorEnergeticoCustom { get; set; }
        public double? PotenciaCustom { get; set; }
        public string? Observacoes { get; set; }
        public int? IsLactanteNumeroFilhotes { get; set; }
        public int? SemanaLactacao { get; set; }
    }
}