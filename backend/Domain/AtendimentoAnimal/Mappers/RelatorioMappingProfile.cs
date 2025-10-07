using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    public class RelatorioMappingProfile : Profile
    {
        public RelatorioMappingProfile()
        {
            CreateMap<UpsertRelatorioDto, Relatorio>();

            CreateMap<Relatorio, GetRelatorioDto>()
                // Mapeia a propriedade 'Dieta' do DTO a partir da propriedade 'DietaPersonalizada' da entidade.
                // O AutoMapper usará automaticamente o 'DietaMappingProfile' que criamos para fazer a conversão interna.
                .ForMember(
                    dest => dest.Dieta,
                    opt => opt.MapFrom(src => src.DietaPersonalizada)
                )
                // Adiciona a data da consulta, que está em uma entidade relacionada
                .ForMember(
                    dest => dest.DataConsulta,
                    opt => opt.MapFrom(src => src.Consulta.DataConsulta)
                );
        }
    }
}