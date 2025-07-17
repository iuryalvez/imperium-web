using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities {
    public class AcompanhamentoInd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public Animal? Animal { get; set; }
        public virtual ICollection<DietaPersonalizada>? DietasPersonalizadas { get; set; }
        public virtual ICollection<Relatorio>? Relatorios { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }
    }
}
