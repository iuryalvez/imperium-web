using backend.Models;

namespace backend.Dtos.Animal
{
    public class CadastrarAnimalClinicaDto
    {
        public List<int>? TutorId { get; set; }
        public string Nome { get; set; }
        public EspecieAnimal Especie { get; set; } // 1-cao, 2-gato
        public DateTime Nascimento { get; set; }
        public DateTime DataCad { get; set; }
        public DateTime DataModificado { get; set; }
        // public int Idade { get; set; } // derivado da data de nascimento
        // public int FaseVida { get; set; } // derivado da idade
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