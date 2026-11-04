using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    /// <summary>
    /// Cancelamento de consulta (PUT /api/clinica/consultas/{id}/cancelar). O motivo é opcional.
    /// </summary>
    public class CancelarConsultaDto
    {
        [StringLength(500, ErrorMessage = "O motivo do cancelamento deve ter no máximo 500 caracteres.")]
        public string? Motivo { get; set; }
    }
}
