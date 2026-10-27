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

            CreateMap<CadastrarTutorClinicaDto, Tutor>();
            CreateMap<EditarTutorClinicaDto, Tutor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}