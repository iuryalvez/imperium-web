using backend.Domain.AtendimentoAnimal.DTOs.AcompanhamentoDto;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.General.Extensions;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Mappers;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    /// <summary>
    /// Monta o histórico completo do animal (tela de acompanhamento): dados do animal, raça, tutores, consultas,
    /// avaliação e dieta de cada consulta. O código é gerado na compilação pelo Mapperly.
    /// </summary>
    [Mapper]
    public static partial class AcompanhamentoMapper
    {
        /// <summary>Animal com tudo aninhado (precisa do animal carregado por GetHistoricoCompletoAsync).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(nameof(Animal.Peso), nameof(AnimalCompletoDto.PesoAtual))]
        [MapProperty(nameof(Animal.FaseVida), nameof(AnimalCompletoDto.FaseVidaAtual))]
        [MapProperty(nameof(Animal.CondicaoCorporal), nameof(AnimalCompletoDto.CondicaoCorporalAtual))]
        [MapPropertyFromSource(nameof(AnimalCompletoDto.IdadeFormatada), Use = nameof(IdadeFormatada))]
        [MapPropertyFromSource(nameof(AnimalCompletoDto.FaseVidaAtualLabel), Use = nameof(RotuloDaFaseDeVida))]
        [MapPropertyFromSource(nameof(AnimalCompletoDto.CondicaoCorporalAtualLabel), Use = nameof(RotuloDaCondicaoCorporal))]
        [MapPropertyFromSource(nameof(AnimalCompletoDto.Tutores), Use = nameof(TutoresDoAnimal))]
        [MapPropertyFromSource(nameof(AnimalCompletoDto.Consultas), Use = nameof(ConsultasDoAnimal))]
        public static partial AnimalCompletoDto ParaHistorico(Animal animal);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        private static partial RacaResumidaDto ParaResumo(Raca raca);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        private static partial TutorResumidoDto ParaResumo(Tutor tutor);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        private static partial AcompanhamentoDto ParaResumo(Acompanhamento acompanhamento);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(ConsultaDetalhadaDto.TutorId), Use = nameof(IdDoTutorOuZero))]
        [MapPropertyFromSource(nameof(ConsultaDetalhadaDto.ProfissionalResponsavel), Use = nameof(NomeDeQuemMarcou))]
        private static partial ConsultaDetalhadaDto ParaDetalhe(Consulta consulta);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(nameof(@Avaliacao.Racao.Nome), nameof(AvaliacaoDetalhadoDto.NomeRacao))]
        private static partial AvaliacaoDetalhadoDto ParaDetalhe(Avaliacao avaliacao);

        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(DietaResumidaDto.TipoDietaLabel), Use = nameof(RotuloDoTipoDeDieta))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.QuantidadeAlimentoGramas), Use = nameof(QuantidadeDeAlimento))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.NecessidadeEnergeticaKcal), Use = nameof(NecessidadeEnergetica))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.RacaoUtilizada), Use = nameof(NomeDaRacaoDaDieta))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.StatusLabel), Use = nameof(RotuloDoStatus))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.NivelRestricaoLabel), Use = nameof(RotuloDoNivelRestricao))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.SemanaAtual), Use = nameof(SemanaAtual))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.FatorRestricao), Use = nameof(FatorRestricao))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.PercentualPerdaSemanal), Use = nameof(PercentualPerdaSemanal))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.PerdaSemanalKg), Use = nameof(PerdaSemanalKg))]
        [MapPropertyFromSource(nameof(DietaResumidaDto.RegistrosPeso), Use = nameof(RegistrosPorSemana))]
        private static partial DietaResumidaDto ParaResumo(Dieta dieta);

        #region Animal

        private static string IdadeFormatada(Animal animal)
        {
            var nascimento = animal.Nascimento;
            var hoje = DateTime.Today;
            int anos = hoje.Year - nascimento.Year;
            int meses = hoje.Month - nascimento.Month;

            if (hoje.Day < nascimento.Day)
            {
                meses--;
            }

            if (meses < 0)
            {
                anos--;
                meses += 12;
            }

            if (anos < 0) anos = 0; // Prevenção

            if (anos == 0) return $"{meses} meses";
            if (meses == 0) return $"{anos} anos";

            return $"{anos} anos e {meses} meses";
        }

        private static string RotuloDaFaseDeVida(Animal animal) => animal.FaseVida.GetDisplayName();

        private static string RotuloDaCondicaoCorporal(Animal animal) =>
            animal.CondicaoCorporal.HasValue
                ? CondicaoCorporalRotulo.Obter(animal.CondicaoCorporal.Value, animal.Especie)
                : "Não informado";

        private static List<TutorResumidoDto> TutoresDoAnimal(Animal animal) =>
            animal.TutoresAnimais?
                .Select(ta => ta.Tutor == null ? null! : ParaResumo(ta.Tutor))
                .ToList() ?? new List<TutorResumidoDto>();

        private static List<ConsultaDetalhadaDto> ConsultasDoAnimal(Animal animal) =>
            animal.Consultas?.Select(ParaDetalhe).ToList() ?? new List<ConsultaDetalhadaDto>();

        #endregion

        #region Consulta

        private static int IdDoTutorOuZero(Consulta consulta) => consulta.TutorId ?? 0;

        // Quem marcou a consulta
        private static string NomeDeQuemMarcou(Consulta consulta) => consulta.Usuario?.Nome ?? "Não informado";

        #endregion

        #region Dieta

        private static string RotuloDoTipoDeDieta(Dieta dieta) => dieta.TipoDieta.GetDisplayName();

        private static double QuantidadeDeAlimento(Dieta dieta) => dieta.QtdAlimento ?? 0;

        private static double NecessidadeEnergetica(Dieta dieta) => dieta.EnergiaMetabolizavel ?? 0;

        private static string NomeDaRacaoDaDieta(Dieta dieta) => dieta.Racao?.Nome ?? "Ração não informada";

        private static string RotuloDoStatus(Dieta dieta) => DietaMapper.RotuloDoStatus(dieta);

        private static string? RotuloDoNivelRestricao(Dieta dieta) => DietaMapper.RotuloDoNivelRestricao(dieta);

        private static int? SemanaAtual(Dieta dieta) => DietaMapper.SemanaAtual(dieta);

        private static double? FatorRestricao(Dieta dieta) => DietaMapper.FatorRestricao(dieta);

        private static double? PercentualPerdaSemanal(Dieta dieta) => DietaMapper.PercentualPerdaSemanal(dieta);

        private static double? PerdaSemanalKg(Dieta dieta) => DietaMapper.PerdaSemanalKg(dieta);

        private static List<GetRegistroPesoDto> RegistrosPorSemana(Dieta dieta) => DietaMapper.RegistrosPorSemana(dieta);

        #endregion
    }
}
