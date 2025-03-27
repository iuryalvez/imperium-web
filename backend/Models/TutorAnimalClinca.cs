using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class TutorAnimalClinica {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
    }
}
