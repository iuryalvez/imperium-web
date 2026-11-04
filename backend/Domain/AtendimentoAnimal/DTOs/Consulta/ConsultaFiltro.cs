using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    /// <summary>
    /// Filtro da listagem de consultas da clínica. Busca por nome do animal ou do tutor.
    /// Ordenação aceita: "dataConsulta" (padrão, mais recentes primeiro), "nomeAnimal".
    /// </summary>
    public class ConsultaFiltro : FiltroBase
    {
        public int? AnimalId { get; set; }
        public int? TutorId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        /// <summary>Pendente, EmAndamento, Concluida ou Cancelada (status calculado, ver StatusConsulta).</summary>
        public StatusConsulta? Status { get; set; }

        /// <summary>Só as consultas que o usuário logado marcou (Consulta.UsuarioId). Usado no Meu Painel do SECRETARIO.</summary>
        public bool? MarcadasPorMim { get; set; }

        /// <summary>Só as consultas cuja avaliação o usuário logado preencheu. Usado no Meu Painel do VETERINARIO.</summary>
        public bool? AtendidasPorMim { get; set; }
    }
}
