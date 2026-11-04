using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.DTOs.Animal
{
    public class EditarAnimalClinicaDto
    {
        [Required(ErrorMessage = "O id do animal é obrigatório.")]
        public int Id { get; set; }
        public List<int>? TutorIds { get; set; }

        // Campos que podem ser atualizados
        public string? Nome { get; set; }
        public int? RacaId { get; set; }
        [EnumDataType(typeof(EspecieAnimal), ErrorMessage = "A espécie do animal é inválida.")]
        public EspecieAnimal? Especie { get; set; }
        public DateTime? Nascimento { get; set; }
        [EnumDataType(typeof(Sexo), ErrorMessage = "O sexo do animal é inválido.")]
        public Sexo? Sexo { get; set; }
        public bool? IsCastrado { get; set; }
    }
}