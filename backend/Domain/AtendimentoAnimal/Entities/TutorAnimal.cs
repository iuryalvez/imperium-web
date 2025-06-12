using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.AtendimentoAnimal.Entities {
    public class TutorAnimal {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O id do tutor é obrigatório.")]
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public DateTime DataAssociacao { get; set; }
    }
}
