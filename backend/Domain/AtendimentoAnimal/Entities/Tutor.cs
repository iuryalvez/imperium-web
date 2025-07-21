using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.AtendimentoAnimal.Entities {
    public class Tutor { // TODO: depois retirar o string.Empty dos atributos obrigatórios
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome do tutor é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do tutor deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        // [Required(ErrorMessage = "O CPF do tutor é obrigatório.")]
        [StringLength(11, ErrorMessage = "O CPF do tutor deve ter no máximo 11 dígitos.")]
        public string Cpf { get; set; } = string.Empty;
        
        [StringLength(300, ErrorMessage = "O nome do tutor deve ter no máximo 300 caracteres.")]
        public string? Endereco { get; set; }
        public int EndNumero { get; set; } 
        public string? EndEstado { get; set; }
        public string? EndCidade { get; set; }
        
        [StringLength(8, ErrorMessage = "O CEP deve ter no máximo 8 dígitos.")]
        public string? Cep { get; set; }
        
        [StringLength(11, ErrorMessage = "O telefone 1 deve ter no máximo 11 dígitos.")]
        public string? Telefone1 { get; set; }
        
        [StringLength(11, ErrorMessage = "O telefone whatsapp deve ter no máximo 11 dígitos.")]
        public string? Whatsapp { get; set; }
        
        // [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }
        public DateTime DataCad { get; set; }
        public virtual ICollection<TutorClinica>? TutoresClinicas { get; set; }
        public virtual ICollection<TutorAnimal>? TutoresAnimais { get; set; }
        public virtual ICollection<Consulta>? Consultas { get; set; }
    }
}
