using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Tutor
    { // TODO: depois retirar o string.Empty dos atributos obrigatórios
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }

        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }

        [Required(ErrorMessage = "O nome do tutor é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do tutor deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        // [Required(ErrorMessage = "O CPF do tutor é obrigatório.")]
        [StringLength(11, ErrorMessage = "O CPF do tutor deve ter no máximo 11 dígitos.")]
        public string Cpf { get; set; } = string.Empty;

        [StringLength(11, ErrorMessage = "O telefone 1 deve ter no máximo 11 dígitos.")]
        public string? Telefone { get; set; }

        [StringLength(11, ErrorMessage = "O telefone whatsapp deve ter no máximo 11 dígitos.")]
        public string? Whatsapp { get; set; }

        // [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool IsAtivo { get; set; } = true;
        public DateTime? DataDesativacao { get; set; }
        public virtual ICollection<TutorAnimal>? TutoresAnimais { get; set; }
        public virtual ICollection<Consulta>? Consultas { get; set; }

        // Endereço
        [StringLength(8, ErrorMessage = "O CEP deve ter no máximo 8 dígitos.")]
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }

        [StringLength(2, ErrorMessage = "Unidade Federativa deve ter no máximo 2 letras.")]
        public string? Uf { get; set; }
    }
}
