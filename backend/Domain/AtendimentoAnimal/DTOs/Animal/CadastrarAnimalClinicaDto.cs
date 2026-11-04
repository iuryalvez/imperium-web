using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.DTOs.Animal
{
    public class CadastrarAnimalClinicaDto
    {
        [Required(ErrorMessage = "A raça do animal é obrigatória.")]
        public int RacaId { get; set; }

        [Required(ErrorMessage = "O sexo do animal é obrigatório.")]
        [EnumDataType(typeof(Sexo), ErrorMessage = "O sexo do animal é inválido.")]
        public Sexo Sexo { get; set; }

        [Required(ErrorMessage = "A espécie do animal é obrigatória.")]
        [EnumDataType(typeof(EspecieAnimal), ErrorMessage = "A espécie do animal é inválida.")]
        public EspecieAnimal Especie { get; set; }

        [Required(ErrorMessage = "O nome do animal é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe se o animal é castrado.")]
        public bool IsCastrado { get; set; }
        public DateTime Nascimento { get; set; }
        public List<int>? TutorIds { get; set; }
    }
}