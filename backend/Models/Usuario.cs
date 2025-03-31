using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class Usuario : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCad { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public string Formacao { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; }
        public virtual ICollection<Consulta>? Consultas { get; set; }
    }
}
