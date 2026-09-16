using System.ComponentModel.DataAnnotations;
using backend.Domain.SaudeAnimal.Attributes;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Enums
{
    public enum PerfilMetabolico
    {
        // MANUTENÇÃO E EMAGRECIMENTO
        [Display(Name = "Gato Exótico (157,5 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        GatoExotico = 1,

        [Display(Name = "Gato Doméstico - Magro ou Ideal, ECC ≤ 5 (100 × PC^0,67)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        GatoDomesticoMagro = 2,

        [Display(Name = "Gato Doméstico - Obeso, ECC > 5 (130 × PC^0,40)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        GatoDomesticoObeso = 3,

        [Display(Name = "Cão Ativo ou de Canil - Necessidade média (130 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        CaoAtivo = 4,

        [Display(Name = "Cão Adulto Jovem e Ativo (140 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        CaoAdultoJovemAtivo = 5,

        [Display(Name = "Cão Dogue Alemão (Great Danes) Adulto e Ativo (200 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        CaoDogueAlemaoAdultoAtivo = 6,

        [Display(Name = "Cão Terrier Adulto e Ativo (180 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        CaoTerrierAdultoAtivo = 7,

        [Display(Name = "Cão Inativo (95 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        CaoInativo = 8,

        [Display(Name = "Cão Idoso Ativo ou Terra-Nova (Newfoundlands) (105 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento)]
        CaoIdosoAtivo = 9,

        // CRESCIMENTO
        [Display(Name = "Gato em Crescimento - Necessidade Média (100 × PC^0,67 × 6,7 × [e^-(0,189p)-0,66])")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        GatoCrescimentoMedia,

        [Display(Name = "Cão em Crescimento - Necessidade Média (130 × PC^0,75 × 3,2 × [e^-(0,87p)-0,1])")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        CaoCrescimentoMedia,

        // LACTAÇÃO
        [Display(Name = "Gato em Lactação")]
        [TipoDietaRef(TipoDieta.Lactacao)]
        GatoLactacao,

        [Display(Name = "Cão em Lactação")]
        [TipoDietaRef(TipoDieta.Lactacao)]
        CaoLactacao,

        // GESTAÇÃO
        [Display(Name = "Gato em Gestação (Sem Equação NRC - 40 a 50% Ganho)")]
        [TipoDietaRef(TipoDieta.Gestacao)]
        GatoGestacao,

        [Display(Name = "Cão em Gestação (4 semanas finais)")]
        [TipoDietaRef(TipoDieta.Gestacao)]
        CaoGestacao
    }
}