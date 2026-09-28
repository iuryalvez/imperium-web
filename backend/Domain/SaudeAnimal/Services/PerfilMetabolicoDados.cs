using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Services
{
    public static class PerfilMetabolicoDados
    {
        // Tupla atualizada: (multiplicador, potencia, descricao)
        public static readonly Dictionary<PerfilMetabolico, (double? multiplicador, double? potencia, string descricao)> Dados =
            new()
            {
                // ----------------- MANUTENÇÃO -----------------
                { PerfilMetabolico.GatoDomesticoMagro, (100, 0.67, "Gatos domésticos em adequada condição corporal ou magros") },
                { PerfilMetabolico.GatoDomesticoObeso, (130, 0.40, "Gatos domésticos obesos") },
                { PerfilMetabolico.CaoAtivo, (130, 0.75, "Cães ativos ou de canil com necessidade média") },
                { PerfilMetabolico.CaoAdultoJovemAtivo, (140, 0.75, "Cães adultos jovens e ativos") },
                { PerfilMetabolico.CaoDogueAlemaoAdultoAtivo, (200, 0.75, "Cães Dogue Alemão (Great Danes) adultos e ativos") },
                { PerfilMetabolico.CaoTerrierAdultoAtivo, (180, 0.75, "Cães Terriers adultos e ativos") },
                { PerfilMetabolico.CaoIdosoAtivo, (105, 0.75, "Cães idosos ativos ou Newfoundlands") },
                { PerfilMetabolico.CaoInativo, (95, 0.75, "Cães inativos") },

                // ----------------- CRESCIMENTO -----------------
                { PerfilMetabolico.GatoEmCrescimentoComNecessidadeMedia, (100, 0.67, "Gato em crescimento com necessidade média") },
                { PerfilMetabolico.CaoEmCrescimentoComNecessidadeMedia, (130, 0.75, "Cão em crescimento com necessidade média") },

                // ----------------- GENÉRICOS (CUSTOMIZADOS) -----------------
                { PerfilMetabolico.CaoEmManutencao, (null, null, "Cão em manutenção customizado") },
                { PerfilMetabolico.GatoEmManutencao, (null, null, "Gato em manutenção customizado") },
                { PerfilMetabolico.CaoEmGestacao, (null, null, "Cão em gestação") },
                { PerfilMetabolico.GatoEmGestacao, (null, null, "Gato em gestação") },
                { PerfilMetabolico.CaoEmCrescimento, (null, null, "Cão em crescimento") },
                { PerfilMetabolico.GatoEmCrescimento, (null, null, "Gato em crescimento") },
                { PerfilMetabolico.CaoEmEmagrecimento, (null, null, "Cão em emagrecimento customizado") },
                { PerfilMetabolico.GatoEmEmagrecimento, (null, null, "Gato em emagrecimento customizado") },
                { PerfilMetabolico.CaoEmLactacao, (null, null, "Cão em lactação") },
                { PerfilMetabolico.GatoEmLactacao, (null, null, "Gato em lactação") },
            };

        public static double CalcularNecessidade(PerfilMetabolico perfil, double peso)
        {
            if (!Dados.ContainsKey(perfil))
                throw new InvalidOperationException($"Perfil metabólico '{perfil}' não tem dados cadastrados.");

            // Usamos '_' para ignorar a descrição na hora de fazer a conta matemática
            var (multiplicador, potencia, _) = Dados[perfil];
            
            if (!multiplicador.HasValue || !potencia.HasValue)
                throw new InvalidOperationException($"Perfil metabólico '{perfil}' genérico não deve ser usado para cálculo base sem envio de K e P explícitos.");

            return multiplicador.Value * Math.Pow(peso, potencia.Value);
        }
    }
}