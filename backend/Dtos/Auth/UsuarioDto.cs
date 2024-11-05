using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Auth
{
    public class UsuarioDto : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public DateTime DataCad { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
    }
}