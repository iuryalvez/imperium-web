using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class UsuarioPerfil
    {
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int PerfilId { get; set; }
        public Perfil? Perfil { get; set; }
    }
}