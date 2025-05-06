using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioClinicaDto
    {
        [Required(ErrorMessage = "O id do usuário é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O id da clínica vinculada é obrigatório.")]
        public int ClinicaId { get; set; }

        [Required(ErrorMessage = "O id do upc é obrigatório.")]
        public int UpcId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;
        public List<string> Perfis { get; set; }
    }
}