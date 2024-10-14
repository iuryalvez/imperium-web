using backend.Dtos.Consulta;
using backend.Models;

namespace backend.Mappers
{
    public static class ConsultaMapper
    {
        public static ConsultaDto ToConsultaDto(this Consulta consultaModel)
        {
            return new ConsultaDto
            {
                Id = consultaModel.Id,
                UsuarioId = consultaModel.UsuarioId,
                TutorId = consultaModel.TutorId,
                AcompanhamentoIndId = consultaModel.AcompanhamentoIndId,
                Date = consultaModel.Date
            };
        }

        public static Consulta ToConsultaFromDto(this ConsultaDto consultaDto)
        {
            return new Consulta
            {
                Id = consultaDto.Id,
                UsuarioId = consultaDto.UsuarioId,
                TutorId = consultaDto.TutorId,
                AcompanhamentoIndId = consultaDto.AcompanhamentoIndId,
                Date = consultaDto.Date
            };
        }
    }
}
