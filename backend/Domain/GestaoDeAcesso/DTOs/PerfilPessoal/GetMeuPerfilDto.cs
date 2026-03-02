namespace backend.Domain.GestaoDeAcesso.DTOs.PerfilPessoal
{
    public class GetMeuPerfilDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Formacao { get; set; }
        public string? Cpf { get; set; }
        public string? Telefone { get; set; }
        public string? Whatsapp { get; set; }
        public DateTime DataCadastro { get; set; }
        public int TotalClinicasVinculadas { get; set; }
    }
}