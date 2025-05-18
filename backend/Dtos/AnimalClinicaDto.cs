using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.TutorAnimalClinica
{
    public class AnimalClinicaDto
    {
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }
    }
}