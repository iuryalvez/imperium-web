using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Services
{
    public static class PerfilMetabolicoResolver
    {
        public static PerfilMetabolico? Resolver(Animal animal, Dieta dieta)
        {
            if (dieta.PerfilMetabolico.HasValue)
                return dieta.PerfilMetabolico.Value;

            switch (dieta.TipoDieta)
            {
                case TipoDieta.Manutencao:
                    return ResolverManutencao(animal);

                case TipoDieta.Crescimento:
                    return animal.Especie == EspecieAnimal.Gato
                        ? PerfilMetabolico.GatoEmCrescimentoComNecessidadeMedia
                        : PerfilMetabolico.CaoEmCrescimentoComNecessidadeMedia;

                case TipoDieta.Lactacao:
                    return animal.Especie == EspecieAnimal.Gato
                        ? PerfilMetabolico.GatoEmLactacao
                        : PerfilMetabolico.CaoEmLactacao;
                case TipoDieta.Gestacao:
                    return animal.Especie == EspecieAnimal.Gato
                        ? PerfilMetabolico.GatoEmGestacao
                        : PerfilMetabolico.CaoEmGestacao;

                default:
                    throw new InvalidOperationException($"Tipo de dieta '{dieta.TipoDieta}' não suportado.");
            }
        }

        private static PerfilMetabolico? ResolverManutencao(Animal animal)
        {
            if (animal.Especie == EspecieAnimal.Gato)
            {
                // if (animal.Raca != null && !animal.Raca.Nome.Contains("SRD", StringComparison.OrdinalIgnoreCase))
                //     return PerfilMetabolico.GatoExotico;

                if (animal.CondicaoCorporal.HasValue && (int)animal.CondicaoCorporal.Value <= 5)
                    return PerfilMetabolico.GatoDomesticoMagro;

                return PerfilMetabolico.GatoDomesticoObeso;
            }

            if (animal.Especie == EspecieAnimal.Cao)
            {
                // Prevents InvalidOperationException if GrauAtividade is null
                if (!animal.GrauAtividade.HasValue) return null;

                var categoria = animal.Raca?.CategoriaMetabolica ?? CategoriaMetabolica.Padrao;

                if (categoria == CategoriaMetabolica.DogueAlemao &&
                    animal.GrauAtividade.Value >= GrauAtividade.Ativo && animal.FaseVida == FaseVidaAnimal.Adulto)
                    return PerfilMetabolico.CaoDogueAlemaoAdultoAtivo;

                if (categoria == CategoriaMetabolica.Terrier &&
                    animal.GrauAtividade.Value >= GrauAtividade.Ativo && animal.FaseVida == FaseVidaAnimal.Adulto)
                    return PerfilMetabolico.CaoTerrierAdultoAtivo;

                if (categoria == CategoriaMetabolica.TerraNova)
                    return PerfilMetabolico.CaoIdosoAtivo;

                if (animal.GrauAtividade.Value >= GrauAtividade.Ativo &&
                    (animal.FaseVida == FaseVidaAnimal.Senior || animal.FaseVida == FaseVidaAnimal.Geriatrico))
                    return PerfilMetabolico.CaoIdosoAtivo;

                if (animal.GrauAtividade.Value == GrauAtividade.Inativo)
                    return PerfilMetabolico.CaoInativo;

                // At least moderate level resolves to Jovem or Adulto profiles
                if (animal.FaseVida == FaseVidaAnimal.Jovem && animal.GrauAtividade.Value >= GrauAtividade.Moderado)
                    return PerfilMetabolico.CaoAdultoJovemAtivo;

                if (animal.FaseVida == FaseVidaAnimal.Adulto && animal.GrauAtividade.Value >= GrauAtividade.Moderado)
                    return PerfilMetabolico.CaoAtivo;

                return null;
            }

            throw new InvalidOperationException($"Espécie '{animal.Especie}' não suportada.");
        }
    }
}