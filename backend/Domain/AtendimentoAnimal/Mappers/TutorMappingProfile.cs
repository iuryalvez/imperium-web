using backend.Domain.Utils;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    public class TutorMappingProfile : Profile
    {
        public TutorMappingProfile()
        {
            CreateMap<Tutor, GetTutoresClinicaDto>()
                .ForMember(
                    dest => dest.Animais, // Para a lista 'Animais' no DTO...
                    opt => opt.MapFrom(src =>
                        // ...mapeie a partir da coleção de vínculos 'TutoresAnimais',
                        // convertendo cada animal vinculado para um AnimalInfoSimplesDto.
                        src.TutoresAnimais.Select(ta => new AnimalInfoSimplesDto
                        {
                            Id = ta.Animal.Id,
                            Nome = ta.Animal.Nome,
                            Especie = ta.Animal.Especie
                        }))
                );

            // ClinicaId, Id e status nunca vêm do DTO: são definidos pelo service a partir do token
            CreateMap<CadastrarTutorClinicaDto, Tutor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore())
                .ForMember(dest => dest.IsAtivo, opt => opt.Ignore())
                .ForMember(dest => dest.DataDesativacao, opt => opt.Ignore());
            CreateMap<EditarTutorClinicaDto, Tutor>()
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore())
                .ForMember(dest => dest.IsAtivo, opt => opt.Ignore())
                .ForMember(dest => dest.DataDesativacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
                .IgnorarNulosDaOrigem(); // campo nulo no DTO mantém o valor atual (ver MappingExtensions)
        }
    }
}