using backend.Domain.Utils;
using AutoMapper;
using backend.Domain.General.Extensions;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.SaudeAnimal.Services;

namespace backend.Domain.SaudeAnimal.Mappers;

public class DietaMappingProfile : Profile
{
    public DietaMappingProfile()
    {
        CreateMap<CreateDietaDto, Dieta>()
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(_ => DateOnly.FromDateTime(DateTime.UtcNow)));

        // Mapeamento para CONSULTAR: De Entidade para DTO
        CreateMap<Dieta, GetDietaDto>()
            .ForMember(
                dest => dest.NomeRacao, // Para o campo 'NomeRacao' no DTO...
                opt => opt.MapFrom(src => src.Racao.Nome) // ...busque o valor em src.Racao.Nome
            )
            .ForMember(dest => dest.StatusLabel, opt => opt.MapFrom(src => src.Status.GetDisplayName()))
            .ForMember(dest => dest.NivelRestricaoLabel, opt => opt.MapFrom(src => src.NivelRestricao.HasValue ? src.NivelRestricao.Value.GetDisplayName() : null))
            .ForMember(dest => dest.SemanaAtual, opt => opt.MapFrom(src => DietaPlanoHelper.CalcularSemanaAtual(src, null)))
            .ForMember(dest => dest.FatorRestricao, opt => opt.MapFrom(src => DietaPlanoHelper.FatorRestricao(src)))
            .ForMember(dest => dest.PercentualPerdaSemanal, opt => opt.MapFrom(src => DietaPlanoHelper.PercentualPerdaSemanal(src)))
            .ForMember(dest => dest.PerdaSemanalKg, opt => opt.MapFrom(src => DietaPlanoHelper.PerdaSemanalKg(src)))
            .ForMember(dest => dest.RegistrosPeso, opt => opt.MapFrom(src => src.RegistrosPeso.OrderBy(r => r.Semana)));

        CreateMap<RegistroPesoDieta, GetRegistroPesoDto>();

        // Mapeamento para ATUALIZAR: De DTO para Entidade (para o futuro)
        // Ignora campos nulos do DTO, permitindo atualizações parciais
        CreateMap<UpdateDietaDto, Dieta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignora o mapeamento do Id para evitar alterações acidentais
            .IgnorarNulosDaOrigem(); // campo nulo no DTO mantém o valor atual (ver MappingExtensions)

        CreateMap<UpdateDietaDto, CreateDietaDto>().ForMember(dest => dest.AvaliacaoId, opt => opt.Ignore());
    }
}