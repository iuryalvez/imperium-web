using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.DTOs.Dieta
{
    public class GetDietaDto
    {
        public int Id { get; set; }
        public int RacaoId { get; set; }
        public string NomeRacao { get; set; } = string.Empty; // Facilita a exibição no frontend
        public int AvaliacaoId { get; set; }

        public TipoDieta TipoDieta { get; set; } // Mantido como enum, será serializado como número

        public double QtdAlimento { get; set; }
        public double EnergiaMetabolizavel { get; set; }
        public double PesoAtual { get; set; }
        public double PesoObjetivo { get; set; }
        public double PesoIdeal { get; set; }

        public double FatorK { get; set; }
        public double Expoente { get; set; }
        public string PerfilNutricional { get; set; } = string.Empty;
        public PerfilMetabolico? PerfilMetabolico { get; set; }
        public double? Potencia { get; set; }
        public bool IsCustomizado { get; set; }
        public bool? HasAdicionalGestacaoCao { get; set; }
        public int? NumeroFilhotes { get; set; }
        public int? SemanaLactacao { get; set; }
        public int? SemanaGestacao { get; set; }
        public string? PrevisoesJson { get; set; }
        public int? DuracaoEstimada { get; set; }
        public string? Observacoes { get; set; }

        // Ciclo de vida
        public StatusDieta Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataConclusao { get; set; }
        public int? SemanaAtual { get; set; }

        // Emagrecimento
        public NivelRestricao? NivelRestricao { get; set; }
        public string? NivelRestricaoLabel { get; set; }
        public double? FatorRestricao { get; set; }
        public double? PercentualPerdaSemanal { get; set; }
        public double? PerdaSemanalKg { get; set; }
        public List<GetRegistroPesoDto> RegistrosPeso { get; set; } = new();
    }
}