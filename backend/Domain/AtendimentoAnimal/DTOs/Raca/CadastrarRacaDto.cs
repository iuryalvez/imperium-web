using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class CadastrarRacaDto
    {
        [Required(ErrorMessage = "O nome da raça é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        public EspecieAnimal Especie { get; set; }

        public double? PesoMinimoReferenciaFemea { get; set; }
        public double? PesoMaximoReferenciaFemea { get; set; }
        public double? PesoMinimoReferenciaMacho { get; set; }
        public double? PesoMaximoReferenciaMacho { get; set; }
        public CategoriaMetabolica? CategoriaMetabolica { get; set; }
    }
}
