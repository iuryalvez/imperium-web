using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioClinicaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public int UpcId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }
        public List<string>? Perfis { get; set; }
    }
}