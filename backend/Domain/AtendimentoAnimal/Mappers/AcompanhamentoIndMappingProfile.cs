using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Acompanhamento;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
public class AcompanhamentoMappingProfile : Profile
{
    public AcompanhamentoMappingProfile()
    {
        CreateMap<AcompanhamentoInd, GetAcompanhamentoDto>()
            // Mapeia o nome do animal, que está em uma entidade relacionada
            .ForMember(
                dest => dest.NomeAnimal,
                opt => opt.MapFrom(src => src.Animal.Nome)
            );
        
        // IMPORTANTE: O AutoMapper é inteligente. Como já definimos um mapa para 'Relatorio -> GetRelatorioDto',
        // ele aplicará automaticamente essa regra para cada item da coleção 'Relatorios'.
        // Não é necessário um '.ForMember' para a lista de relatórios.
    }
}
}