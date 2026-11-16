using backend.Domain.General.Extensions;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Services;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.SaudeAnimal.Mappers
{
    /// <summary>
    /// Conversões da dieta e dos registros semanais de peso. O código é gerado na compilação pelo Mapperly.
    /// A dieta é gravada campo a campo pelo DietaService (cálculo), então aqui só há leitura.
    /// </summary>
    [Mapper]
    public static partial class DietaMapper
    {
        /// <summary>Entidade para o DTO de leitura, com rótulos, dados do plano semanal e registros de peso por semana.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(nameof(@Dieta.Racao.Nome), nameof(GetDietaDto.NomeRacao))]
        [MapPropertyFromSource(nameof(GetDietaDto.StatusLabel), Use = nameof(RotuloDoStatus))]
        [MapPropertyFromSource(nameof(GetDietaDto.NivelRestricaoLabel), Use = nameof(RotuloDoNivelRestricao))]
        [MapPropertyFromSource(nameof(GetDietaDto.SemanaAtual), Use = nameof(SemanaAtual))]
        [MapPropertyFromSource(nameof(GetDietaDto.FatorRestricao), Use = nameof(FatorRestricao))]
        [MapPropertyFromSource(nameof(GetDietaDto.PercentualPerdaSemanal), Use = nameof(PercentualPerdaSemanal))]
        [MapPropertyFromSource(nameof(GetDietaDto.PerdaSemanalKg), Use = nameof(PerdaSemanalKg))]
        [MapPropertyFromSource(nameof(GetDietaDto.RegistrosPeso), Use = nameof(RegistrosPorSemana))]
        public static partial GetDietaDto ParaDto(Dieta dieta);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial GetRegistroPesoDto ParaDto(RegistroPesoDieta registro);

        /// <summary>
        /// A atualização reaproveita o cálculo da criação: leva os dados do formulário de edição para o DTO de criação.
        /// O service preenche a avaliação a partir da dieta gravada.
        /// </summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(UpdateDietaDto.Id))]
        [MapperIgnoreSource(nameof(UpdateDietaDto.AvaliacaoId))]
        public static partial CreateDietaDto ParaCriacao(UpdateDietaDto dto);

        internal static string RotuloDoStatus(Dieta dieta) => dieta.Status.GetDisplayName();

        internal static string? RotuloDoNivelRestricao(Dieta dieta) => dieta.NivelRestricao?.GetDisplayName();

        internal static int? SemanaAtual(Dieta dieta) => DietaPlanoHelper.CalcularSemanaAtual(dieta, null);

        internal static double? FatorRestricao(Dieta dieta) => DietaPlanoHelper.FatorRestricao(dieta);

        internal static double? PercentualPerdaSemanal(Dieta dieta) => DietaPlanoHelper.PercentualPerdaSemanal(dieta);

        internal static double? PerdaSemanalKg(Dieta dieta) => DietaPlanoHelper.PerdaSemanalKg(dieta);

        internal static List<GetRegistroPesoDto> RegistrosPorSemana(Dieta dieta) =>
            dieta.RegistrosPeso.OrderBy(r => r.Semana).Select(ParaDto).ToList();
    }
}
