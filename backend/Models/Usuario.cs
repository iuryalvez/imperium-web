using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models {

    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha_hash { get; set; } = string.Empty;
        public DateTime Data_cad { get; set; } = DateTime.Now;
        public DateTime Data_ultimoacesso { get; set; }
        public bool Habilitado { get; set; }
        public string Formacao { get; set; } = string.Empty;
        public virtual ICollection<UsuarioPerfil>? UsuarioPerfis { get; set; } = new List<UsuarioPerfil>();
        // Um usuário para n consultas 
        // public virtual ICollection<Consulta>? Consultas { get; set; } = new List<Consulta>(); // Propriedade de navegação para Consulta
    }
}