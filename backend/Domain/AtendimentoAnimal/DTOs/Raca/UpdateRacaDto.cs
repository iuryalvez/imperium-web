using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class UpdateRacaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da raça é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        public EspecieAnimal Especie { get; set; }

        public float? PesoMinimoReferenciaFemea { get; set; }
        public float? PesoMaximoReferenciaFemea { get; set; }
        public float? PesoMinimoReferenciaMacho { get; set; }
        public float? PesoMaximoReferenciaMacho { get; set; }
    }
}