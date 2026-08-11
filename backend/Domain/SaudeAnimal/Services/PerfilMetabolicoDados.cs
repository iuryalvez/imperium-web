using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Services
{
    public static class PerfilMetabolicoDados
    {
        // Tupla atualizada: (multiplicador, potencia, descricao)
        public static readonly Dictionary<PerfilMetabolico, (double multiplicador, double potencia, string descricao)> Dados =
            new()
            {
                // ----------------- MANUTENÇÃO -----------------
                { PerfilMetabolico.GatoExotico, ((55 + 260) / 2.0, 0.75, "Gatos exóticos (Média NRC)") },
                { PerfilMetabolico.GatoDomesticoMagro, (100, 0.67, "Gatos domésticos em adequada condição corporal ou magros (ECC ≤ 5)") },
                { PerfilMetabolico.GatoDomesticoObeso, (130, 0.40, "Gatos domésticos obesos (ECC > 5)") },
                { PerfilMetabolico.CaoAtivo, (130, 0.75, "Cães ativos ou de canil (necessidade média)") },
                { PerfilMetabolico.CaoAdultoJovemAtivo, (140, 0.75, "Cães adultos jovens e ativos") },
                { PerfilMetabolico.CaoDogueAlemaoAdultoAtivo, (200, 0.75, "Cães Dogue Alemão (Great Danes) adultos e ativos") },
                { PerfilMetabolico.CaoTerrierAdultoAtivo, (180, 0.75, "Cães Terriers adultos e ativos") },
                { PerfilMetabolico.CaoIdosoAtivo, (105, 0.75, "Cães idosos ativos ou Newfoundlands") },
                { PerfilMetabolico.CaoInativo, (95, 0.75, "Cães inativos") },
            };

        public static double CalcularNecessidade(PerfilMetabolico perfil, double peso)
        {
            if (!Dados.ContainsKey(perfil))
                throw new InvalidOperationException($"Perfil metabólico '{perfil}' não tem dados cadastrados.");

            // Usamos '_' para ignorar a descrição na hora de fazer a conta matemática
            var (multiplicador, potencia, _) = Dados[perfil];
            return multiplicador * Math.Pow(peso, potencia);
        }
    }
}