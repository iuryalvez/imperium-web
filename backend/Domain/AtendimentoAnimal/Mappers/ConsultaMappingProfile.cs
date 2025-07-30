using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
public class ConsultaMappingProfile : Profile
{
    public ConsultaMappingProfile()
    {
        // Mapeamento para CRIAR e ATUALIZAR
        CreateMap<UpsertConsultaDto, Consulta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignora o ID ao criar

        // Mapeamento para a LISTA de consultas
        CreateMap<Consulta, GetConsultaDto>()
            .ForMember(dest => dest.NomeAnimal, opt => opt.MapFrom(src => src.Animal.Nome))
            .ForMember(dest => dest.EspecieAnimal, opt => opt.MapFrom(src => src.Animal.Especie))
            .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(src => src.Tutor.Nome))
            .ForMember(dest => dest.NomeVeterinario, opt => opt.MapFrom(src => src.Usuario.Nome))
            .ForMember(dest => dest.RelatorioPreenchido, opt => opt.MapFrom(src => src.Relatorio != null));

        // Mapeamento para os DETALHES de uma consulta
        CreateMap<Consulta, DetalhesConsultaDto>()
            .ForMember(dest => dest.NomeAnimal, opt => opt.MapFrom(src => src.Animal.Nome))
            .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(src => src.Tutor.Nome))
            .ForMember(dest => dest.CpfTutor, opt => opt.MapFrom(src => src.Tutor.Cpf))
            .ForMember(dest => dest.EmailTutor, opt => opt.MapFrom(src => src.Tutor.Email))
            .ForMember(dest => dest.TelefoneTutor, opt => opt.MapFrom(src => src.Tutor.Telefone1))
            .ForMember(dest => dest.NomeVeterinario, opt => opt.MapFrom(src => src.Usuario.Nome))
            .ForMember(dest => dest.RelatorioId, opt => opt.MapFrom(src => src.Relatorio != null ? src.Relatorio.Id : (int?)null));
    }
}
}