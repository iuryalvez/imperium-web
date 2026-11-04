using backend.Domain.Utils;
using AutoMapper;
using backend.Domain.Usuarios.DTOs.Usuario;
using backend.Domain.Usuarios.DTOs.Vinculo;
using backend.Domain.Usuarios.DTOs.PerfilPessoal;
using backend.Domain.Usuarios.Entities;

namespace backend.Domain.Usuarios.Mappers
{
    public class UsuarioMappingProfile : Profile
    {
        public UsuarioMappingProfile()
        {
            CreateMap<CadastrarUsuarioSistemaDto, Usuario>();
            CreateMap<CadastrarUsuarioClinicaDto, Usuario>();

            CreateMap<EditarUsuarioSistemaDto, Usuario>()
                .IgnorarNulosDaOrigem(); // campo nulo no DTO mantém o valor atual (ver MappingExtensions)

            CreateMap<Usuario, GetUsuariosSistemaDto>()
            .ForMember(dest => dest.CompositeKey, opt => opt.MapFrom(src => $"admin-{src.Id}"))
            .ForMember(dest => dest.UpcId, opt => opt.MapFrom(src => -1))
            .ForMember(dest => dest.NomeClinica, opt => opt.MapFrom(src => "N/A"))
            .ForMember(dest => dest.ClinicaId, opt => opt.MapFrom(src => -1))
            .ForMember(dest => dest.Perfis, opt => opt.MapFrom(src =>
                src.IsAtivo ? new List<string> { "ADMIN" } : new List<string> { "ADMIN", "INATIVO" }))
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCadastro))
            .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.Telefone));

            CreateMap<UsuarioPerfilClinica, GetUsuariosSistemaDto>()
                .ForMember(dest => dest.CompositeKey, opt => opt.MapFrom(src => $"link-{src.Id}"))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UpcId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Usuario.UserName))
                .ForMember(dest => dest.NomeClinica, opt => opt.MapFrom(src => src.Clinica.NomeClinica))
                .ForMember(dest => dest.ClinicaId, opt => opt.MapFrom(src => src.ClinicaId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.Perfis, opt => opt.MapFrom(src => src.Perfis))
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.Usuario.DataCadastro))
                .ForMember(dest => dest.DataUltimoAcesso, opt => opt.MapFrom(src => src.Usuario.DataUltimoAcesso))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.Usuario.IsAdmin))
                // Cada linha é um vínculo: o status mostrado (e alterado pelo botão/switch) é o do vínculo
                .ForMember(dest => dest.IsAtivo, opt => opt.MapFrom(src => !(src.Perfis != null && src.Perfis.Contains("INATIVO"))))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.Usuario.Cpf))
                .ForMember(dest => dest.Formacao, opt => opt.MapFrom(src => src.Usuario.Formacao))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.Usuario.Telefone))
                .ForMember(dest => dest.Whatsapp, opt => opt.MapFrom(src => src.Usuario.Whatsapp));

            CreateMap<UsuarioPerfilClinica, GetUsuariosClinicaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UpcId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Usuario.UserName))
                .ForMember(dest => dest.ClinicaId, opt => opt.MapFrom(src => src.ClinicaId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.Perfis, opt => opt.MapFrom(src => src.Perfis))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.Usuario.Cpf))
                .ForMember(dest => dest.Formacao, opt => opt.MapFrom(src => src.Usuario.Formacao))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.Usuario.Telefone))
                .ForMember(dest => dest.Whatsapp, opt => opt.MapFrom(src => src.Usuario.Whatsapp))
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.Usuario.DataCadastro))
                .ForMember(dest => dest.DataUltimoAcesso, opt => opt.MapFrom(src => src.Usuario.DataUltimoAcesso))
                .ForMember(dest => dest.IsAtivoNaClinica, opt => opt.MapFrom(src => !(src.Perfis != null && src.Perfis.Contains("INATIVO"))));

            CreateMap<Usuario, GetMeuPerfilDto>()
                .ForMember(dest => dest.TotalClinicasVinculadas, opt => opt.MapFrom(src => 
                    src.UsuariosPerfisClinicas != null ? src.UsuariosPerfisClinicas.Count : 0));
            CreateMap<UpdateMeuPerfilDto, Usuario>()
                .IgnorarNulosDaOrigem(); // campo nulo no DTO mantém o valor atual (ver MappingExtensions)
        }
    }
}