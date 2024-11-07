using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class GetUsuariosDto : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
    }
}