using System.ComponentModel.DataAnnotations;

namespace backend.Domain.SaudeAnimal.Enums
{
    public enum PerfilMetabolico
    {
        // MANUTENÇÃO
        [Display(Name = "Gato Exótico")]
        GatoExotico = 1,

        [Display(Name = "Gato Doméstico (Magro ou Ideal - ECC ≤ 5)")]
        GatoDomesticoMagro = 2,

        [Display(Name = "Gato Doméstico (Obeso - ECC > 5)")]
        GatoDomesticoObeso = 3,

        [Display(Name = "Cão Ativo ou de Canil (Necessidade média)")]
        CaoAtivo = 4,

        [Display(Name = "Cão Adulto Jovem e Ativo")]
        CaoAdultoJovemAtivo = 5,

        [Display(Name = "Cão Dogue Alemão (Great Danes) Adulto e Ativo")]
        CaoDogueAlemaoAdultoAtivo = 6,

        [Display(Name = "Cão Terrier Adulto e Ativo")]
        CaoTerrierAdultoAtivo = 7,

        [Display(Name = "Cão Inativo")]
        CaoInativo = 8,

        [Display(Name = "Cão Idoso Ativo ou Terra-Nova (Newfoundlands)")]
        CaoIdosoAtivo = 9,

        // FUTUROS PERFIS:
        [Display(Name = "Gato em Crescimento")]
        GatoCrescimento,

        [Display(Name = "Cão em Crescimento")]
        CaoCrescimento,

        [Display(Name = "Gato em Lactação")]
        GatoLactacao,

        [Display(Name = "Cão em Lactação")]
        CaoLactacao
    }
}