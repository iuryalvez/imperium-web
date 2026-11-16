using backend.Domain.GestaoClinica.Entities;
using backend.Domain.SaudeAnimal.DTOs.Racao;
using backend.Domain.SaudeAnimal.Entities;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.SaudeAnimal.Mappers
{
    /// <summary>
    /// Conversões entre a entidade Racao e os DTOs. O código é gerado na compilação pelo Mapperly.
    /// <c>AllowNullPropertyAssignment = false</c> faz a edição parcial: campo nulo no DTO mantém o valor atual.
    /// </summary>
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class RacaoMapper
    {
        /// <summary>Entidade para o DTO de leitura. IsOculta depende da clínica e é preenchida pelo service.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(nameof(@Racao.Clinica.NomeClinica), nameof(GetRacoesDto.NomeClinica))]
        [MapPropertyFromSource(nameof(GetRacoesDto.IsSistema), Use = nameof(EhDoSistema))]
        [MapperIgnoreTarget(nameof(GetRacoesDto.IsOculta))]
        public static partial GetRacoesDto ParaDto(Racao racao);

        /// <summary>Cadastro. ClinicaId, status, datas e os valores calculados são definidos pelo service.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        public static partial Racao ParaEntidade(CadastrarRacaoDto dto);

        /// <summary>Edição (e cópia de ração do sistema): aplica sobre a entidade só o que veio preenchido no DTO.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(AtualizarRacaoDto.Id))]
        public static partial void Aplicar(AtualizarRacaoDto dto, [MappingTarget] Racao racao);

        private static bool EhDoSistema(Racao racao) => racao.ClinicaId == null;
    }
}
