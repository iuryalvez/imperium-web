using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Auth.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A verificação do captcha é obrigatória.")]
        public string CaptchaToken { get; set; } = string.Empty;
    }
}
