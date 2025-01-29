using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class GetUsuariosDto : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public string Perfil { get; set; } = string.Empty;
        public bool Habilitado { get; set; }
    }
}