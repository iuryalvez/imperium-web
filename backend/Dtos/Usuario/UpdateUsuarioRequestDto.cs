using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Usuario
{
    public class UpdateUsuarioRequestDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // public DateTime DataCad { get; set; } = DateTime.UtcNow;
        public DateTime DataUltimoAcesso { get; set; }
        public bool Ativo { get; set; }
        public string Formacao { get; set; } = string.Empty;
        // public Senha? Senha { get; set; }
        // public virtual ICollection<UsuarioPerfilClinica>? UsuariosPerfisClinicas { get; set; } = new List<UsuarioPerfilClinica>(); // referência de navegação com o dependente
    }
}