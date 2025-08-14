using System.ComponentModel.DataAnnotations;

namespace backend.Domain.GestaoDeAcesso.DTOs
{
    public class CadastrarUsuarioSistemaDto
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; }
        public string? UserName { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter até 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O status de administrador é obrigatório.")]
        public bool IsAdmin { get; set; }
        [Required(ErrorMessage = "O status de administrador é obrigatório.")]
        public bool IsAtivo { get; set; }
        public List<int>? ClinicaIds { get; set; }
        public List<string>? Perfis { get; set; }
    }
}
