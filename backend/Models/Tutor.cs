namespace backend.Models {
    public class Tutor {
        public int Id { get; set; }
        public int ClinicaId { get; set; }

        public Clinica? Clinica { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int EndNumero { get; set; } 
        public string EndEstado { get; set; } = string.Empty;
        public string EndCidade { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone1 { get; set; } = string.Empty; 
        public string Telefone2 { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataCad { get; set; } = DateTime.UtcNow;
        public virtual ICollection<TutorAnimalClinica> TutoresAnimaisClinicas { get; set; } = new List<TutorAnimalClinica>();
        public virtual ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
    }
}
