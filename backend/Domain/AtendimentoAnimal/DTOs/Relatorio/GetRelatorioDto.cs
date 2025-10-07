using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.DTOs.Dieta;

namespace backend.Domain.AtendimentoAnimal.DTOs.Relatorio
{
    public class GetRelatorioDto
    {
        public int Id { get; set; }
        public int ConsultaId { get; set; }
        public DateTime DataConsulta { get; set; }
        public Sexo SexoAnimal { get; set; }
        public int Idade { get; set; }
        public double Peso { get; set; }
        public int CondicaoCorporal { get; set; }
        public GrauAtividade GrauAtividade { get; set; } 
        public int RacaoId { get; set; }
        public string NomeRacao { get; set; } // Nome para exibição
        public string? DescricaoDieta { get; set; }
        public GetDietaDto? Dieta { get; set; }
        public int TipoDieta { get; set; }
        public bool IsGestante { get; set; }
        public int IsLactanteNumeroFilhotes { get; set; }
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
    }
}