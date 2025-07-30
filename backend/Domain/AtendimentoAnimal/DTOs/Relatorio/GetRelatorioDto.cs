using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Domain.AtendimentoAnimal.DTOs.Relatorio
{
    public class GetRelatorioDto
    {
        public int Id { get; set; }
        public int ConsultaId { get; set; }

        public int Idade { get; set; }
        public float Peso { get; set; }
        public string CondicaoCorporal { get; set; } // Retornado como string
        public string GrauAtividade { get; set; } // Retornado como string

        public int RacaoId { get; set; }
        public string NomeRacao { get; set; } // Nome para exibição

        public string? DescricaoDieta { get; set; }

        public bool IsGestante { get; set; }
        public int IsLactanteNumeroFilhotes { get; set; }
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
    }
}