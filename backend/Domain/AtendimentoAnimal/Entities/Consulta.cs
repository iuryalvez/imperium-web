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
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
        public int ClinicaId { get; set; }
        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }
        public int TutorId { get; set; }
        [ForeignKey("TutorId")]
        public Tutor? Tutor { get; set; }
        public string? TutorNome { get; set; }
        public string? TutorCpf { get; set; }
        public bool TutorIsCadastrado { get; set; }
        public int AnimalId { get; set; } // Foreign Key for the specific animal
        [ForeignKey("AnimalId")]
        public Animal? Animal { get; set; } // Navigation property to the animal
        public DateTime DataConsulta { get; set; }
        public Relatorio? Relatorio { get; set; }
    }
}
