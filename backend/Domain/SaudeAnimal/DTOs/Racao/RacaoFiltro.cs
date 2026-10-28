using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.SaudeAnimal.DTOs.Racao;

/// <summary>
/// Filtro das listagens de rações. Ordenação aceita: "dataCadastro" (padrão), "nome", "especie".
/// </summary>
public class RacaoFiltro : FiltroBase
{
    public bool? Ativo { get; set; }
    public EspecieAnimal? Especie { get; set; }

    /// <summary>Clínica: "sistema" (só do sistema), "clinica" (só próprias) ou vazio (todas).</summary>
    public string? Origem { get; set; }

    /// <summary>Clínica: filtra rações do sistema ocultas (true) ou visíveis (false) para a clínica.</summary>
    public bool? Oculta { get; set; }
}
