using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Auth.DTOs
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "O e-mail de recuperação de senha é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "O token de recuperação de senha é obrigatório")]
        public string Token { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        public string NewPassword { get; set; }
    }
}