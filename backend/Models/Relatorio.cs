using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Relatorio {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ConsultaId { get; set; } // foreign Key para ConsultaId
        public Consulta? Consulta { get; set; } // um relatorio para uma Consulta
        public int AcompanhamentoIndId { get; set; }
        public AcompanhamentoInd? AcompanhamentoInd { get; set; }
        public int Idade { get; set; }
        public float Peso { get; set; }
        public int CondicaoCorporal { get; set; }
        public int GrauAtividade { get; set; } // 0-inativo, 1-moderado, 2-ativo
        public string? Racao { get; set; }
        public string? DescDieta { get; set; }
    }
}
