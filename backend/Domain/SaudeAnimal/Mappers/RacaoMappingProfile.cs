using AutoMapper;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.SaudeAnimal.DTOs.Racao;

namespace backend.Domain.SaudeAnimal.Mappers
{
    public class RacaoMappingProfile : Profile
    {
        public RacaoMappingProfile()
        {
            // Mapeamento para CRIAR: De DTO para Entidade
            CreateMap<CadastrarRacaoDto, Racao>()
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore());

            // Mapeamento para ATUALIZAR: De DTO para Entidade
            // Esta configuração especial ignora propriedades nulas do DTO de origem,
            // o que implementa perfeitamente a nossa lógica de atualização parcial (PATCH).
            CreateMap<AtualizarRacaoDto, Racao>()
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeamento para CONSULTAR: De Entidade para DTO
            CreateMap<Racao, GetRacoesDto>()
                .ForMember(dest => dest.IsSistema, opt => opt.MapFrom(src => src.ClinicaId == null));
        }
    }
}