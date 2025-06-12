using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal
{
    public class TutoresClinicasDto
    {
        [Required(ErrorMessage = "O id do tutor é obrigatório.")]
        public int TutorId { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }
    }
}