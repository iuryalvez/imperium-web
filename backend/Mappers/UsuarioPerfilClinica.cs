using backend.Dtos.UsuarioPerfilClinica;
using backend.Models;

namespace backend.Mappers
{
    public static class UsuarioPerfilClinicaMapper
    {
        public static UsuarioPerfilClinicaDto ToUsuarioPerfilClinicaDto(this UsuarioPerfilClinica usuarioPerfilClinicaModel)
        {
            return new UsuarioPerfilClinicaDto
            {
                Id = usuarioPerfilClinicaModel.Id,
                UsuarioId = usuarioPerfilClinicaModel.UsuarioId,
                PerfilId = usuarioPerfilClinicaModel.PerfilId,
                ClinicaId = usuarioPerfilClinicaModel.ClinicaId
            };
        }

        public static UsuarioPerfilClinica ToUsuarioPerfilClinicaFromDto(this UsuarioPerfilClinicaDto usuarioPerfilClinicaDto)
        {
            return new UsuarioPerfilClinica
            {
                Id = usuarioPerfilClinicaDto.Id,
                UsuarioId = usuarioPerfilClinicaDto.UsuarioId,
                PerfilId = usuarioPerfilClinicaDto.PerfilId,
                ClinicaId = usuarioPerfilClinicaDto.ClinicaId
            };
        }
    }
}
