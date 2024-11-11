using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Racao
{
    public class CreateRacaoDto
    {
        [Required]
        public int ClinicaId { get; set; }
        [Required]
        public string Nome { get; set; } = string.Empty;
        [Required]
        public float Umidade { get; set; }
        [Required]
        public float Pb { get; set; } // unidade: utilizar % ou g
        [Required]
        public float Ee { get; set; }
        [Required]
        public float Mm { get; set; }
        [Required]
        public float Fb { get; set; }
        // [Required]
        // public float Ms { get; set; } // MS calculada ou determinada
    }
}