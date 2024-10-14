using backend.Dtos.DietaPersonalizada;
using backend.Models;

namespace backend.Mappers
{
    public static class DietaPersonalizadaMapper
    {
        public static DietaPersonalizadaDto ToDietaPersonalizadaDto(this DietaPersonalizada dietaModel)
        {
            return new DietaPersonalizadaDto
            {
                Id = dietaModel.Id,
                RacaoId = dietaModel.RacaoId,
                AcompanhamentoIndId = dietaModel.AcompanhamentoIndId,
                TipoDieta = dietaModel.TipoDieta,
                PesoAtual = dietaModel.PesoAtual,
                PesoObjetivo = dietaModel.PesoObjetivo,
                PesoIdeal = dietaModel.PesoIdeal
            };
        }

        public static DietaPersonalizada ToDietaPersonalizadaFromDto(this DietaPersonalizadaDto dietaDto)
        {
            return new DietaPersonalizada
            {
                Id = dietaDto.Id,
                RacaoId = dietaDto.RacaoId,
                AcompanhamentoIndId = dietaDto.AcompanhamentoIndId,
                TipoDieta = dietaDto.TipoDieta,
                PesoAtual = dietaDto.PesoAtual,
                PesoObjetivo = dietaDto.PesoObjetivo,
                PesoIdeal = dietaDto.PesoIdeal
            };
        }
    }
}
