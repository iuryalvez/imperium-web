using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Auth.DTOs {
    public class LoginDto {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
        public required string CaptchaToken { get; set; }
        public bool RememberMe { get; set; }
    }
}