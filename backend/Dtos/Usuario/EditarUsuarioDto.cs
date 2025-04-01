using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioDto : IdentityUser<int>
    {
        public int UpcId { get; set; }
        public string? Nome { get; set; }
        public Dictionary<string, bool>? Perfis { get; set; }
    }
}