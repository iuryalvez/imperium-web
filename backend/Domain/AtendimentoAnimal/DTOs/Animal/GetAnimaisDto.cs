using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal
{
    public class GetAnimaisDto
    {
        public int Id { get; set; }
        public int TutorId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public EspecieAnimal Especie { get; set; } // 1-cao, 2-gato
        public Sexo Sexo { get; set; } // 1-macho, 2-fêmea
        public String? NomeTutor { get; set; }
    }
}