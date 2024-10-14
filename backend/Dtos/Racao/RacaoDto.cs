using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Racao
{
    public class RacaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float Umidade { get; set; }
        public float Pb { get; set; } // unidade: utilizar % ou g
        public float Eea { get; set; }
        public float Mm { get; set; }
        public float Fb { get; set; }
        public float Ms { get; set; } // MS calculada ou determina
    }
}