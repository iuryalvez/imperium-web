using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class Usuario : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCad { get; set; } = DateTime.UtcNow;
        public DateTime DataUltimoAcesso { get; set; }
        public string Formacao { get; set; } = string.Empty;
        public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; }
        public virtual ICollection<Consulta>? Consultas { get; set; }
    }
}
