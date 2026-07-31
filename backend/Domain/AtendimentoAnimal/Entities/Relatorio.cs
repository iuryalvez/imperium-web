using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Enums;
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
        public int AcompanhamentoId { get; set; }
        [ForeignKey("AcompanhamentoId")]
        public Acompanhamento? Acompanhamento { get; set; }
        public int? UsuarioResponsavelId { get; set; } // UserId de quem criou o relatório
        public DietaPersonalizada? DietaPersonalizada { get; set; }
        public int Idade { get; set; }
        public double Peso { get; set; }
        public double? PesoIdeal { get; set; }
        public CondicaoCorporal CondicaoCorporal { get; set; }
        public GrauAtividade GrauAtividade { get; set; } // 0-inativo, 1-moderado, 2-ativo
        public bool IsGestante { get; set; }
        public int IsLactanteNumeroFilhotes { get; set; }
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
        public int RacaoId { get; set; }
        [ForeignKey("RacaoId")]
        public Racao Racao { get; set; }
        public TipoDieta TipoDieta { get; set; }
        public string? DescricaoDieta { get; set; }
    }
}
