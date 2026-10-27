using AutoMapper;
using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.DTOs.Dashboard;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.GestaoClinica.Mappers
{
    public class ClinicaMappingProfile : Profile
    {
        public ClinicaMappingProfile()
        {
            // De Entidade para DTO de Consulta (GET)
            CreateMap<Clinica, GetClinicaDto>();

            // De DTO de Cadastro para Entidade
            CreateMap<CadastrarClinicaDto, Clinica>();

            // De DTO de Edição (Admin) para Entidade
            // Configurado para ignorar campos nulos, permitindo atualizações parciais (PATCH)
            CreateMap<EditarClinicaAdminDto, Clinica>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // De DTO de Edição (Clínica) para Entidade
            CreateMap<EditarClinicaDto, Clinica>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}