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
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Lactacao)]
        GatoExotico = 1,

        [Display(Name = "Gato Doméstico - Magro ou Ideal, ECC ≤ 5 (100 × PC^0,67)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Lactacao)]
        GatoDomesticoMagro = 2,

        [Display(Name = "Gato Doméstico - Obeso, ECC > 5 (130 × PC^0,40)")]
        [TipoDietaRef(TipoDieta.Manutencao, TipoDieta.Emagrecimento, TipoDieta.Lactacao)]
        GatoDomesticoObeso = 3,

        // ==========================================
        // MANUTENÇÃO E EMAGRECIMENTO (CÃES)
        // ==========================================
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

        // ==========================================
        // CRESCIMENTO
        // ==========================================
        [Display(Name = "Gato em Crescimento - Necessidade Média (100 × PC^0,67 × 6,7 × [e^-(0,189p)-0,66])")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        GatoEmCrescimentoComNecessidadeMedia = 10,

        [Display(Name = "Cão em Crescimento - Necessidade Média (130 × PC^0,75 × 3,2 × [e^-(0,87p)-0,1])")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        CaoEmCrescimentoComNecessidadeMedia = 11,

        // ==========================================
        // LACTAÇÃO
        // ==========================================
        [Display(Name = "Gato em Lactação")]
        [TipoDietaRef(TipoDieta.Lactacao)]
        GatoEmLactacao = 12,

        [Display(Name = "Cão em Lactação")]
        [TipoDietaRef(TipoDieta.Lactacao)]
        CaoEmLactacao = 13,

        // ==========================================
        // PERFIS GENÉRICOS (CUSTOMIZADOS)
        // ==========================================
        [Display(Name = "Cão em Manutenção Customizado")]
        [TipoDietaRef(TipoDieta.Manutencao)]
        CaoEmManutencao = 14,

        [Display(Name = "Gato em Manutenção Customizado")]
        [TipoDietaRef(TipoDieta.Manutencao)]
        GatoEmManutencao = 15,

        [Display(Name = "Cão em Gestação")]
        [TipoDietaRef(TipoDieta.Gestacao)]
        CaoEmGestacao = 16,

        [Display(Name = "Gato em Gestação")]
        [TipoDietaRef(TipoDieta.Gestacao)]
        GatoEmGestacao = 17,

        [Display(Name = "Cão em Crescimento Customizado")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        CaoEmCrescimento = 18,

        [Display(Name = "Gato em Crescimento Customizado")]
        [TipoDietaRef(TipoDieta.Crescimento)]
        GatoEmCrescimento = 19,

        [Display(Name = "Cão em Emagrecimento")]
        [TipoDietaRef(TipoDieta.Emagrecimento)]
        CaoEmEmagrecimento = 20,

        [Display(Name = "Gato em Emagrecimento")]
        [TipoDietaRef(TipoDieta.Emagrecimento)]
        GatoEmEmagrecimento = 21
    }
}