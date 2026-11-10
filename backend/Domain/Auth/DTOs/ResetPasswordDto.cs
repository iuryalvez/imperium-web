using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Auth.DTOs
{
    public class ResetPasswordDto
    {
        /// <summary>
        /// Id do usuário, vindo do link de recuperação (parâmetro uid). O e-mail não vai na URL.
        /// Pode ser negativo: os usuários do seed têm id negativo. Id inexistente cai na mensagem genérica do service.
        /// </summary>
        public int UserId { get; set; }

        [Required(ErrorMessage = "O token de recuperação de senha é obrigatório.")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
