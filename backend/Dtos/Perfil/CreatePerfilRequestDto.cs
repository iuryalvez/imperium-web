using backend.Dtos.UsuarioPerfilClinica;
using backend.Models;

namespace backend.Dtos.Perfil
{
    public class CreatePerfilRequestDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Tipo { get; set; } // 1-adm, 2-gestor, 3-vet/zoot, 4-secretario, 5-tecnico
        public virtual ICollection<UsuarioPerfilClinicaDto>? UsuariosPerfisClinicas { get; set; } // (1-N)
    }
}