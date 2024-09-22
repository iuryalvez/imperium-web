namespace backend.Models {
    public class Consulta {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int TutorId { get; set; }
        public Tutor Tutor { get; set; } = null!;
        public int AcompanhamentoIndId { get; set; }
        public AcompanhamentoInd? AcompanhamentoInd { get; set; }

        public DateOnly Date { get; set; }
        public Relatorio? Relatorio { get; set; } = null!; // referência de navegação com o dependente
    }
}
