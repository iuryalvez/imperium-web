using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.DTOs
{
    public class CadastrarAnimalClinicaDto
    {
        [Required]
        public int RacaId { get; set; }

        [Required]
        public Sexo Sexo { get; set; }

        [Required]
        public EspecieAnimal Especie { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public bool IsCastrado { get; set; }
        public DateTime Nascimento { get; set; }
        public List<int>? TutorIds { get; set; }
    }
}