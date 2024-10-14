using backend.Dtos.Racao;
using backend.Models;

namespace backend.Mappers
{
    public static class RacaoMapper
    {
        public static RacaoDto ToRacaoDto(this Racao racaoModel)
        {
            return new RacaoDto
            {
                Id = racaoModel.Id,
                Nome = racaoModel.Nome,
                Umidade = racaoModel.Umidade,
                Pb = racaoModel.Pb,
                Eea = racaoModel.Eea,
                Mm = racaoModel.Mm,
                Fb = racaoModel.Fb,
                Ms = racaoModel.Ms
            };
        }

        public static Racao ToRacaoFromDto(this RacaoDto racaoDto)
        {
            return new Racao
            {
                Id = racaoDto.Id,
                Nome = racaoDto.Nome,
                Umidade = racaoDto.Umidade,
                Pb = racaoDto.Pb,
                Eea = racaoDto.Eea,
                Mm = racaoDto.Mm,
                Fb = racaoDto.Fb,
                Ms = racaoDto.Ms
            };
        }
    }
}
