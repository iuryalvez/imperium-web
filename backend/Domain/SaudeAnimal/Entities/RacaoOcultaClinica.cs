using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.SaudeAnimal.Entities
{
    /// <summary>
    /// Ração do sistema que uma clínica escolheu ocultar só para ela (não aparece nos selects da clínica).
    /// </summary>
    public class RacaoOcultaClinica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O id da ração é obrigatório.")]
        public int RacaoId { get; set; }

        [ForeignKey("RacaoId")]
        public Racao? Racao { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }

        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }

        public DateTime DataOcultacao { get; set; }
    }
}
