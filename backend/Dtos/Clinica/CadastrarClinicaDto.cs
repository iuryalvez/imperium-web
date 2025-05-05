using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Clinica
{
    public class CadastrarClinicaDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "A clínica deve ter um nome entre 0 e 100 caracteres.")]
        public string NomeClinica { get; set; }
        public bool IsAtiva { get; set; }
    }
}