using System.ComponentModel.DataAnnotations;

namespace backend.Domain.SaudeAnimal.Enums
{
    public enum PerfilMetabolico
    {
        // MANUTENÇÃO
        [Display(Name = "Gato Exótico")]
        GatoExotico,

        [Display(Name = "Gato Domestico (Magro ou Ideal - ECC ≤ 5)")]
        GatoDomesticoMagro,

        [Display(Name = "Gato Domestico (Obeso - ECC > 5)")]
        GatoDomesticoObeso,

        [Display(Name = "Cão Ativo ou de Canil (Média)")]
        CaoAtivo,

        [Display(Name = "Cão Adulto Jovem e Ativo")]
        CaoAdultoJovemAtivo,

        [Display(Name = "Cão Jovem com Atividade Moderada")]
        CaoJovemModerado,

        [Display(Name = "Cão Dogue Alemão Adulto e Ativo")]
        CaoDogueAlemaoAdultoAtivo,

        [Display(Name = "Cão Terrier Adulto e Ativo")]
        CaoTerrierAdultoAtivo,

        [Display(Name = "Cão Inativo / Baixa Atividade")]
        CaoInativo,

        [Display(Name = "Cão Idoso Ativo ou Terra-Nova")]
        CaoIdosoAtivo,

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