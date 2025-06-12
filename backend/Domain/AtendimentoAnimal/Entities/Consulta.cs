using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoDeAcesso.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Consulta
    {
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
