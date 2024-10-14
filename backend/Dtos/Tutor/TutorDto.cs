using backend.Dtos.Consulta;
using backend.Dtos.TutorAnimalClinica;

namespace backend.Dtos.Tutor
{
    public class TutorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        // public int EndNumero { get; set; } 
        // public string EndEstado { get; set; } = string.Empty;
        // public string EndCidade { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        // public string Telefone1 { get; set; } = string.Empty; 
        // public string Telefone2 { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataCad { get; set; }
        public virtual ICollection<TutorAnimalClinicaDto>? TutoresAnimaisClinicas { get; set; }
        public virtual ICollection<ConsultaDto>? Consultas { get; set; }
    }
}
