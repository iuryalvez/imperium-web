using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.GestaoClinica.Entities {
    public class Clinica {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da clínica deve ter no máximo 100 caracteres.")]
        public string? NomeClinica { get; set; }
        
        [StringLength(100, ErrorMessage = "O nome fantasia da clínica deve ter no máximo 100 caracteres.")]
        public string? NomeFantasia { get; set; }
        
        [StringLength(14, ErrorMessage = "O CNPJ deve ter no máximo 14 dígitos.")]
        public string? Cnpj { get; set; }
        
        [StringLength(300, ErrorMessage = "O endereço deve ter no máximo 300 caracteres.")]
        public string? Endereco { get; set; }
        
        public int EndNumero { get; set; }
        public string? EndEstado { get; set; }
        public string? EndCidade { get; set; }
        
        [StringLength(9, ErrorMessage = "O CEP deve ter no máximo 9 dígitos.")]
        public string? Cep { get; set; }
        
        [StringLength(11, ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        public string? Telefone1 { get; set; }
        
        [StringLength(11, ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        public string? Telefone2 { get; set; }
        
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }
        
        [StringLength(11, ErrorMessage = "O telefone whatsapp deve ter 11 dígitos")]
        public string? Whatsapp { get; set; } 
        
        [StringLength(30, ErrorMessage = "O instagram da clínica deve ter no máximo 30 caracteres.")]
        public string? Instagram { get; set; }   
        public DateTime DataCad { get; set; }
        
        [Required(ErrorMessage = "O status da clínica é obrigatório.")]
        public bool IsAtiva { get; set; }
        public string? LogoUrl { get; set; }
        public virtual ICollection<Consulta>? Consultas { get; set; }
        public virtual ICollection<Racao>? Racoes { get; set; }
        public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; }
        public virtual ICollection<TutorClinica>? TutoresClinicas { get; set; }
        public virtual ICollection<AnimalClinica>? AnimaisClinicas { get; set; }
    }
}
