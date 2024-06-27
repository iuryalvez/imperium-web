using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Dieta_personalizada
    {
        public int Id { get; set; }
        public byte Tipo_de_dieta { get; set; } // 1-manutenção, 2-crescimento, 3-lactação
        public float Qtd_alimento { get; set; } // calculado
        public float Energia_metabolizavel { get; set; } // calculado
        public float Peso_atual { get; set; }
        public float Peso_objetivo { get; set; }
        public float Peso_ideal { get; set; }
    }
}