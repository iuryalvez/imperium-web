using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Animal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O id da clínica é obrigatório.")]
        public int ClinicaId { get; set; }

        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }
        public string? Nome { get; set; }
        public EspecieAnimal Especie { get; set; }
        public int? RacaId { get; set; }

        [ForeignKey("RacaId")]
        public Raca? Raca { get; set; }
        public DateTime Nascimento { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataModificado { get; set; }
        public bool IsAtivo { get; set; } = true;
        public DateTime? DataDesativacao { get; set; }
        public Sexo Sexo { get; set; }
        public double? Peso { get; set; }
        public double? PesoMinimo { get; set; }
        public double? PesoMaximo { get; set; }
        public double? PesoIdeal { get; set; }
        public CondicaoCorporal? CondicaoCorporal { get; set; } // valores: 1-9
        public GrauAtividade? GrauAtividade { get; set; } // 1-inativo, 2-moderado, 3-ativo
        public bool? IsCastrado { get; set; }
        public bool? IsGestante { get; set; }
        public int? IsLactanteNumeroFilhotes { get; set; } // numero de filhotes
        public int? SemanaLactacao { get; set; }
        public int? SemanaGestacao { get; set; }
        public PerfilMetabolico PerfilCalculado { get; set; } // salva o perfil usado
        public double? NecessidadeEnergeticaDiaria { get; set; } // resultado kcal calculado 
        public Acompanhamento? Acompanhamento { get; set; } // referência de navegação com o dependente
        public ICollection<Consulta>? Consultas { get; set; } // navegation
        public ICollection<TutorAnimal>? TutoresAnimais { get; set; }

        [NotMapped] // Informa ao EF Core para NÃO criar esta coluna no banco
        public int Idade
        {
            get
            {
                // Certifique-se de que Nascimento está em Hora Local ou ajuste para UtcNow.Date se Nascimento for UTC.
                // Assumindo que Nascimento é a data de nascimento local.
                var hoje = DateTime.Today;
                var idade = hoje.Year - Nascimento.Year;

                // Ajusta a idade se o aniversário ainda não ocorreu este ano
                if (Nascimento.Date > hoje.AddYears(-idade))
                {
                    idade--;
                }
                return idade < 0 ? 0 : idade; // Garante que a idade não seja negativa
            }
        }

        [NotMapped]
        public FaseVidaAnimal FaseVida
        {
            get
            {
                int idadeAtual = this.Idade;

                switch (this.Especie)
                {
                    case EspecieAnimal.Cao:
                        // Deteção de porte: Cães >= 25kg (Grandes/Gigantes) envelhecem mais rápido segundo a AAHA
                        double pesoReferencia = this.PesoMaximo ?? this.PesoIdeal ?? this.Peso ?? 0;
                        bool isGrandePorte = pesoReferencia >= 25.0;

                        if (idadeAtual < 1) return FaseVidaAnimal.Filhote;       // 0 a 11 meses
                        if (idadeAtual <= 2) return FaseVidaAnimal.Jovem;        // 1 a 2 anos

                        if (isGrandePorte)
                        {
                            if (idadeAtual <= 5) return FaseVidaAnimal.Adulto;   // 3 a 5 anos
                            if (idadeAtual <= 8) return FaseVidaAnimal.Senior;   // 6 a 8 anos
                            return FaseVidaAnimal.Geriatrico;                    // 9+ anos
                        }
                        else
                        {
                            if (idadeAtual <= 6) return FaseVidaAnimal.Adulto;   // 3 a 6 anos
                            if (idadeAtual <= 10) return FaseVidaAnimal.Senior;  // 7 a 10 anos
                            return FaseVidaAnimal.Geriatrico;                    // 11+ anos
                        }

                    case EspecieAnimal.Gato:
                        // Diretrizes oficiais AAFP (American Association of Feline Practitioners)
                        if (idadeAtual < 1) return FaseVidaAnimal.Filhote;       // 0 a 11 meses
                        if (idadeAtual <= 6) return FaseVidaAnimal.Jovem;        // 1 a 6 anos (Young Adult)
                        if (idadeAtual <= 10) return FaseVidaAnimal.Adulto;      // 7 a 10 anos (Mature Adult)
                        if (idadeAtual <= 14) return FaseVidaAnimal.Senior;      // 11 a 14 anos (Senior)
                        return FaseVidaAnimal.Geriatrico;                        // 15+ anos (Super Senior)

                    default:
                        return FaseVidaAnimal.Desconhecida;
                }
            }
        }

        [NotMapped]
        public double PesoReferencia
        {
            get
            {
                // 1. Tenta o peso ideal/alvo individual
                if (PesoIdeal.HasValue && PesoIdeal > 0) return PesoIdeal.Value;

                // 2. Tenta o peso máximo individualizado
                if (PesoMaximo.HasValue && PesoMaximo > 0) return PesoMaximo.Value;

                // 3. Tenta o peso médio da raça de acordo com o sexo
                double? pesoRaca = Sexo == Sexo.Macho ? Raca?.PesoMedioMacho : Raca?.PesoMedioFemea;
                if (pesoRaca.HasValue && pesoRaca > 0) return pesoRaca.Value;

                // 4. Fallback final: o peso atual do animal
                return Peso ?? 0;
            }
        }
    }
}