using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal
{
    public class TutoresAnimaisDto
    {
        [Required(ErrorMessage = "O id do tutor é obrigatório.")]
        public int TutorId { get; set; }
        public int AnimalId { get; set; }
    }
}