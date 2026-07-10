using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Usuarios.DTOs
{
    public class CadastrarUsuarioClinicaDto
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; }
        public string? UserName { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O status se é administrador ou não é obrigatório.")]
        public bool IsAdmin { get; set; }
        [Required(ErrorMessage = "O status se é administrador ou não é obrigatório.")]
        public bool IsAtivo { get; set; }

        [Required(ErrorMessage = "A clínica vinculada é obrigatória.")]
        public int ClinicaId { get; set; }

        [Required(ErrorMessage = "Os perfis são obrigatórios.")]
        public List<string> Perfis { get; set; }
    }
}