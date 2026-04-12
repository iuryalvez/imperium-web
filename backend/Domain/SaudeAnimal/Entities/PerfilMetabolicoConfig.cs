using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.SaudeAnimal.Entities
{
    public class PerfilMetabolicoConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty; // Ex: Gato Doméstico Obeso, Cão Ativo
        
        // Coeficientes para a fórmula de Manutenção
        public double Multiplicador { get; set; }
        public double Potencia { get; set; }

        public virtual ICollection<PerfilMetabolicoRules>? Regras { get; set; }
        
        public bool IsAtivo { get; set; } = true;
    }
}