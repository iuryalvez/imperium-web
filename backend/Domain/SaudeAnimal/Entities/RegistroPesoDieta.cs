using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.SaudeAnimal.Entities
{
    /// <summary>
    /// Registro do peso real do animal em uma semana do plano de emagrecimento.
    /// </summary>
    public class RegistroPesoDieta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DietaId { get; set; }

        [ForeignKey("DietaId")]
        public Dieta? Dieta { get; set; }

        public int Semana { get; set; }
        public DateOnly DataRegistro { get; set; }
        public double PesoReal { get; set; }
        public string? Observacao { get; set; }
    }
}
