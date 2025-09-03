using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Entities
{
    public class DietaPersonalizada
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RacaoId { get; set; }
        [ForeignKey("RacaoId")]
        public Racao? Racao { get; set; }
        public int RelatorioId { get; set; }

        [ForeignKey("RelatorioId")]
        public Relatorio? Relatorio { get; set; }
        public TipoDieta TipoDieta { get; set; } // 1-manutenção, 2-crescimento, 3-lactação
        public float QtdAlimento { get; set; } // calculado
        public float EnergiaMetabolizavel { get; set; } // calculado
        public float PesoAtual { get; set; }
        public float PesoObjetivo { get; set; }
        public float PesoIdeal { get; set; }
        public DateOnly DataCriacao { get; set; }
        public string? CriadoPor { get; set; }
        public int? DuracaoEstimada { get; set; }
        public string? Observacoes { get; set; }
        FaseVidaAnimal FaseVida { get; set; }
    }
}
