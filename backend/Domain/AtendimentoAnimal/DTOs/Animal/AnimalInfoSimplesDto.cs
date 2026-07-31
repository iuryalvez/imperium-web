using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal
{
    public class AnimalInfoSimplesDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public EspecieAnimal Especie { get; set; }
    }
}
