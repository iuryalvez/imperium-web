using System.ComponentModel.DataAnnotations;

namespace backend.Domain.SaudeAnimal.DTOs.Dieta
{
    public class CreateRegistroPesoDto
    {
        [Required]
        [Range(1, 15, ErrorMessage = "A semana deve estar entre 1 e 15.")]
        public int Semana { get; set; }

        public DateOnly? DataRegistro { get; set; }

        [Required]
        [Range(0.01, 500, ErrorMessage = "Informe um peso válido.")]
        public double PesoReal { get; set; }

        public string? Observacao { get; set; }
    }

    public class GetRegistroPesoDto
    {
        public int Id { get; set; }
        public int Semana { get; set; }
        public DateOnly DataRegistro { get; set; }
        public double PesoReal { get; set; }
        public string? Observacao { get; set; }
    }
}
