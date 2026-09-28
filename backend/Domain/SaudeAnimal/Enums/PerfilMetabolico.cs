using System.ComponentModel.DataAnnotations;
using backend.Domain.SaudeAnimal.Attributes;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Enums
{
    public enum PerfilMetabolico
    {
        // ==========================================
        // MANUTENÇÃO E EMAGRECIMENTO (GATOS)
        // ==========================================
        [Display(Name = "Gato Exótico (K × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Lactacao, TipoDieta.Crescimento)]
        GatoExotico = 1,

        [Display(Name = "Gato Doméstico - Magro ou Ideal, ECC ≤ 5 (100 × PC^0,67)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Lactacao, TipoDieta.Crescimento)]
        GatoDomesticoMagro = 2,

        [Display(Name = "Gato Doméstico - Obeso, ECC > 5 (130 × PC^0,40)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Lactacao, TipoDieta.Crescimento)]
        GatoDomesticoObeso = 3,

        // ==========================================
        // MANUTENÇÃO E EMAGRECIMENTO (CÃES)
        // ==========================================
        [Display(Name = "Cão Ativo ou de Canil - Necessidade média (130 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Crescimento)]
        CaoAtivo = 4,

        [Display(Name = "Cão Adulto Jovem e Ativo (140 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Crescimento)]
        CaoAdultoJovemAtivo = 5,

        [Display(Name = "Cão Dogue Alemão (Great Danes) Adulto e Ativo (200 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Crescimento)]
        CaoDogueAlemaoAdultoAtivo = 6,

        [Display(Name = "Cão Terrier Adulto e Ativo (180 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Crescimento)]
        CaoTerrierAdultoAtivo = 7,

        [Display(Name = "Cão Inativo (95 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Crescimento)]
        CaoInativo = 8,

        [Display(Name = "Cão Idoso Ativo ou Terra-Nova (Newfoundlands) (105 × PC^0,75)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Crescimento)]
        CaoIdosoAtivo = 9,


        // ==========================================
        // LACTAÇÃO
        // ==========================================
        [Display(Name = "Gata em Lactação")]
        [TipoDietaRef(TipoDieta.Lactacao)]
        GataEmLactacao = 10,

        [Display(Name = "Cadela em Lactação")]
        [TipoDietaRef(TipoDieta.Lactacao)]
        CadelaEmLactacao = 11,

        // ==========================================
        // PERFIS GENÉRICOS (CUSTOMIZADOS)
        // ==========================================
        [Display(Name = "Cão em Manutenção Customizado")]
        [TipoDietaRef(TipoDieta.Manutencao)]
        CaoEmManutencao = 12,

        [Display(Name = "Gato em Manutenção Customizado")]
        [TipoDietaRef(TipoDieta.Manutencao)]
        GatoEmManutencao = 13,

        [Display(Name = "Cadela em Gestação")]
        [TipoDietaRef(TipoDieta.Gestacao)]
        CadelaEmGestacao = 14,

        [Display(Name = "Gata em Gestação")]
        [TipoDietaRef(TipoDieta.Gestacao)]
        GataEmGestacao = 15,

        [Display(Name = "Cão em Crescimento Customizado")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        CaoEmCrescimento = 16,

        [Display(Name = "Gato em Crescimento Customizado")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        GatoEmCrescimento = 17,

        [Display(Name = "Cão em Emagrecimento")]
        [TipoDietaRef(TipoDieta.Emagrecimento)]
        CaoEmEmagrecimento = 18,

        [Display(Name = "Gato em Emagrecimento")]
        [TipoDietaRef(TipoDieta.Emagrecimento)]
        GatoEmEmagrecimento = 19
    }
}