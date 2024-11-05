using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Auth {
    public class LoginDto {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Senha { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}