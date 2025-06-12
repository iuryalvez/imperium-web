namespace backend.Domain.AtendimentoAnimal.DTOs
{
    public class ConsultaDto
    {
        public int UsuarioId { get; set; }
        public int ClinicaId { get; set; }
        public int TutorId { get; set; }
        public DateTime DataConsulta { get; set; }
    }
}