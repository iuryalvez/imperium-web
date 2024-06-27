using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Perfil
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public ICollection<UsuarioPerfil> UsuarioPerfis { get; set; } = new List<UsuarioPerfil>();
    }
}