using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class CadastrarUsuarioDto : IdentityUser<int>
    {
        public string? Nome { get; set; }
        public string? Senha { get; set; }
        public int PerfilId { get; set; }
        public DateTime DataCad { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
    }
}