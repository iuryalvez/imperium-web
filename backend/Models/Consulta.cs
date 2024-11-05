namespace backend.Models {
    public class Consulta {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
        public int TutorId { get; set; }
        public Tutor Tutor { get; set; } = null!;
        public DateOnly Date { get; set; }
        public Relatorio? Relatorio { get; set; } = null!; // referência de navegação com o dependente
    }
}
