using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Racao
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float Umidade { get; set; }
        // unidade: utilizar % ou g
        public float Pb { get; set; }
        public float Eea { get; set; }
        public float Mm { get; set; }
        public float Fb { get; set; }
        // MS calculada ou determina
    }
}