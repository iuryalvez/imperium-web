using backend.Domain.SaudeAnimal.Enums;

using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Services
{
    public static class PerfilMetabolicoDados
    {
        // Tupla atualizada: (multiplicador, potencia, descricao, especie, sexo)
        public static readonly Dictionary<PerfilMetabolico, (double? multiplicador, double? potencia, string descricao, EspecieAnimal especie, Sexo? sexo)> Dados =
            new()
            {
                // ----------------- MANUTENÇÃO -----------------
                { PerfilMetabolico.GatoExotico, (null, 0.75, "Gatos exóticos", EspecieAnimal.Gato, null) },
                { PerfilMetabolico.GatoDomesticoMagro, (100, 0.67, "Gatos domésticos em adequada condição corporal ou magros", EspecieAnimal.Gato, null) },
                { PerfilMetabolico.GatoDomesticoObeso, (130, 0.40, "Gatos domésticos obesos", EspecieAnimal.Gato, null) },
                { PerfilMetabolico.CaoAtivo, (130, 0.75, "Cães ativos ou de canil com necessidade média", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.CaoAdultoJovemAtivo, (140, 0.75, "Cães adultos jovens e ativos", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.CaoDogueAlemaoAdultoAtivo, (200, 0.75, "Cães Dogue Alemão (Great Danes) adultos e ativos", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.CaoTerrierAdultoAtivo, (180, 0.75, "Cães Terriers adultos e ativos", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.CaoIdosoAtivo, (105, 0.75, "Cães idosos ativos ou Newfoundlands", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.CaoInativo, (95, 0.75, "Cães inativos", EspecieAnimal.Cao, null) },


                // ----------------- GENÉRICOS (CUSTOMIZADOS) -----------------
                { PerfilMetabolico.CaoEmManutencao, (null, null, "Cães em manutenção", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.GatoEmManutencao, (null, null, "Gatos em manutenção", EspecieAnimal.Gato, null) },
                { PerfilMetabolico.CadelaEmGestacao, (null, null, "Cadelas em gestação", EspecieAnimal.Cao, Sexo.Femea) },
                { PerfilMetabolico.GataEmGestacao, (null, null, "Gatas em gestação", EspecieAnimal.Gato, Sexo.Femea) },
                { PerfilMetabolico.CaoEmCrescimento, (null, null, "Cães em crescimento", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.GatoEmCrescimento, (null, null, "Gatos em crescimento", EspecieAnimal.Gato, null) },
                { PerfilMetabolico.CaoEmEmagrecimento, (null, null, "Cães em emagrecimento", EspecieAnimal.Cao, null) },
                { PerfilMetabolico.GatoEmEmagrecimento, (null, null, "Gatos em emagrecimento", EspecieAnimal.Gato, null) },
                { PerfilMetabolico.CadelaEmLactacao, (null, null, "Cadelas em lactação", EspecieAnimal.Cao, Sexo.Femea) },
                { PerfilMetabolico.GataEmLactacao, (null, null, "Gatas em lactação", EspecieAnimal.Gato, Sexo.Femea) },
            };

        public static double CalcularNecessidade(PerfilMetabolico perfil, double peso)
        {
            if (!Dados.ContainsKey(perfil))
                throw new InvalidOperationException($"Perfil metabólico '{perfil}' não tem dados cadastrados.");

            // Usamos '_' para ignorar a descrição, especie e sexo na hora de fazer a conta matemática
            var (multiplicador, potencia, _, _, _) = Dados[perfil];
            
            if (!multiplicador.HasValue || !potencia.HasValue)
                throw new InvalidOperationException($"Perfil metabólico '{perfil}' genérico não deve ser usado para cálculo base sem envio de K e P explícitos.");

            return multiplicador.Value * Math.Pow(peso, potencia.Value);
        }
    }
}