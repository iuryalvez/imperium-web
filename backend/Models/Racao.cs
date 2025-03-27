using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Racao {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float Umidade { get; set; }
        public float Pb { get; set; } // unidade: utilizar % ou g
        public float Ee { get; set; }
        public float Mm { get; set; }
        public float Fb { get; set; }
        public float Ms { get; set; } // MS calculada ou determina
        public DietaPersonalizada? DietaPersonalizada { get; set; } // referência de navegação do dependente
    }
}
