using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Acompanhamento_ind
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public int Dieta_personalizadaId { get; set; }
        public Dieta_personalizada? Dieta_Personalizada { get; set; }
        public int ConsultaId { get; set; }
        public Consulta? Consulta { get; set; }
        public DateOnly Data_inicio { get; set; }
        public DateOnly Data_fim { get; set; }
    }
}