using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs;
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

            CreateMap<CadastrarAnimalClinicaDto, Animal>()
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DataModificado, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Nascimento, opt => opt.MapFrom(src => src.Nascimento.ToUniversalTime()));

            CreateMap<EditarAnimalClinicaDto, Animal>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}