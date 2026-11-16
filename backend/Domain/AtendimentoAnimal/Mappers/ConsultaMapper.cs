using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.AtendimentoAnimal.Mappers
{
    /// <summary>
    /// Conversões entre a entidade Consulta e os DTOs. O código é gerado na compilação pelo Mapperly.
    /// A edição substitui animal, tutor e data pelo que veio no DTO (tutor nulo remove o tutor).
    /// </summary>
    [Mapper]
    public static partial class ConsultaMapper
    {
        /// <summary>Cadastro. Clínica e usuário vêm do token; o cancelamento só muda pelas rotas cancelar/reabrir.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        public static partial Consulta ParaEntidade(UpsertConsultaDto dto);

        /// <summary>Edição: animal, tutor e data. Id e cancelamento não vêm do DTO.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        public static partial void Aplicar(UpsertConsultaDto dto, [MappingTarget] Consulta consulta);

        /// <summary>Item da lista de consultas (precisa de Animal.Raca, Animal.Acompanhamento, Tutor, Usuario e Avaliacao.Dieta carregados).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(nameof(@Consulta.Tutor.Nome), nameof(GetConsultaDto.NomeTutor))]
        [MapProperty(nameof(@Consulta.Usuario.Nome), nameof(GetConsultaDto.NomeUsuario))]
        [MapProperty(nameof(@Consulta.Animal.Nome), nameof(GetConsultaDto.NomeAnimal))]
        [MapProperty(nameof(@Consulta.Animal.Raca.Nome), nameof(GetConsultaDto.NomeRaca))]
        [MapProperty(nameof(@Consulta.CanceladaPorUsuario.Nome), nameof(GetConsultaDto.NomeCanceladaPor))]
        [MapPropertyFromSource(nameof(GetConsultaDto.TutorId), Use = nameof(IdDoTutorOuZero))]
        [MapPropertyFromSource(nameof(GetConsultaDto.EspecieAnimal), Use = nameof(EspecieDoAnimal))]
        [MapPropertyFromSource(nameof(GetConsultaDto.SexoAnimal), Use = nameof(SexoDoAnimal))]
        [MapPropertyFromSource(nameof(GetConsultaDto.AvaliacaoPreenchida), Use = nameof(TemAvaliacao))]
        [MapPropertyFromSource(nameof(GetConsultaDto.DietaId), Use = nameof(IdDaDieta))]
        [MapPropertyFromSource(nameof(GetConsultaDto.AcompanhamentoId), Use = nameof(IdDoAcompanhamento))]
        [MapPropertyFromSource(nameof(GetConsultaDto.PesoMinimoRaca), Use = nameof(PesoMinimoDaRaca))]
        [MapPropertyFromSource(nameof(GetConsultaDto.PesoMaximoRaca), Use = nameof(PesoMaximoDaRaca))]
        public static partial GetConsultaDto ParaDto(Consulta consulta);

        /// <summary>Detalhes de uma consulta (precisa de Animal, Tutor, Usuario e Avaliacao carregados).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapProperty(nameof(@Consulta.Animal.Nome), nameof(DetalhesConsultaDto.NomeAnimal))]
        [MapProperty(nameof(@Consulta.Tutor.Nome), nameof(DetalhesConsultaDto.NomeTutor))]
        [MapProperty(nameof(@Consulta.Tutor.Cpf), nameof(DetalhesConsultaDto.CpfTutor))]
        [MapProperty(nameof(@Consulta.Tutor.Email), nameof(DetalhesConsultaDto.EmailTutor))]
        [MapProperty(nameof(@Consulta.Tutor.Telefone), nameof(DetalhesConsultaDto.TelefoneTutor))]
        [MapProperty(nameof(@Consulta.Usuario.Nome), nameof(DetalhesConsultaDto.NomeVeterinario))]
        [MapPropertyFromSource(nameof(DetalhesConsultaDto.AvaliacaoId), Use = nameof(IdDaAvaliacao))]
        public static partial DetalhesConsultaDto ParaDetalhes(Consulta consulta);

        private static int IdDoTutorOuZero(Consulta consulta) => consulta.TutorId ?? 0;

        private static EspecieAnimal EspecieDoAnimal(Consulta consulta) => consulta.Animal?.Especie ?? default;

        private static Sexo SexoDoAnimal(Consulta consulta) => consulta.Animal?.Sexo ?? default;

        // Se a avaliação existe, está preenchida
        private static bool TemAvaliacao(Consulta consulta) => consulta.Avaliacao != null;

        private static int? IdDaAvaliacao(Consulta consulta) => consulta.Avaliacao?.Id;

        private static int? IdDaDieta(Consulta consulta) => consulta.Avaliacao?.Dieta?.Id;

        private static int IdDoAcompanhamento(Consulta consulta) => consulta.Animal?.Acompanhamento?.Id ?? 0;

        // Referência de peso da raça conforme o sexo do animal
        private static double? PesoMinimoDaRaca(Consulta consulta)
        {
            var raca = consulta.Animal?.Raca;
            if (raca == null) return null;
            return consulta.Animal!.Sexo == Sexo.Macho ? raca.PesoMinimoReferenciaMacho : raca.PesoMinimoReferenciaFemea;
        }

        private static double? PesoMaximoDaRaca(Consulta consulta)
        {
            var raca = consulta.Animal?.Raca;
            if (raca == null) return null;
            return consulta.Animal!.Sexo == Sexo.Macho ? raca.PesoMaximoReferenciaMacho : raca.PesoMaximoReferenciaFemea;
        }
    }
}
