namespace backend.Dtos.Relatorio
{
    public class RelatorioDto
    {
        public int Id { get; set; }
        public int ConsultaId { get; set; } // foreign key
        public int AcompanhamentoIndId { get; set; } // foreign key
        public int Idade { get; set; }
        public float Peso { get; set; }
        public int CondicaoCorporal { get; set; }
        public int GrauAtividade { get; set; } // 0-inativo, 1-moderado, 2-ativo
        public string Racao { get; set; } = string.Empty;
        public string DescDieta { get; set; } = string.Empty;
    }
}