using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Relatorio
    {
        public int Id { get; set; }
        public byte Idade { get; set; }
        public float Peso { get; set; }
        public byte Condicao_corporal { get; set; }
        public byte Grau_atividade { get; set; } // 0-inativo, 1-moderado, 2-ativo
        public string Desc_dieta { get; set; } = string.Empty;
        // Foreign Key para ConsultaId
        public int ConsultaId { get; set; }
        // Um Relatorio para uma Consulta
        public Consulta? Consulta { get; set; }
        public int RacaoId { get; set; }
        public Racao? Racao { get; set; }
    }
}