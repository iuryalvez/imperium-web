using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Entities {
    public class DietaPersonalizada {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RacaoId { get; set; }
        [ForeignKey("RacaoId")]
        public Racao? Racao { get; set; }
        public int AcompanhamentoIndId { get; set; }

        [ForeignKey("AcompanhamentoIndId")]
        public AcompanhamentoInd? AcompanhamentoInd { get; set; }
        public int TipoDieta { get; set; } // 1-manutenção, 2-crescimento, 3-lactação
        public float QtdAlimento { get; set; } // calculado
        public float EnergiaMetabolizavel { get; set; } // calculado
        public float PesoAtual { get; set; }
        public float PesoObjetivo { get; set; }
        public float PesoIdeal { get; set; }
    }
}
