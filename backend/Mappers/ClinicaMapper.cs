using backend.Dtos.Clinica;
using backend.Models;

namespace backend.Mappers
{
    public static class ClinicaMapper
    {
        public static ClinicaDto ToClinicaDto(this Clinica clinicaModel)
        {
            return new ClinicaDto
            {
                Id = clinicaModel.Id,
                NomeClinica = clinicaModel.NomeClinica,
                NomeFantasia = clinicaModel.NomeFantasia,
                Cnpj = clinicaModel.Cnpj,
                Endereco = clinicaModel.Endereco,
                Cep = clinicaModel.Cep,
                Whatsapp = clinicaModel.Whatsapp,
                Email = clinicaModel.Email,
                UsuariosPerfisClinicas = clinicaModel.UsuariosPerfisClinicas.Select(upc => upc.ToUsuarioPerfilClinicaDto()).ToList(),
                TutoresAnimaisClinicas = clinicaModel.TutoresAnimaisClinicas.Select(tac => tac.ToTutorAnimalClinicaDto()).ToList()
            };
        }

        public static Clinica ToClinicaFromDto(this ClinicaDto clinicaDto)
        {
            return new Clinica
            {
                Id = clinicaDto.Id,
                NomeClinica = clinicaDto.NomeClinica,
                NomeFantasia = clinicaDto.NomeFantasia,
                Cnpj = clinicaDto.Cnpj,
                Endereco = clinicaDto.Endereco,
                Cep = clinicaDto.Cep,
                Whatsapp = clinicaDto.Whatsapp,
                Email = clinicaDto.Email,
                UsuariosPerfisClinicas = clinicaDto.UsuariosPerfisClinicas.Select(upc => upc.ToUsuarioPerfilClinicaFromDto()).ToList(),
                TutoresAnimaisClinicas = clinicaDto.TutoresAnimaisClinicas.Select(tac => tac.ToTutorAnimalClinicaFromDto()).ToList()
            };
        }
    }
}
