using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Vinculo
{
    public class GetVinculoAdminDto
    {
        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }

        [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
        public string NomeClinica { get; set; }

        [Required(ErrorMessage = "Os perfis são obrigatórios.")]
        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}