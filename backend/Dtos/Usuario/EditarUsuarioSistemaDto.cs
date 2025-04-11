using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioSistemaDto
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public List<string>? Perfis { get; set; } 
        public int ClinicaId { get; set; }
    }
}