using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities {
    public class TutorClinica {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O id do tutor é obrigatório.")]
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
        public DateTime DataAssociacao { get; set; }
    }
}
