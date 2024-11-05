using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace backend.Models {
    public class Perfil : IdentityRole<int> {
        public string Descricao { get; set; } = string.Empty;
        public int Tipo { get; set; } // 1-adm, 2-gestor, 3-vet/zoot, 4-secretario, 5-tecnico
        public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; }
    }
}
