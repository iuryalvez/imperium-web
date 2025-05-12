using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class Usuario : IdentityUser<int>
    {
        [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do usuário deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCad { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        
        [StringLength(60, ErrorMessage = "A formação do usuário deve ter no máximo 60 caracteres.")]
        public string? Formacao { get; set; }
        
        [StringLength(11, ErrorMessage = "O CPF do usuário deve ter no máximo 11 caracteres.")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "O status de administrador é obrigatório.")]
        public bool IsAdmin { get; set; }
        [Required(ErrorMessage = "O status de administrador é obrigatório.")]
        public bool IsAtivo { get; set; }

        [StringLength(11, ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        public string? Telefone1 { get; set; }
        
        [StringLength(11, ErrorMessage = "O telefone whatsapp deve ter no máximo 11 dígitos.")]
        public string? Whatsapp { get; set; }
        public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; }
        public virtual ICollection<Consulta>? Consultas { get; set; }
    }
}
