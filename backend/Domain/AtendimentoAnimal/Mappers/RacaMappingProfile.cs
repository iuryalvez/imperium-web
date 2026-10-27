using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    public class RacaMappingProfile : Profile
    {
        public RacaMappingProfile()
        {
            // Mapping from the 'Create' DTO to the Entity
            CreateMap<CadastrarRacaDto, Raca>();

            // Mapping from the 'Update' DTO to the Entity
            CreateMap<UpdateRacaDto, Raca>();

            // Mapping from the Entity to the 'Read' DTO
            // This one has a custom rule for the 'Especie' property
            CreateMap<Raca, GetRacaDto>();
        }
    }
}