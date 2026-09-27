using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Entities
{
    public class Dieta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RacaoId { get; set; }
        [ForeignKey("RacaoId")]
        public Racao? Racao { get; set; }
        public int AvaliacaoId { get; set; }

        [ForeignKey("AvaliacaoId")]
        public Avaliacao? Avaliacao { get; set; }
        public TipoDieta TipoDieta { get; set; } // 1-manutenção, 2-crescimento, 3-lactação
        public double FatorK { get; set; } // Ex: 130, 95, 100
        public double Expoente { get; set; } // Ex: 0.75, 0.67, 0.40
        public string PerfilNutricional { get; set; } = string.Empty; // Ex: "Cães ativos ou de canil (NRC, 2006)"
        public PerfilMetabolico? PerfilMetabolico { get; set; }
        public bool IsCustomizado { get; set; } = false;
        public double? MultiplicadorEnergeticoCustom { get; set; } // exemplo: 150 kcal
        public double? PotenciaCustom { get; set; } // exemplo: 0.8 (em vez de 0.75)
        public double? QtdAlimento { get; set; } // calculado
        public double? EnergiaMetabolizavel { get; set; } // calculado
        public double? PesoAtual { get; set; }
        public double? PesoObjetivo { get; set; }
        public double? PesoIdeal { get; set; }
        public DateOnly DataCriacao { get; set; }
        public string? CriadoPor { get; set; }
        public int? DuracaoEstimada { get; set; }
        public string? Observacoes { get; set; }
        public FaseVidaAnimal? FaseVida { get; set; }

        public bool AdicionalGestacao { get; set; }
    }
}
