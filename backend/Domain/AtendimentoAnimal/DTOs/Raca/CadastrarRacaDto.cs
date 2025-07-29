using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class CadastrarRacaDto
    {
        [Required(ErrorMessage = "O nome da raça é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        public EspecieAnimal Especie { get; set; }

        public float? PesoMinimoReferencia { get; set; }
        public float? PesoMaximoReferencia { get; set; }
    }
}
