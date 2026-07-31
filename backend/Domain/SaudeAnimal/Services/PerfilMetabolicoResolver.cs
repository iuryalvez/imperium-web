using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
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

                if (animal.CondicaoCorporal.HasValue && (int) animal.CondicaoCorporal.Value <= 5)
                    return PerfilMetabolico.GatoDomesticoMagro;

                return PerfilMetabolico.GatoDomesticoObeso;
            }

            if (animal.Especie == EspecieAnimal.Cao)
            {
                var categoria = animal.Raca?.CategoriaMetabolica ?? CategoriaMetabolica.Padrao;

                // Exceções Metabólicas
                if (categoria == CategoriaMetabolica.DogueAlemao &&
                    (animal.GrauAtividade.Value) >= GrauAtividade.Ativo && animal.FaseVida == FaseVidaAnimal.Adulto)
                    return PerfilMetabolico.CaoDogueAlemaoAdultoAtivo;

                if (categoria == CategoriaMetabolica.Terrier &&
                    animal.GrauAtividade.Value >= GrauAtividade.Ativo && animal.FaseVida == FaseVidaAnimal.Adulto)
                    return PerfilMetabolico.CaoTerrierAdultoAtivo;

                if (categoria == CategoriaMetabolica.TerraNova)
                    return PerfilMetabolico.CaoIdosoAtivo; // Usa o mesmo multiplicador de 105 kcal

                // Fluxo Padrão (Sem exceção de raça)
                if ((animal.FaseVida == FaseVidaAnimal.Senior && animal.GrauAtividade.Value == GrauAtividade.Ativo) ||
                    (animal.FaseVida == FaseVidaAnimal.Geriatrico && animal.GrauAtividade.Value == GrauAtividade.Ativo))
                    return PerfilMetabolico.CaoIdosoAtivo;

                if (animal.GrauAtividade.Value == GrauAtividade.Inativo)
                    return PerfilMetabolico.CaoInativo;

                if (animal.FaseVida is FaseVidaAnimal.Jovem or FaseVidaAnimal.Adulto &&
                    animal.GrauAtividade.Value == GrauAtividade.Ativo)
                    return PerfilMetabolico.CaoAdultoJovemAtivo;

                return PerfilMetabolico.CaoJovemModerado; // Fallback padrão
            }

            throw new InvalidOperationException($"Espécie '{animal.Especie}' não suportada.");
        }
    }
}