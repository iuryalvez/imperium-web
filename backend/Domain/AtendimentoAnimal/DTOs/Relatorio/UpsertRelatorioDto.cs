using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.DTOs.Relatorio
{
    public class UpsertRelatorioDto
    {
        // O ID da Consulta virá da rota da URL, não do corpo da requisição.

        [Required]
        public float Peso { get; set; }

        [Required]
        public CondicaoCorporal CondicaoCorporal { get; set; }

        [Required]
        public GrauAtividade GrauAtividade { get; set; }

        [Required]
        public int RacaoId { get; set; }

        [StringLength(2000)]
        public string? DescricaoDieta { get; set; }

        // Novos campos de status reprodutivo
        public bool IsGestante { get; set; }
        public int IsLactanteNumeroFilhotes { get; set; }
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
    }
}