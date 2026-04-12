using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Entities
{
    public class FormulaCalculo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CategoriaDietaId { get; set; }
        [ForeignKey("CategoriaDietaId")]
        public virtual CategoriaDieta? CategoriaDieta { get; set; }

        public EspecieAnimal Especie { get; set; }
        public ParametrosFormula Parametros { get; set; } = new();

        // OPCIONAL: Se quiser total liberdade futura, você pode armazenar a expressão matemática como string
        // e usar uma biblioteca como o 'NCalc' no C# para resolver.
        // Exemplo: "(ConstanteA * Pow(Peso, ConstanteB)) + (26 * Peso)"
        [StringLength(500)]
        public string? ExpressaoMatematica { get; set; }
    }
}