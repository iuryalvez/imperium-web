using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.DTOs.Animal
{
    /// <summary>
    /// Filtro da listagem de animais da clínica. Busca por nome do animal ou do tutor.
    /// Ordenação aceita: "dataCadastro" (padrão), "nome", "especie".
    /// </summary>
    public class AnimalFiltro : FiltroBase
    {
        public bool? Ativo { get; set; }
        public EspecieAnimal? Especie { get; set; }
        public int? TutorId { get; set; }
    }
}
