using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.TutorAnimalClinica
{
    public class TutorAnimalClinicaDto
    {
        [Required(ErrorMessage = "O id do tutor é obrigatório.")]
        public int TutorId { get; set; }

        [Required(ErrorMessage = "O id do animal é obrigatório.")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }
    }
}