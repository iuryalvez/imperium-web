using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.Usuarios.Entities;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Consulta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
        public int ClinicaId { get; set; }
        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }
        public int? TutorId { get; set; }
        [ForeignKey("TutorId")]
        public Tutor? Tutor { get; set; }
        public bool? TutorIsCadastrado { get; set; }
        public int AnimalId { get; set; } // Foreign Key for the specific animal
        [ForeignKey("AnimalId")]
        public Animal? Animal { get; set; } // Navigation property to the animal
        public DateTime DataConsulta { get; set; }
        public Avaliacao? Avaliacao { get; set; }

        // Cancelamento: a consulta cancelada continua registrada (não é exclusão) e pode ser reaberta
        public DateTime? DataCancelamento { get; set; }

        [StringLength(500, ErrorMessage = "O motivo do cancelamento deve ter no máximo 500 caracteres.")]
        public string? MotivoCancelamento { get; set; }

        public int? CanceladaPorUsuarioId { get; set; }
        [ForeignKey("CanceladaPorUsuarioId")]
        public Usuario? CanceladaPorUsuario { get; set; }

        /// <summary>
        /// Status calculado (precisa de Avaliacao e Avaliacao.Dieta carregadas). Só o cancelamento é gravado.
        /// </summary>
        [NotMapped]
        public StatusConsulta Status =>
            DataCancelamento != null ? StatusConsulta.Cancelada
            : Avaliacao?.Dieta != null ? StatusConsulta.Concluida
            : Avaliacao != null ? StatusConsulta.EmAndamento
            : StatusConsulta.Pendente;
    }
}
