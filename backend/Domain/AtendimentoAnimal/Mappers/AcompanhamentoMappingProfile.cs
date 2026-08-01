using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.General.Extensions;

namespace backend.Domain.AtendimentoAnimal.Mappers 
{
    public class AcompanhamentoProfile : Profile
    {
        public AcompanhamentoProfile()
        {
            // Mapeamento Principal: Da Entidade Animal para o DTO Completo
            CreateMap<Animal, AnimalCompletoDto>()
                .ForMember(dest => dest.Especie, opt => opt.MapFrom(src => src.Especie.ToString()))
                .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo.ToString()))
                .ForMember(dest => dest.FaseVida, opt => opt.MapFrom(src => src.FaseVida.ToString()))
                .ForMember(dest => dest.FaseVidaLabel, opt => opt.MapFrom(src => src.FaseVida.GetDisplayName()))
                .ForMember(dest => dest.CondicaoCorporalLabel, opt => opt.MapFrom(src => src.CondicaoCorporal.GetDisplayName()))
                .ForMember(dest => dest.PesoAtual, opt => opt.MapFrom(src => src.Peso))
                .ForMember(dest => dest.CondicaoCorporal, opt => opt.MapFrom(src => src.CondicaoCorporal.ToString()))
                // O AutoMapper usará o mapeamento de 'Tutor' para 'TutorResumidoDto' definido abaixo
                .ForMember(dest => dest.Tutores, opt => opt.MapFrom(src => 
                    src.TutoresAnimais != null 
                    ? src.TutoresAnimais.Select(ta => ta.Tutor) 
                    : new List<Tutor>()))
                // O AutoMapper usará o mapeamento de 'Acompanhamento' para 'AcompanhamentoDto'
                .ForMember(dest => dest.Acompanhamento, opt => opt.MapFrom(src => src.Acompanhamento));

            // Mapeamentos Auxiliares (para os DTOs aninhados)

            CreateMap<Raca, RacaResumidaDto>();

            CreateMap<Tutor, TutorResumidoDto>();

            CreateMap<Acompanhamento, AcompanhamentoDto>();

            CreateMap<Avaliacao, AvaliacaoDetalhadoDto>()
                // Busca a data da consulta aninhada no relatório
                .ForMember(dest => dest.DataConsulta, opt => opt.MapFrom(src => 
                    src.Consulta != null ? src.Consulta.DataConsulta : default))
                // Busca o nome do profissional aninhado na consulta
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.MapFrom(src => 
                    src.Consulta != null && src.Consulta.Usuario != null 
                    ? src.Consulta.Usuario.Nome // Supondo que a entidade Usuario tem a prop 'Nome'
                    : "Não informado"))
                .ForMember(dest => dest.CondicaoCorporal, opt => opt.MapFrom(src => src.CondicaoCorporal.ToString()))
                .ForMember(dest => dest.GrauAtividade, opt => opt.MapFrom(src => src.GrauAtividade.ToString()))
                // O AutoMapper usará o mapeamento de 'Dieta' para 'DietaResumidaDto'
                .ForMember(dest => dest.Dieta, opt => opt.MapFrom(src => src.Dieta));
            
            CreateMap<Dieta, DietaResumidaDto>()
                .ForMember(dest => dest.TipoDieta, opt => opt.MapFrom(src => src.TipoDieta.ToString()))
                .ForMember(dest => dest.TipoDietaLabel, opt => opt.MapFrom(src => src.TipoDieta.GetDisplayName()))
                // Garante que não haverá erro se o valor for nulo
                .ForMember(dest => dest.QuantidadeAlimentoGramas, opt => opt.MapFrom(src => src.QtdAlimento ?? 0))
                .ForMember(dest => dest.NecessidadeEnergeticaKcal, opt => opt.MapFrom(src => src.EnergiaMetabolizavel ?? 0))
                // Busca o nome da ração aninhada na dieta
                .ForMember(dest => dest.RacaoUtilizada, opt => opt.MapFrom(src => 
                    src.Racao != null ? src.Racao.Nome : "Ração não informada"));
        }
    }
}