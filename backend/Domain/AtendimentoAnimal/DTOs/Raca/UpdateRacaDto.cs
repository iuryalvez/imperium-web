using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class UpdateRacaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da raça é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da raça deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        [EnumDataType(typeof(EspecieAnimal), ErrorMessage = "A espécie é inválida.")]
        public EspecieAnimal Especie { get; set; }

        public double? PesoMinimoReferenciaFemea { get; set; }
        public double? PesoMaximoReferenciaFemea { get; set; }
        public double? PesoMinimoReferenciaMacho { get; set; }
        public double? PesoMaximoReferenciaMacho { get; set; }
        public CategoriaMetabolica? CategoriaMetabolica { get; set; }
    }
}