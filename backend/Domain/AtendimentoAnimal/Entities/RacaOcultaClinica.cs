using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    /// <summary>
    /// Raça do sistema que uma clínica escolheu ocultar só para ela (não aparece nos selects da clínica).
    /// </summary>
    public class RacaOcultaClinica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O id da raça é obrigatório.")]
        public int RacaId { get; set; }

        [ForeignKey("RacaId")]
        public Raca? Raca { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }

        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }

        public DateTime DataOcultacao { get; set; }
    }
}
