using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.GestaoClinica.Mappers
{
    /// <summary>
    /// Conversões entre a entidade Clinica e os DTOs. O código é gerado na compilação pelo Mapperly.
    /// <c>AllowNullPropertyAssignment = false</c> faz a edição parcial: campo nulo no DTO mantém o valor atual.
    /// </summary>
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class ClinicaMapper
    {
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        public static partial GetClinicaDto ParaDto(Clinica clinica);

        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        public static partial Clinica ParaEntidade(CadastrarClinicaDto dto);

        /// <summary>Edição pelo admin do sistema: aplica só o que veio preenchido (inclusive o status).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(EditarClinicaAdminDto.Id))]
        public static partial void Aplicar(EditarClinicaAdminDto dto, [MappingTarget] Clinica clinica);

        /// <summary>Edição pela própria clínica: aplica só o que veio preenchido.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        public static partial void Aplicar(EditarClinicaDto dto, [MappingTarget] Clinica clinica);
    }
}
