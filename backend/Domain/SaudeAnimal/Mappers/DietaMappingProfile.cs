using AutoMapper;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;

public class DietaMappingProfile : Profile
{
    public DietaMappingProfile()
    {
        // Mapeamento para CONSULTAR: De Entidade para DTO
        CreateMap<DietaPersonalizada, GetDietaDto>()
            .ForMember(
                dest => dest.NomeRacao, // Para o campo 'NomeRacao' no DTO...
                opt => opt.MapFrom(src => src.Racao.Nome) // ...busque o valor em src.Racao.Nome
            );

        // Mapeamento para ATUALIZAR: De DTO para Entidade (para o futuro)
        // Ignora campos nulos do DTO, permitindo atualizações parciais
        CreateMap<UpdateDietaDto, DietaPersonalizada>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}