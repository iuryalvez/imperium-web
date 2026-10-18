using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.General.Extensions;
using backend.Domain.SaudeAnimal.Services;

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
                .ForMember(dest => dest.IdadeFormatada, opt => opt.MapFrom(src => FormatIdade(src.Nascimento)))
                .ForMember(dest => dest.FaseVidaAtual, opt => opt.MapFrom(src => src.FaseVida.ToString()))
                .ForMember(dest => dest.FaseVidaAtualLabel, opt => opt.MapFrom(src => src.FaseVida.GetDisplayName()))
                .ForMember(dest => dest.CondicaoCorporalAtualLabel, opt => opt.MapFrom(src => src.CondicaoCorporal.HasValue ? src.CondicaoCorporal.Value.GetDisplayName() : "Não informado"))
                .ForMember(dest => dest.PesoAtual, opt => opt.MapFrom(src => src.Peso))
                .ForMember(dest => dest.CondicaoCorporalAtual, opt => opt.MapFrom(src => src.CondicaoCorporal.HasValue ? src.CondicaoCorporal.Value.ToString() : null))
                // O AutoMapper usará o mapeamento de 'Tutor' para 'TutorResumidoDto' definido abaixo
                .ForMember(dest => dest.Tutores, opt => opt.MapFrom(src => 
                    src.TutoresAnimais != null 
                    ? src.TutoresAnimais.Select(ta => ta.Tutor) 
                    : new List<Tutor>()))
                // O AutoMapper usará o mapeamento de 'Acompanhamento' para 'AcompanhamentoDto'
                .ForMember(dest => dest.Acompanhamento, opt => opt.MapFrom(src => src.Acompanhamento))
                .ForMember(dest => dest.Consultas, opt => opt.MapFrom(src => src.Consultas));

            // Mapeamentos Auxiliares (para os DTOs aninhados)

            CreateMap<Raca, RacaResumidaDto>();

            CreateMap<Tutor, TutorResumidoDto>();

            CreateMap<Acompanhamento, AcompanhamentoDto>();

            CreateMap<Consulta, ConsultaDetalhadaDto>()
                .ForMember(dest => dest.TutorId, opt => opt.MapFrom(src => src.TutorId))
                .ForMember(dest => dest.ProfissionalResponsavel, opt => opt.MapFrom(src => 
                    src.Usuario != null ? src.Usuario.Nome : "Não informado"))
                .ForMember(dest => dest.Avaliacao, opt => opt.MapFrom(src => src.Avaliacao));

            CreateMap<Avaliacao, AvaliacaoDetalhadoDto>()
                .ForMember(dest => dest.CondicaoCorporal, opt => opt.MapFrom(src => src.CondicaoCorporal.HasValue ? (int?)src.CondicaoCorporal.Value : null))
                .ForMember(dest => dest.GrauAtividade, opt => opt.MapFrom(src => src.GrauAtividade.HasValue ? (int?)src.GrauAtividade.Value : null))
                .ForMember(dest => dest.TipoDieta, opt => opt.MapFrom(src => (int)src.TipoDieta))
                .ForMember(dest => dest.NomeRacao, opt => opt.MapFrom(src => src.Racao != null ? src.Racao.Nome : null))
                // O AutoMapper usará o mapeamento de 'Dieta' para 'DietaResumidaDto'
                .ForMember(dest => dest.Dieta, opt => opt.MapFrom(src => src.Dieta));
            
            CreateMap<Dieta, DietaResumidaDto>()
                .ForMember(dest => dest.TipoDieta, opt => opt.MapFrom(src => (int)src.TipoDieta))
                .ForMember(dest => dest.TipoDietaLabel, opt => opt.MapFrom(src => src.TipoDieta.GetDisplayName()))
                // Garante que não haverá erro se o valor for nulo
                .ForMember(dest => dest.QuantidadeAlimentoGramas, opt => opt.MapFrom(src => src.QtdAlimento ?? 0))
                .ForMember(dest => dest.NecessidadeEnergeticaKcal, opt => opt.MapFrom(src => src.EnergiaMetabolizavel ?? 0))
                // Busca o nome da ração aninhada na dieta
                .ForMember(dest => dest.RacaoUtilizada, opt => opt.MapFrom(src => 
                    src.Racao != null ? src.Racao.Nome : "Ração não informada"))
                // Ciclo de vida e emagrecimento
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.StatusLabel, opt => opt.MapFrom(src => src.Status.GetDisplayName()))
                .ForMember(dest => dest.NivelRestricao, opt => opt.MapFrom(src => src.NivelRestricao.HasValue ? (int?)src.NivelRestricao.Value : null))
                .ForMember(dest => dest.NivelRestricaoLabel, opt => opt.MapFrom(src => src.NivelRestricao.HasValue ? src.NivelRestricao.Value.GetDisplayName() : null))
                .ForMember(dest => dest.SemanaAtual, opt => opt.MapFrom(src => DietaPlanoHelper.CalcularSemanaAtual(src, null)))
                .ForMember(dest => dest.FatorRestricao, opt => opt.MapFrom(src => DietaPlanoHelper.FatorRestricao(src)))
                .ForMember(dest => dest.PercentualPerdaSemanal, opt => opt.MapFrom(src => DietaPlanoHelper.PercentualPerdaSemanal(src)))
                .ForMember(dest => dest.PerdaSemanalKg, opt => opt.MapFrom(src => DietaPlanoHelper.PerdaSemanalKg(src)))
                .ForMember(dest => dest.RegistrosPeso, opt => opt.MapFrom(src => src.RegistrosPeso.OrderBy(r => r.Semana)));
        }

        private static string FormatIdade(DateTime nascimento)
        {
            var hoje = DateTime.Today;
            int anos = hoje.Year - nascimento.Year;
            int meses = hoje.Month - nascimento.Month;

            if (hoje.Day < nascimento.Day)
            {
                meses--;
            }

            if (meses < 0)
            {
                anos--;
                meses += 12;
            }

            if (anos < 0) anos = 0; // Prevenção
            
            if (anos == 0) return $"{meses} meses";
            if (meses == 0) return $"{anos} anos";
            return $"{anos} anos e {meses} meses";
        }
    }
}