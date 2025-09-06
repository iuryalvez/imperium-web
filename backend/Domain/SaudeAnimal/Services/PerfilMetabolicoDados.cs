using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Services
{
    public static class PerfilMetabolicoDados
    {
        public static readonly Dictionary<PerfilMetabolico, (double multiplicador, double potencia)> Dados =
            new()
            {
                // ----------------- MANUTENÇÃO -----------------
                { PerfilMetabolico.GatoExotico, ((55 + 260) / 2.0, 0.75) },
                { PerfilMetabolico.GatoDomesticoMagro, (100, 0.67) },
                { PerfilMetabolico.GatoDomesticoObeso, (130, 0.40) },
                { PerfilMetabolico.CaoAtivo, (130, 0.75) },
                { PerfilMetabolico.CaoAdultoJovemAtivo, (140, 0.75) },
                { PerfilMetabolico.CaoJovemModerado, (130, 0.75) },
                { PerfilMetabolico.CaoDogueAlemaoAdultoAtivo, (200, 0.75) },
                { PerfilMetabolico.CaoTerrierAdultoAtivo, (180, 0.75) },
                { PerfilMetabolico.CaoIdosoAtivo, (105, 0.75) },
                { PerfilMetabolico.CaoInativo, (95, 0.75) },
            };

        public static double CalcularNecessidade(PerfilMetabolico perfil, double peso)
        {
            if (!Dados.ContainsKey(perfil))
                throw new InvalidOperationException($"Perfil metabólico '{perfil}' não tem dados cadastrados.");

            var (multiplicador, potencia) = Dados[perfil];
            return multiplicador * Math.Pow(peso, potencia);
        }
    }
}
