using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.DTOs.Tutor
{
    /// <summary>
    /// Filtro da listagem de tutores da clínica. Busca por nome, CPF ou e-mail.
    /// Ordenação aceita: "dataCadastro" (padrão), "nome".
    /// </summary>
    public class TutorFiltro : FiltroBase
    {
        public bool? Ativo { get; set; }
    }
}
