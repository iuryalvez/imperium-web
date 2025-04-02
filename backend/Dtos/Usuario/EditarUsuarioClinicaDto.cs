using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioClinicaDto : IdentityUser<int>
    {
        public int UpcId { get; set; }
        public string? Nome { get; set; }
        public List<string>? Perfis { get; set; } 
    }
}