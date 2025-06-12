using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal
{
    public class EditarAnimalClinicaDto
    {
        public int Id { get; set; }
        public List<int>? TutorId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public EspecieAnimal Especie { get; set; } // 1-cao, 2-gato
        public DateTime Nascimento { get; set; }
        public Sexo Sexo { get; set; } // 1-macho, 2-fêmea
        public bool Castrado { get; set; }
        public float Peso { get; set; }
        public CondicaoCorporal CondicaoCorporal { get; set; } // valores: 1-10
        public GrauAtividade GrauAtividade { get; set; } // 1-inativo, 2-moderado, 3-ativo
        public bool Gestacao { get; set; }
        public int Lactacao { get; set; } // numero de filhotes
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
    }
}