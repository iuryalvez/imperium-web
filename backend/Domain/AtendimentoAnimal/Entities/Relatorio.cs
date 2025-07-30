using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Relatorio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ConsultaId { get; set; } // foreign Key para ConsultaId
        [ForeignKey("ConsultaId")]
        public Consulta? Consulta { get; set; }
        public int Idade { get; set; }
        public float Peso { get; set; }
        public CondicaoCorporal CondicaoCorporal { get; set; }
        public GrauAtividade GrauAtividade { get; set; } // 0-inativo, 1-moderado, 2-ativo
        public bool IsGestante { get; set; }
        public int IsLactanteNumeroFilhotes { get; set; }
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
        public int RacaoId { get; set; }
        [ForeignKey("RacaoId")]
        public Racao? Racao { get; set; }
        public string? DescricaoDieta { get; set; }
    }
}
