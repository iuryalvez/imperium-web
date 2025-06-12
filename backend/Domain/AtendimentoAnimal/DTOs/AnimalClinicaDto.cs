using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal 
{
    public class AnimalClinicaDto
    {
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }
    }
}