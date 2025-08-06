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

        public float QtdAlimento { get; set; }
        public float EnergiaMetabolizavel { get; set; }
        public float PesoAtual { get; set; }
        public float PesoObjetivo { get; set; }
        public float PesoIdeal { get; set; }
    }
}