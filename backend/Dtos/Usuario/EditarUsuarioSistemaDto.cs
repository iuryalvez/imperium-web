using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioSistemaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public int UpcId { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public List<string>? Perfis { get; set; } 
    }
}