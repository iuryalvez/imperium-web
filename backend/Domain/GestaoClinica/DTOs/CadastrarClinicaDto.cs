using System.ComponentModel.DataAnnotations;

namespace backend.Domain.GestaoClinica.DTOs
{
    public class CadastrarClinicaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
        [StringLength(100, ErrorMessage = "A clínica deve ter um nome entre 0 e 100 caracteres.")]
        public string NomeClinica { get; set; }
        
        [Required(ErrorMessage = "O status da clínica é obrigatório.")]
        public bool IsAtiva { get; set; }
    }
}