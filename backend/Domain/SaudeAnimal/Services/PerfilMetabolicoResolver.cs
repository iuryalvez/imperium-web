using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Services
{
    public static class PerfilMetabolicoResolver
    {
        public static PerfilMetabolico Resolver(Animal animal, DietaPersonalizada dieta)
        {
            switch (dieta.TipoDieta)
            {
                case TipoDieta.Manutencao:
                    return ResolverManutencao(animal);

                case TipoDieta.Crescimento:
                    // TODO: implementar no futuro
                    return animal.Especie == EspecieAnimal.Gato
                        ? PerfilMetabolico.GatoCrescimento
                        : PerfilMetabolico.CaoCrescimento;

                case TipoDieta.Lactacao:
                    // TODO: implementar no futuro
                    return animal.Especie == EspecieAnimal.Gato
                        ? PerfilMetabolico.GatoLactacao
                        : PerfilMetabolico.CaoLactacao;

                default:
                    throw new InvalidOperationException($"Tipo de dieta '{dieta.TipoDieta}' não suportado.");
            }
        }

        private static PerfilMetabolico ResolverManutencao(Animal animal)
        {
            if (animal.Especie == EspecieAnimal.Gato)
            {
                if (animal.Raca != null && !animal.Raca.Nome.Contains("SRD", StringComparison.OrdinalIgnoreCase))
                    return PerfilMetabolico.GatoExotico;

                if (animal.CondicaoCorporal.HasValue && (int)animal.CondicaoCorporal <= 5)
                    return PerfilMetabolico.GatoDomesticoMagro;

                return PerfilMetabolico.GatoDomesticoObeso;
            }

            if (animal.Especie == EspecieAnimal.Cao)
            {
                string? nomeRaca = animal.Raca?.Nome;

                if (!string.IsNullOrEmpty(nomeRaca))
                {
                    if ((nomeRaca.Contains("dogue alemão", StringComparison.OrdinalIgnoreCase) ||
                        nomeRaca.Contains("grande dinamarquês", StringComparison.OrdinalIgnoreCase)) &&
                        animal.GrauAtividade >= GrauAtividade.Ativo && animal.FaseVida == FaseVidaAnimal.Adulto)
                        return PerfilMetabolico.CaoDogueAlemaoAdultoAtivo;

                    if (nomeRaca.Contains("terrier", StringComparison.OrdinalIgnoreCase) &&
                        animal.GrauAtividade >= GrauAtividade.Ativo && animal.FaseVida == FaseVidaAnimal.Adulto)
                        return PerfilMetabolico.CaoTerrierAdultoAtivo;
                }

                if ((animal.FaseVida == FaseVidaAnimal.Senior && animal.GrauAtividade == GrauAtividade.Ativo) ||
                    (animal.FaseVida == FaseVidaAnimal.Geriatrico && animal.GrauAtividade == GrauAtividade.Ativo))
                    return PerfilMetabolico.CaoIdosoAtivo;

                if (animal.GrauAtividade == GrauAtividade.Inativo)
                    return PerfilMetabolico.CaoInativo;

                if (animal.FaseVida is FaseVidaAnimal.Jovem or FaseVidaAnimal.Adulto &&
                    animal.GrauAtividade == GrauAtividade.Ativo)
                    return PerfilMetabolico.CaoAdultoJovemAtivo;

                return PerfilMetabolico.CaoJovemModerado;
            }

            throw new InvalidOperationException($"Espécie '{animal.Especie}' não suportada.");
        }
    }
}