using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.DTOs.Relatorio
{
    public class UpsertRelatorioDto
    {
        // O ID da Consulta virá da rota da URL, não do corpo da requisição.

        [Required]
        public double Peso { get; set; }

        [Required]
        public CondicaoCorporal CondicaoCorporal { get; set; }

        [Required]
        public GrauAtividade GrauAtividade { get; set; }

        [Required]
        public int RacaoId { get; set; }

        [StringLength(2000)]
        public string? DescricaoDieta { get; set; }
        public int TipoDieta { get; set; }
        public bool IsGestante { get; set; }
        public int IsLactanteNumeroFilhotes { get; set; }
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
    }
}