using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    /// <summary>
    /// Filtro das listagens de raças. Ordenação aceita: "dataCadastro" (padrão), "nome", "especie".
    /// </summary>
    public class RacaFiltro : FiltroBase
    {
        public bool? Ativo { get; set; }
        public EspecieAnimal? Especie { get; set; }

        /// <summary>Clínica: "sistema" (só do sistema), "clinica" (só próprias) ou vazio (todas).</summary>
        public string? Origem { get; set; }

        /// <summary>Clínica: filtra raças do sistema ocultas (true) ou visíveis (false) para a clínica.</summary>
        public bool? Oculta { get; set; }
    }
}
