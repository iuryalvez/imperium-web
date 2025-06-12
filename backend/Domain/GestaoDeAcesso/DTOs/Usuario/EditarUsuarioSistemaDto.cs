using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace backend.Domain.GestaoDeAcesso.DTOs
{
    public class EditarUsuarioSistemaDto
    {
        [Required(ErrorMessage = "O id do usuário é obrigatório.")]
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public int UpcId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAtivo { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; }
        public List<string>? Perfis { get; set; } 
    }
}