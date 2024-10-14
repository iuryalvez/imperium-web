using backend.Dtos.Relatorio;
using backend.Models;

namespace backend.Mappers
{
    public static class RelatorioMapper
    {
        public static RelatorioDto ToRelatorioDto(this Relatorio relatorioModel)
        {
            return new RelatorioDto
            {
                Id = relatorioModel.Id,
                ConsultaId = relatorioModel.ConsultaId,
                Idade = relatorioModel.Idade,
                Peso = relatorioModel.Peso,
                CondicaoCorporal = relatorioModel.CondicaoCorporal,
                GrauAtividade = relatorioModel.GrauAtividade,
                Racao = relatorioModel.Racao,
                DescDieta = relatorioModel.DescDieta
            };
        }

        public static Relatorio ToRelatorioFromDto(this RelatorioDto relatorioDto)
        {
            return new Relatorio
            {
                Id = relatorioDto.Id,
                ConsultaId = relatorioDto.ConsultaId,
                Idade = relatorioDto.Idade,
                Peso = relatorioDto.Peso,
                CondicaoCorporal = relatorioDto.CondicaoCorporal,
                GrauAtividade = relatorioDto.GrauAtividade,
                Racao = relatorioDto.Racao,
                DescDieta = relatorioDto.DescDieta
            };
        }
    }
}
