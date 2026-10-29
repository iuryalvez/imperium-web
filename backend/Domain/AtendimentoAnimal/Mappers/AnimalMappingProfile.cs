using backend.Domain.Utils;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    public class AnimalProfile : Profile
    {
        public AnimalProfile()
        {
            CreateMap<Animal, GetAnimaisClinicaDto>()
                .ForMember(dest => dest.RacaNome, opt => opt.MapFrom(src => src.Raca != null ? src.Raca.Nome : null))
                .ForMember(dest => dest.Gestacao, opt => opt.MapFrom(src => src.IsGestante))
                .ForMember(dest => dest.Lactacao, opt => opt.MapFrom(src => src.IsLactanteNumeroFilhotes))
                .ForMember(dest => dest.AcompanhamentoId, opt => opt.MapFrom(src =>
                    src.Acompanhamento != null ? (int?)src.Acompanhamento.Id : null))
                .ForMember(dest => dest.TutorIds, opt => opt.MapFrom(src =>
                    src.TutoresAnimais != null
                        ? src.TutoresAnimais.Select(ta => ta.TutorId).ToList()
                        : new List<int>()))
                .ForMember(dest => dest.Tutores, opt => opt.MapFrom(src =>
                    src.TutoresAnimais != null
                        ? src.TutoresAnimais
                            .Where(ta => ta.Tutor != null)
                            .Select(ta => new TutorInfoSimplesDto
                            {
                                Id = ta.TutorId,
                                Nome = ta.Tutor.Nome
                            })
                            .ToList()
                        : new List<TutorInfoSimplesDto>()));

            // ClinicaId e status são definidos pelo service a partir do token
            CreateMap<CadastrarAnimalClinicaDto, Animal>()
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore())
                .ForMember(dest => dest.IsAtivo, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DataModificado, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Nascimento, opt => opt.MapFrom(src => src.Nascimento.ToUniversalTime()));

            // Atualização parcial: campo nulo no DTO mantém o valor atual (ver MappingExtensions)
            CreateMap<EditarAnimalClinicaDto, Animal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .IgnorarNulosDaOrigem();

            // Mapeia os dados de estado de um relatório para a entidade Animal.
            // Usado para atualizar o estado atual do animal sempre que um novo relatório é salvo.
            CreateMap<UpsertAvaliacaoDto, Animal>();
        }
    }
}