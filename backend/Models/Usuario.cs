using Microsoft.AspNetCore.Identity;

namespace backend.Models {
    public class Usuario : IdentityUser {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; }
        public DateTime DataCad { get; set; } = DateTime.UtcNow;
        public DateTime DataUltimoAcesso { get; set; }
        public bool Ativo { get; set; }
        public string Formacao { get; set; } = string.Empty;
        public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; }
    }
}
