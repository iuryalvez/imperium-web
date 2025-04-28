using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioSistemaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public int UpcId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }
        public List<string>? Perfis { get; set; } 
    }
}