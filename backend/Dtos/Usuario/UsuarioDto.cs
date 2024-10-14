using backend.Dtos.UsuarioPerfilClinica;

namespace backend.Dtos.Usuario
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataCad { get; set; } = DateTime.UtcNow;
        public DateTime DataUltimoAcesso { get; set; }
        public bool Ativo { get; set; }
        public string Formacao { get; set; } = string.Empty;
        // public Senha? Senha { get; set; }
        public virtual ICollection<UsuarioPerfilClinicaDto>? UsuariosPerfisClinicas { get; set; }
    }
}