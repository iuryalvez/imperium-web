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
    }
}
