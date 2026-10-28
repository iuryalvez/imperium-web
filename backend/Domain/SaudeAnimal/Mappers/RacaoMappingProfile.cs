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

            // Mapeamento para ATUALIZAR: De DTO para Entidade (atualização parcial: campo nulo no DTO mantém o valor atual).
            // A condição olha o DTO de origem: com ForAllMembers(srcMember != null), um double?/enum? nulo
            // chegava já convertido para 0 e zerava o campo (ex.: espécie 0 quebrava o cálculo de EM).
            CreateMap<AtualizarRacaoDto, Racao>()
                .ForMember(dest => dest.ClinicaId, opt => opt.Ignore())
                .ForMember(dest => dest.Nome, opt => opt.Condition(src => src.Nome != null))
                .ForMember(dest => dest.Especie, opt => opt.Condition(src => src.Especie != null))
                .ForMember(dest => dest.Umidade, opt => opt.Condition(src => src.Umidade != null))
                .ForMember(dest => dest.ProteinaBruta, opt => opt.Condition(src => src.ProteinaBruta != null))
                .ForMember(dest => dest.ExtratoEtereo, opt => opt.Condition(src => src.ExtratoEtereo != null))
                .ForMember(dest => dest.MateriaMineral, opt => opt.Condition(src => src.MateriaMineral != null))
                .ForMember(dest => dest.FibraBruta, opt => opt.Condition(src => src.FibraBruta != null));

            // Mapeamento para CONSULTAR: De Entidade para DTO
            CreateMap<Racao, GetRacoesDto>()
                .ForMember(dest => dest.IsSistema, opt => opt.MapFrom(src => src.ClinicaId == null))
                .ForMember(dest => dest.NomeClinica, opt => opt.MapFrom(src => src.Clinica != null ? src.Clinica.NomeClinica : null))
                .ForMember(dest => dest.IsOculta, opt => opt.Ignore());
        }
    }
}