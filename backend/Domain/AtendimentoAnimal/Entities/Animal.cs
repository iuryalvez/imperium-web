using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Animal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Nome { get; set; }
        public EspecieAnimal Especie { get; set; }
        public int? RacaId { get; set; }
        
        [ForeignKey("RacaId")]
        public Raca? Raca { get; set; }
        public DateTime Nascimento { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataModificado { get; set; }
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
        public AcompanhamentoIndividualizado? AcompanhamentoIndividualizado { get; set; } // referência de navegação com o dependente
        public ICollection<Consulta>? Consultas { get; set; } // navegation
        public ICollection<AnimalClinica>? AnimaisClinicas { get; set; } // referência de navegação com o dependente
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

        [NotMapped] // Informa ao EF Core para NÃO criar esta coluna no banco
        public FaseVidaAnimal FaseVida
        {
            get
            {
                int idadeAtual = this.Idade; // Usa a propriedade Idade calculada

                switch (this.Especie)
                {
                    case EspecieAnimal.Cao:
                        if (idadeAtual < 1) return FaseVidaAnimal.Filhote;
                        if (idadeAtual < 2) return FaseVidaAnimal.Jovem; // Ex: 1 ano até (mas não incluindo) 2 anos
                        if (idadeAtual < 7) return FaseVidaAnimal.Adulto; // Ex: 2 anos até (mas não incluindo) 7 anos
                        if (idadeAtual < 10) return FaseVidaAnimal.Senior; // Ex: 7 anos até (mas não incluindo) 10 anos
                        return FaseVidaAnimal.Geriatrico; // Ex: 10 anos ou mais

                    case EspecieAnimal.Gato:
                        if (idadeAtual < 1) return FaseVidaAnimal.Filhote;
                        if (idadeAtual < 2) return FaseVidaAnimal.Jovem; // Ex: 1 ano até (mas não incluindo) 2 anos
                        if (idadeAtual < 8) return FaseVidaAnimal.Adulto; // Ex: 2 anos até (mas não incluindo) 8 anos
                        if (idadeAtual < 12) return FaseVidaAnimal.Senior; // Ex: 8 anos até (mas não incluindo) 12 anos
                        return FaseVidaAnimal.Geriatrico; // Ex: 12 anos ou mais

                    default:
                        return FaseVidaAnimal.Desconhecida;
                }
            }
        }
    }
}
