using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Vinculo
{
    public class GetVinculoDto
    {
        [Required(ErrorMessage = "O id do upc é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O id do usuário é obrigatório.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }

        [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
        public string? NomeClinica { get; set; }
        // lista para armazenar os perfis do usuário na clínica

        [Required(ErrorMessage = "Os perfis são obrigatórios.")]
        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}