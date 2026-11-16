using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Mappers;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    /// <summary>
    /// Conversões da avaliação de uma consulta. O código é gerado na compilação pelo Mapperly.
    /// Salvar a avaliação substitui todos os campos do DTO, inclusive os nulos (não é edição parcial).
    /// </summary>
    [Mapper]
    [UseStaticMapper(typeof(DietaMapper))]
    public static partial class AvaliacaoMapper
    {
        /// <summary>Aplica os dados do formulário na avaliação (nova ou existente).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        public static partial void Aplicar(UpsertAvaliacaoDto dto, [MappingTarget] Avaliacao avaliacao);

        /// <summary>
        /// Leva para o animal o estado registrado na avaliação (peso, condição corporal, atividade, gestação e lactação).
        /// Ração, descrição e tipo de dieta são só da avaliação.
        /// </summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(UpsertAvaliacaoDto.RacaoId))]
        [MapperIgnoreSource(nameof(UpsertAvaliacaoDto.DescricaoDieta))]
        [MapperIgnoreSource(nameof(UpsertAvaliacaoDto.TipoDieta))]
        public static partial void AplicarNoAnimal(UpsertAvaliacaoDto dto, [MappingTarget] Animal animal);

        /// <summary>Entidade para o DTO de leitura (precisa de Racao, Dieta e Consulta.Animal carregados).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(GetAvaliacaoDto.DataConsulta), Use = nameof(DataDaConsulta))]
        [MapPropertyFromSource(nameof(GetAvaliacaoDto.NomeRacao), Use = nameof(NomeDaRacao))]
        [MapPropertyFromSource(nameof(GetAvaliacaoDto.SexoAnimal), Use = nameof(SexoDoAnimal))]
        [MapPropertyFromSource(nameof(GetAvaliacaoDto.Idade), Use = nameof(IdadeDoAnimal))]
        public static partial GetAvaliacaoDto ParaDto(Avaliacao avaliacao);

        private static DateTime DataDaConsulta(Avaliacao avaliacao) => avaliacao.Consulta?.DataConsulta ?? default;

        private static string NomeDaRacao(Avaliacao avaliacao) => avaliacao.Racao?.Nome ?? string.Empty;

        private static Sexo SexoDoAnimal(Avaliacao avaliacao) => avaliacao.Consulta?.Animal?.Sexo ?? default;

        // Idade atual do animal (não a gravada na avaliação)
        private static int IdadeDoAnimal(Avaliacao avaliacao) => avaliacao.Consulta?.Animal?.Idade ?? 0;
    }
}
