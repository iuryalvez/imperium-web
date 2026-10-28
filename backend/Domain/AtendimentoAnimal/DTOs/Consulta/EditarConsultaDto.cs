using System.ComponentModel.DataAnnotations;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    /// <summary>
    /// Edição de consulta (PUT /api/clinica/consultas/editar): os mesmos campos do cadastro + o Id.
    /// </summary>
    public class EditarConsultaDto : UpsertConsultaDto
    {
        [Required(ErrorMessage = "O id da consulta é obrigatório.")]
        public int Id { get; set; }
    }
}
