using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    public class AvaliacaoMappingProfile : Profile
    {
        public AvaliacaoMappingProfile()
        {
            CreateMap<UpsertAvaliacaoDto, Avaliacao>();

            CreateMap<Avaliacao, GetAvaliacaoDto>()
                .ForMember(dest => dest.Dieta, opt => opt.MapFrom(src => src.Dieta))
                .ForMember(dest => dest.DataConsulta, opt => opt.MapFrom(src =>
                    src.Consulta != null ? src.Consulta.DataConsulta : default))
                .ForMember(dest => dest.NomeRacao, opt => opt.MapFrom(src =>
                    src.Racao != null ? src.Racao.Nome : string.Empty))
                .ForMember(dest => dest.SexoAnimal, opt => opt.MapFrom(src =>
                    src.Consulta != null && src.Consulta.Animal != null ? src.Consulta.Animal.Sexo : default))
                .ForMember(dest => dest.Idade, opt => opt.MapFrom(src =>
                    src.Consulta != null && src.Consulta.Animal != null ? src.Consulta.Animal.Idade : 0));
        }
    }
}