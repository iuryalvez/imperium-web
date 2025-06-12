using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.SaudeAnimal.Entities {
    public class Racao {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A umidade da ração é obrigatória")]
        public float Umidade { get; set; }

        [Required(ErrorMessage = "A proteína bruta da ração é obrigatória.")]
        public float ProteinaBruta { get; set; } // unidade: utilizar % ou g
        
        [Required(ErrorMessage = "O extrato etéreo da ração é obrigatório.")]
        public float ExtratoEtereo { get; set; }

        [Required(ErrorMessage = "A matéria mineral da ração é obrigatória.")]
        public float MateriaMineral { get; set; }

        [Required(ErrorMessage = "A fibra bruta da ração é obrigatória.")]
        public float FibraBruta { get; set; }
        public float MateriaSeca { get; set; } // MS calculada ou determina
        public DietaPersonalizada? DietaPersonalizada { get; set; } // referência de navegação do dependente
    }
}
