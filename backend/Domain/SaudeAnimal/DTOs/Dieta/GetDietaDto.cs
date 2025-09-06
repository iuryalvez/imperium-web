using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.DTOs.Dieta
{
    public class GetDietaDto
    {
        public int Id { get; set; }
        public int RacaoId { get; set; }
        public string NomeRacao { get; set; } = string.Empty; // Facilita a exibição no frontend
        public int RelatorioId { get; set; }

        public TipoDieta TipoDieta { get; set; } // Mantido como enum, será serializado como número

        public double QtdAlimento { get; set; }
        public double EnergiaMetabolizavel { get; set; }
        public double PesoAtual { get; set; }
        public double PesoObjetivo { get; set; }
        public double PesoIdeal { get; set; }
    }
}