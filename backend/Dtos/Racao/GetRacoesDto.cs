namespace backend.Dtos.Racao
{
    public class GetRacoesDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Ms { get; set; }
    }
}