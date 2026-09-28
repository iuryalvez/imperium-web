using System.ComponentModel.DataAnnotations;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.DTOs.Dieta
{
    public class CreateDietaDto
    {
        [Required]
        public int AvaliacaoId { get; set; }
        
        [Required]
        public int AcompanhamentoId { get; set; }

        [Required]
        public int RacaoId { get; set; }

        [Required]
        [EnumDataType(typeof(TipoDieta))]
        public TipoDieta TipoDieta { get; set; }

        public double? PesoAtual { get; set; }
        public double? PesoObjetivo { get; set; }
        public double? PesoIdeal { get; set; }
        public int? DuracaoEstimada { get; set; }

        public PerfilMetabolico? PerfilMetabolico { get; set; }
        
        // Customização opcional pelo usuário
        public double? MultiplicadorEnergetico { get; set; }
        public double? Potencia { get; set; }
        public string? Observacoes { get; set; }
        public int? NumeroFilhotes { get; set; }
        public int? SemanaLactacao { get; set; }
        public int? SemanaGestacao { get; set; }
        public bool? HasAdicionalGestacaoCao { get; set; }
        public string? PrevisoesJson { get; set; }
    }
}