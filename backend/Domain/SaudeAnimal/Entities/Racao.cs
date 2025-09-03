using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.SaudeAnimal.Entities {
    public class Racao {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClinicaId { get; set; }

        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }

        [Required]
        public EspecieAnimal Especie { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool IsPorcentagem { get; set; }
        
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
        public float? MateriaSeca { get; set; } // Retirar a umidade do cálculo do ENN
        public float? FibraDieteticaTotal { get; set; } // CDE = 95,6-(0,89 x porcentagem da FDT na matéria seca)
        public float? EnergiaMetabolizavel { get; set; }
        public ICollection<DietaPersonalizada>? DietasPersonalizadas { get; set; } // referência de navegação do dependente
        public ICollection<Relatorio>? Relatorios { get; set; } // referência de navegação do dependente

        // Atributos que serão calculados e armazenados
        public float ExtrativoNaoNitrogenado { get; set; }
        public float EnergiaBruta { get; set; }
        public float CoeficienteDigestibilidadeEnergia { get; set; }
        public float EnergiaDigestivel { get; set; }
        public float EnergiaMetabolizavel { get; set; }
    }
}
