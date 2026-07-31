using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;

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
                // Navegações Básicas Seguras
                .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(src => src.Tutor != null ? src.Tutor.Nome : null))
                .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nome : null))
                .ForMember(dest => dest.NomeAnimal, opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Nome : null))
                .ForMember(dest => dest.EspecieAnimal, opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Especie : default))
                .ForMember(dest => dest.SexoAnimal, opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Sexo : default))

                // Relatório e Dieta
                // Sobrescreve agressivamente qualquer booleano do banco: se o objeto existe, é true.
                .ForMember(dest => dest.RelatorioPreenchido, opt => opt.MapFrom(src => src.Relatorio != null))
                .ForMember(dest => dest.DietaId, opt => opt.MapFrom(src =>
                    src.Relatorio != null && src.Relatorio.DietaPersonalizada != null
                    ? src.Relatorio.DietaPersonalizada.Id : (int?)null))

                // Acompanhamento e Raça
                .ForMember(dest => dest.AcompanhamentoId, opt => opt.MapFrom(src =>
                    src.Animal != null && src.Animal.Acompanhamento != null
                    ? src.Animal.Acompanhamento.Id : 0))
                .ForMember(dest => dest.NomeRaca, opt => opt.MapFrom(src =>
                    src.Animal != null && src.Animal.Raca != null ? src.Animal.Raca.Nome : null))

                // Inteligência de Pesos baseada no Sexo
                .ForMember(dest => dest.PesoMinimoRaca, opt => opt.MapFrom(src =>
                    src.Animal != null && src.Animal.Raca != null
                        ? (src.Animal.Sexo == Sexo.Macho ? src.Animal.Raca.PesoMinimoReferenciaMacho : src.Animal.Raca.PesoMinimoReferenciaFemea)
                        : null))
                .ForMember(dest => dest.PesoMaximoRaca, opt => opt.MapFrom(src =>
                    src.Animal != null && src.Animal.Raca != null
                        ? (src.Animal.Sexo == Sexo.Macho ? src.Animal.Raca.PesoMaximoReferenciaMacho : src.Animal.Raca.PesoMaximoReferenciaFemea)
                        : null));

            // Mapeamento para os DETALHES de uma consulta
            CreateMap<Consulta, DetalhesConsultaDto>()
                .ForMember(dest => dest.NomeAnimal, opt => opt.MapFrom(src => src.Animal.Nome))
                .ForMember(dest => dest.NomeTutor, opt => opt.MapFrom(src => src.Tutor.Nome))
                .ForMember(dest => dest.CpfTutor, opt => opt.MapFrom(src => src.Tutor.Cpf))
                .ForMember(dest => dest.EmailTutor, opt => opt.MapFrom(src => src.Tutor.Email))
                .ForMember(dest => dest.TelefoneTutor, opt => opt.MapFrom(src => src.Tutor.Telefone))
                .ForMember(dest => dest.NomeVeterinario, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.RelatorioId, opt => opt.MapFrom(src => src.Relatorio != null ? src.Relatorio.Id : (int?)null));
        }
    }
}