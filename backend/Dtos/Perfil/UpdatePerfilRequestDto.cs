using backend.Models;

namespace backend.Dtos.Perfil {
    public class UpdatePerfilRequestDto {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Tipo { get; set; } // 1-adm, 2-gestor, 3-vet/zoot, 4-secretario, 5-tecnico
        // public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; } = new List<UsuarioPerfilClinica>(); // referência de navegação com o dependente
    }
}
