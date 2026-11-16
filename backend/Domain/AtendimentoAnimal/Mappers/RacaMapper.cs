using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.Entities;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    /// <summary>
    /// Conversões entre a entidade Raca e os DTOs. O código é gerado na compilação pelo Mapperly.
    /// <c>AllowNullPropertyAssignment = false</c> faz a edição parcial: campo nulo no DTO mantém o valor atual.
    /// </summary>
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class RacaMapper
    {
        /// <summary>Entidade para o DTO de leitura. IsOculta depende da clínica e é preenchida pelo service.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(new[] { nameof(Raca.Clinica), nameof(Clinica.NomeClinica) }, new[] { nameof(GetRacaDto.NomeClinica) })]
        [MapPropertyFromSource(nameof(GetRacaDto.IsSistema), Use = nameof(EhDoSistema))]
        [MapperIgnoreTarget(nameof(GetRacaDto.IsOculta))]
        public static partial GetRacaDto ParaDto(Raca raca);

        /// <summary>Cadastro. ClinicaId, status e datas são definidos pelo service.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        public static partial Raca ParaEntidade(CadastrarRacaDto dto);

        /// <summary>Edição (e cópia de raça do sistema): aplica sobre a entidade só o que veio preenchido no DTO.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(UpdateRacaDto.Id))]
        public static partial void Aplicar(UpdateRacaDto dto, [MappingTarget] Raca raca);

        private static bool EhDoSistema(Raca raca) => raca.ClinicaId == null;
    }
}
