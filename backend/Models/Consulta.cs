using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Models
{
    public class Consulta
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int TutorId { get; set; }
        public Tutor Tutor { get; set; } = null!;
        public Relatorio Relatorio { get; set; } = null!; // Reference navigation to dependent
    }
}