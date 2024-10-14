using backend.Dtos.AcompanhamentoInd;
using backend.Dtos.Consulta;
using backend.Models;

namespace backend.Mappers
{
    public static class AcompanhamentoIndMapper
    {
        public static AcompanhamentoIndDto ToAcompanhamentoIndDto(this AcompanhamentoInd acompanhamentoIndModel)
        {
            return new AcompanhamentoIndDto
            {
                Id = acompanhamentoIndModel.Id,
                AnimalId = acompanhamentoIndModel.AnimalId,
                DietasPersonalizadas = acompanhamentoIndModel.DietasPersonalizadas.Select(dp => dp.ToDietaPersonalizadaDto()).ToList(),
                Consultas = acompanhamentoIndModel.Consultas.Select(c => c.ToConsultaDto()).ToList(),
                DataInicio = acompanhamentoIndModel.DataInicio,
                DataFim = acompanhamentoIndModel.DataFim
            };
        }
        public static AcompanhamentoInd ToAcompanhamentoIndFromCreateDTO (this CreateAcompanhamentoIndRequestDto acompanhamentoIndDto) 
        {
            return new AcompanhamentoInd
            {
                Id = acompanhamentoIndDto.Id,
                AnimalId = acompanhamentoIndDto.AnimalId,
                DietasPersonalizadas = acompanhamentoIndDto.DietasPersonalizadas.Select(dp => dp.ToDietaPersonalizadaFromDto()).ToList(),
                Consultas = acompanhamentoIndDto.Consultas.Select(c => c.ToConsultaFromDto()).ToList(),
                DataInicio = acompanhamentoIndDto.DataInicio,
                DataFim = acompanhamentoIndDto.DataFim
            };
        }
    }
}