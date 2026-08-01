using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Acompanhamento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public Animal? Animal { get; set; }
        public virtual ICollection<Avaliacao>? Avaliacoes{ get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }
    }
}
