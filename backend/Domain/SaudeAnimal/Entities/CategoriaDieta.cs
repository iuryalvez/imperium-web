using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.SaudeAnimal.Entities
{
    public class CategoriaDieta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty; // Manutenção, Crescimento, Lactação, Gestação
        
        public bool IsAtivo { get; set; } = true;
    }
}