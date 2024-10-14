namespace backend.Dtos.Consulta
{
    public class ConsultaDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int TutorId { get; set; }
        public int AcompanhamentoIndId { get; set; }
        public DateOnly Date { get; set; }
    }
}