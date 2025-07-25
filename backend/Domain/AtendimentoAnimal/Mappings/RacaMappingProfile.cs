using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappings
{
    public class RacaMappingProfile : Profile
    {
        public RacaMappingProfile()
        {
            // Mapping from the 'Create' DTO to the Entity
            CreateMap<CadastrarRacaoDto, Raca>();

            // Mapping from the 'Update' DTO to the Entity
            CreateMap<UpdateRacaDto, Raca>();

            // Mapping from the Entity to the 'Read' DTO
            // This one has a custom rule for the 'Especie' property
            CreateMap<Raca, GetRacaDto>()
                .ForMember(
                    dest => dest.Especie,
                    opt => opt.MapFrom(src => src.Especie.ToString()) // Converts enum to string
                );
        }
    }
}