using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Entities
{
    public class PerfilMetabolicoRules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PerfilMetabolicoConfigId { get; set; }

        [ForeignKey("PerfilMetabolicoConfigId")]
        public virtual PerfilMetabolicoConfig? PerfilMetabolico { get; set; }

        // Ordem de avaliação (regras mais específicas devem ter prioridade menor/maior)
        public int OrdemPrioridade { get; set; } 

        // Critérios (Se nulo, significa que a regra ignora este filtro)
        public EspecieAnimal? Especie { get; set; }
        public FaseVidaAnimal? FaseVida { get; set; }
        public GrauAtividade? GrauAtividade { get; set; }
        
        // Pode usar um valor numérico para checar ranges
        public int? CondicaoCorporalMin { get; set; } 
        public int? CondicaoCorporalMax { get; set; }

        // Para regras específicas de raças (ex: "Dogue Alemão")
        [StringLength(100)]
        public string? RacaContem { get; set; } 
    }
}