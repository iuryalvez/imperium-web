using backend.Domain.Utils;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    public class RacaMappingProfile : Profile
    {
        public RacaMappingProfile()
        {
            // Mapping from the 'Create' DTO to the Entity. ClinicaId, status e datas são definidos pelo service.
            CreateMap<CadastrarRacaDto, Raca>()
                .ForMember(dest => dest.CategoriaMetabolica, opt => opt.Condition(src => src.CategoriaMetabolica != null));

            // Edição (e cópia de raça do sistema): campo nulo no DTO mantém o valor atual (ver MappingExtensions)
            CreateMap<UpdateRacaDto, Raca>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore())
                .ForMember(dest => dest.IsAtivo, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
                .ForMember(dest => dest.DataDesativacao, opt => opt.Ignore())
                .IgnorarNulosDaOrigem();

            // Mapping from the Entity to the 'Read' DTO
            CreateMap<Raca, GetRacaDto>()
                .ForMember(dest => dest.IsSistema, opt => opt.MapFrom(src => src.ClinicaId == null))
                .ForMember(dest => dest.NomeClinica, opt => opt.MapFrom(src => src.Clinica != null ? src.Clinica.NomeClinica : null))
                .ForMember(dest => dest.IsOculta, opt => opt.Ignore());
        }
    }
}
