using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Raca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public EspecieAnimal Especie { get; set; }

        public float? PesoMinimoReferencia { get; set; }
        public float? PesoMaximoReferencia { get; set; }

        [JsonIgnore] // Avoids circular references in API responses
        public ICollection<Animal>? Animais { get; set; }
    }
}