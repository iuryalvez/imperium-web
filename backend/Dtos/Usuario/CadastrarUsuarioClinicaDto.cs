using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Usuario
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

        [Required]
        public string? Senha { get; set; }
        public DateTime DataCad { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public bool IsAdmin { get; set; }
        public int ClinicaId { get; set; }
        public List<string>? Perfis { get; set; }
    }
}