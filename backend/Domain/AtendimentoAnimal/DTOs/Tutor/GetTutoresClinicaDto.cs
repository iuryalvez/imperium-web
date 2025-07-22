namespace backend.Domain.AtendimentoAnimal.DTOs
{
    public class GetTutoresClinicaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Endereco { get; set; }
        public int EndNumero { get; set; }
        public string? EndEstado { get; set; }
        public string? EndCidade { get; set; }
        public string? Cep { get; set; }
        public string? Telefone1 { get; set; }
        public string? Whatsapp { get; set; }
        public string? Email { get; set; }
        public DateTime DataCad { get; set; }
        public List<AnimalInfoSimplesDto>? Animais { get; set; }
    }
}
