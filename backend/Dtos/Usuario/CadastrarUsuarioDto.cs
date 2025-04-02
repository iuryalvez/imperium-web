using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class CadastrarUsuarioDto : IdentityUser<int>
    {
        public string? Nome { get; set; }
        public string? Senha { get; set; }
        public DateTime DataCad { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public bool IsAdmin { get; set; }
        public int ClinicaId { get; set; }
        public List<string>? Perfis { get; set; }
    }
}