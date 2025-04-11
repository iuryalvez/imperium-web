using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioClinicaDto
    {
        public string? Nome { get; set; }
        public List<string>? Perfis { get; set; }
        public int ClinicaId { get; set; }
    }
}