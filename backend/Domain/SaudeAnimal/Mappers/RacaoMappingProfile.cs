using backend.Domain.Utils;
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

            // Mapeamento para ATUALIZAR: De DTO para Entidade (atualização parcial: campo nulo no DTO mantém o valor atual)
            CreateMap<AtualizarRacaoDto, Racao>()
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore())
                .IgnorarNulosDaOrigem(); // ver MappingExtensions

            // Mapeamento para CONSULTAR: De Entidade para DTO
            CreateMap<Racao, GetRacoesDto>()
                .ForMember(dest => dest.IsSistema, opt => opt.MapFrom(src => src.ClinicaId == null))
                .ForMember(dest => dest.NomeClinica, opt => opt.MapFrom(src => src.Clinica != null ? src.Clinica.NomeClinica : null))
                .ForMember(dest => dest.IsOculta, opt => opt.Ignore());
        }
    }
}