using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Consulta {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }
        public DateTime DataConsulta { get; set; }
        public Relatorio? Relatorio { get; set; }
    }
}
