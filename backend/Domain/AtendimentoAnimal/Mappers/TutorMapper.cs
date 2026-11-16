using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    /// <summary>
    /// Conversões entre a entidade Tutor e os DTOs. O código é gerado na compilação pelo Mapperly.
    /// <c>AllowNullPropertyAssignment = false</c> faz a edição parcial: campo nulo no DTO mantém o valor atual.
    /// </summary>
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class TutorMapper
    {
        /// <summary>Entidade para o DTO de leitura, com os animais vinculados (precisa de TutoresAnimais.Animal carregado).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(GetTutoresClinicaDto.Animais), Use = nameof(AnimaisDoTutor))]
        public static partial GetTutoresClinicaDto ParaDto(Tutor tutor);

        /// <summary>Cadastro. ClinicaId, Id e status são definidos pelo service; os vínculos com animais também.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(CadastrarTutorClinicaDto.AnimalIds))]
        public static partial Tutor ParaEntidade(CadastrarTutorClinicaDto dto);

        /// <summary>Edição: aplica só o que veio preenchido. Id, data de cadastro e vínculos não vêm deste mapa.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(EditarTutorClinicaDto.Id))]
        [MapperIgnoreSource(nameof(EditarTutorClinicaDto.AnimalIds))]
        [MapperIgnoreSource(nameof(EditarTutorClinicaDto.DataCadastro))]
        public static partial void Aplicar(EditarTutorClinicaDto dto, [MappingTarget] Tutor tutor);

        private static List<AnimalInfoSimplesDto> AnimaisDoTutor(Tutor tutor) =>
            tutor.TutoresAnimais?
                .Select(ta => new AnimalInfoSimplesDto
                {
                    Id = ta.Animal!.Id,
                    Nome = ta.Animal.Nome,
                    Especie = ta.Animal.Especie
                })
                .ToList() ?? new List<AnimalInfoSimplesDto>();
    }
}
