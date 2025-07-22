// DTO análogo para animais, para ser usado no GetTutoresClinicaDto
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal
{
    public class AnimalInfoSimplesDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public EspecieAnimal Especie { get; set; }
    }
}
