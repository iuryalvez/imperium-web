using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.SaudeAnimal.DTOs.Dieta;

namespace backend.Domain.AtendimentoAnimal.DTOs.AcompanhamentoDto
{
    public class AnimalCompletoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Especie { get; set; }
        public string Sexo { get; set; }
        public DateTime Nascimento { get; set; }
        public bool IsCastrado { get; set; }
        public string IdadeFormatada { get; set; }
        public string FaseVidaAtual { get; set; }
        public string FaseVidaAtualLabel { get; set; } = string.Empty;
        public double? PesoAtual { get; set; }
        public string CondicaoCorporalAtual { get; set; }
        public string CondicaoCorporalAtualLabel { get; set; } = string.Empty;
        public int? IsLactanteNumeroFilhotes { get; set; }
        public int? SemanaLactacao { get; set; }
        public int? SemanaGestacao { get; set; }

        public RacaResumidaDto Raca { get; set; }
        public List<TutorResumidoDto>? Tutores { get; set; }
        public AcompanhamentoDto Acompanhamento { get; set; }
        public List<ConsultaDetalhadaDto>? Consultas { get; set; }
    }

    public class RacaResumidaDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
    }

    public class TutorResumidoDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
    }

    public class AcompanhamentoDto
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; }
    }

    public class ConsultaDetalhadaDto
    {
        public int Id { get; set; }
        public DateTime DataConsulta { get; set; }
        public int TutorId { get; set; }
        public string? ProfissionalResponsavel { get; set; }
        public AvaliacaoDetalhadoDto? Avaliacao { get; set; }

        /// <summary>Pendente, EmAndamento, Concluida ou Cancelada.</summary>
        public string Status { get; set; } = string.Empty;
        public string? MotivoCancelamento { get; set; }
    }

    public class AvaliacaoDetalhadoDto
    {
        public int Id { get; set; }

        // Dados da avaliação
        public double Peso { get; set; }
        public double? PesoIdeal { get; set; }
        public int? CondicaoCorporal { get; set; }
        public int? GrauAtividade { get; set; }
        public int Idade { get; set; }
        public int RacaoId { get; set; }
        public string? NomeRacao { get; set; }
        public string? DescricaoDieta { get; set; }
        public int TipoDieta { get; set; }
        public bool IsGestante { get; set; }
        public int IsLactanteNumeroFilhotes { get; set; }
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }

        public DietaResumidaDto Dieta { get; set; }
    }

    public class DietaResumidaDto
    {
        public int Id { get; set; }
        public int TipoDieta { get; set; }
        public string TipoDietaLabel { get; set; } = string.Empty;
        public double QuantidadeAlimentoGramas { get; set; }
        public double NecessidadeEnergeticaKcal { get; set; }
        public string? RacaoUtilizada { get; set; }
        public string? Observacoes { get; set; }
        public DateOnly DataCriacao { get; set; }
        public double? PesoAtual { get; set; }
        public double? PesoObjetivo { get; set; }
        public double? PesoIdeal { get; set; }
        public double FatorK { get; set; }
        public double Expoente { get; set; }
        public string? PerfilNutricional { get; set; }
        public PerfilMetabolico? PerfilMetabolico { get; set; }
        public double? MultiplicadorEnergetico { get; set; }
        public double? Potencia { get; set; }
        public bool IsCustomizado { get; set; }
        public bool HasAdicionalGestacaoCao { get; set; }
        public int? NumeroFilhotes { get; set; }
        public int? SemanaLactacao { get; set; }
        public int? SemanaGestacao { get; set; }
        public string? PrevisoesJson { get; set; }
        public int? DuracaoEstimada { get; set; }

        // Ciclo de vida
        public int Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataConclusao { get; set; }
        public int? SemanaAtual { get; set; }

        // Emagrecimento
        public int? NivelRestricao { get; set; }
        public string? NivelRestricaoLabel { get; set; }
        public double? FatorRestricao { get; set; }
        public double? PercentualPerdaSemanal { get; set; }
        public double? PerdaSemanalKg { get; set; }
        public List<GetRegistroPesoDto> RegistrosPeso { get; set; } = new();
    }
}