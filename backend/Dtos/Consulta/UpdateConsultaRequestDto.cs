using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Consulta
{
    public class UpdateConsultaRequestDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int TutorId { get; set; }
        public int AcompanhamentoIndId { get; set; }
        public DateOnly Date { get; set; }
    }
}