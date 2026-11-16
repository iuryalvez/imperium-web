using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    /// <summary>
    /// Conversões entre a entidade Animal e os DTOs de cadastro, edição e leitura. O código é gerado na compilação pelo Mapperly.
    /// <c>AllowNullPropertyAssignment = false</c> faz a edição parcial: campo nulo no DTO mantém o valor atual.
    /// O histórico completo do animal fica no <see cref="AcompanhamentoMapper"/>.
    /// </summary>
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class AnimalMapper
    {
        /// <summary>Entidade para o DTO de leitura (lista e detalhe).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(nameof(@Animal.Raca.Nome), nameof(GetAnimaisClinicaDto.RacaNome))]
        [MapProperty(nameof(Animal.IsGestante), nameof(GetAnimaisClinicaDto.Gestacao))]
        [MapProperty(nameof(Animal.IsLactanteNumeroFilhotes), nameof(GetAnimaisClinicaDto.Lactacao))]
        [MapPropertyFromSource(nameof(GetAnimaisClinicaDto.AcompanhamentoId), Use = nameof(IdDoAcompanhamento))]
        [MapPropertyFromSource(nameof(GetAnimaisClinicaDto.TutorIds), Use = nameof(IdsDosTutores))]
        [MapPropertyFromSource(nameof(GetAnimaisClinicaDto.Tutores), Use = nameof(TutoresDoAnimal))]
        public static partial GetAnimaisClinicaDto ParaDto(Animal animal);

        /// <summary>Cadastro. ClinicaId e status são definidos pelo service a partir do token; os tutores também.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(CadastrarAnimalClinicaDto.TutorIds))]
        [MapProperty(nameof(CadastrarAnimalClinicaDto.Nascimento), nameof(Animal.Nascimento), Use = nameof(ParaUtc))]
        [MapValue(nameof(Animal.DataCadastro), Use = nameof(Agora))]
        [MapValue(nameof(Animal.DataModificado), Use = nameof(Agora))]
        public static partial Animal ParaEntidade(CadastrarAnimalClinicaDto dto);

        /// <summary>Edição: aplica só o que veio preenchido. Id e tutores não vêm deste mapa.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(EditarAnimalClinicaDto.Id))]
        [MapperIgnoreSource(nameof(EditarAnimalClinicaDto.TutorIds))]
        public static partial void Aplicar(EditarAnimalClinicaDto dto, [MappingTarget] Animal animal);

        private static DateTime Agora() => DateTime.UtcNow;

        private static DateTime ParaUtc(DateTime data) => data.ToUniversalTime();

        private static int IdDoAcompanhamento(Animal animal) => animal.Acompanhamento?.Id ?? 0;

        private static List<int> IdsDosTutores(Animal animal) =>
            animal.TutoresAnimais?.Select(ta => ta.TutorId).ToList() ?? new List<int>();

        private static List<TutorInfoSimplesDto> TutoresDoAnimal(Animal animal) =>
            animal.TutoresAnimais?
                .Where(ta => ta.Tutor != null)
                .Select(ta => new TutorInfoSimplesDto
                {
                    Id = ta.TutorId,
                    Nome = ta.Tutor!.Nome
                })
                .ToList() ?? new List<TutorInfoSimplesDto>();
    }
}
