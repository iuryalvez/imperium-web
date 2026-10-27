using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.AtendimentoAnimal.DTOs.Animal
{
    public class AnimalInfoSimplesDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public EspecieAnimal Especie { get; set; }
    }
}
