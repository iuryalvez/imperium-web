using backend.Domain.Utils.Paginacao;

namespace backend.Domain.GestaoClinica.DTOs
{
    /// <summary>
    /// Filtro da listagem de clínicas do sistema (admin). Busca por nome, nome fantasia ou CNPJ.
    /// Ordenação aceita: "dataCadastro" (padrão), "nomeClinica".
    /// </summary>
    public class ClinicaFiltro : FiltroBase
    {
        public bool? Ativa { get; set; }
    }
}
