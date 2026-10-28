using backend.Domain.AtendimentoAnimal.DTOs.Animal;
namespace backend.Domain.AtendimentoAnimal.DTOs.Tutor
{
    public class GetTutoresClinicaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public string? Telefone { get; set; }
        public string? Whatsapp { get; set; }
        public string? Email { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool IsAtivo { get; set; }
        public DateTime? DataDesativacao { get; set; }
        public List<AnimalInfoSimplesDto>? Animais { get; set; }

        // Endereço
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Uf { get; set; }
    }
}
