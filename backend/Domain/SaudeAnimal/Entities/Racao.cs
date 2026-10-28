using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.SaudeAnimal.Entities
{
    public class Racao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ClinicaId { get; set; } // null = ração do sistema (compartilhada entre as clínicas)

        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }

        [Required]
        public EspecieAnimal Especie { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool IsPorcentagem { get; set; }

        [Required(ErrorMessage = "A umidade da ração é obrigatória")]
        public double Umidade { get; set; }

        [Required(ErrorMessage = "A proteína bruta da ração é obrigatória.")]
        public double ProteinaBruta { get; set; } // unidade: utilizar % ou g

        [Required(ErrorMessage = "O extrato etéreo da ração é obrigatório.")]
        public double ExtratoEtereo { get; set; }

        [Required(ErrorMessage = "A matéria mineral da ração é obrigatória.")]
        public double MateriaMineral { get; set; }

        [Required(ErrorMessage = "A fibra bruta da ração é obrigatória.")]
        public double FibraBruta { get; set; }
        public double? MateriaSeca { get; set; } // Retirar a umidade do cálculo do ENN
        public DateTime DataCadastro { get; set; }
        public bool IsAtivo { get; set; } = true;
        public DateTime? DataDesativacao { get; set; }
        public ICollection<RacaoOcultaClinica>? OcultaEmClinicas { get; set; } // clínicas que ocultaram esta ração do sistema
        public ICollection<Dieta>? Dietas { get; set; } // referência de navegação do dependente
        public ICollection<Avaliacao>? Avaliacoes{ get; set; } // referência de navegação do dependente

        // Atributos que serão calculados e armazenados
        public double? ExtrativoNaoNitrogenado { get; set; }
        public double? EnergiaBruta { get; set; }
        public double? FibraDieteticaTotal { get; set; } // CDE = 95,6-(0,89 x porcentagem da FDT na matéria seca)
        public double? CoeficienteDigestibilidadeEnergia { get; set; }
        public double? EnergiaDigestivel { get; set; }
        public double? EnergiaMetabolizavel { get; set; }

        // Novos atributos para distribuição energética
        public double? EnergiaProteina { get; set; } // kcal/100g
        public double? EnergiaExtratoEtereo { get; set; }
        public double? EnergiaExtrativoNaoNitrogenado { get; set; }
        public double? EnergiaTotalEstimativa { get; set; }
    }
}
