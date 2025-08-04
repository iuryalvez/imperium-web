using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            .ForMember(dest => dest.NomeRacao, opt => opt.MapFrom(src => src.Racao.Nome));
        }
    }
}