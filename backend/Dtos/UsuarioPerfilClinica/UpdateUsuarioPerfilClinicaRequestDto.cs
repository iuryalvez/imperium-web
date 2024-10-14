using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.UsuarioPerfilClinica
{
    public class UpdateUsuarioPerfilClinicaRequestDto
    {
                public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PerfilId { get; set; }
        public int ClinicaId { get; set; }
    }
}