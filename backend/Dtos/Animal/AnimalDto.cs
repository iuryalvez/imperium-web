

using backend.Dtos.TutorAnimalClinica;

namespace backend.Dtos.Animal
{
    public class AnimalDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public byte Especie { get; set; } // 1-cao, 2-gato
        public DateTime Nascimento { get; set; } 
        public DateTime DataCad { get; set; }
        public DateTime DataCadModificado { get; set; }
        // public int Idade { get; set; } // derivado da data de nascimento
        // public int FaseVida { get; set; } // derivado da idade
        public int Sexo { get; set; } // 1-macho, 2-fêmea
        public bool Castrado { get; set; }
        public float Peso { get; set; }
        public int CondicaoCorporal { get; set; } // valores: 1-10
        public int GrauAtividade { get; set; } // 1-inativo, 2-moderado, 3-ativo
        public bool Gestacao { get; set; }
        public int Lactacao { get; set; } // numero de filhotes
        public int SemanaLactacao { get; set; }
        public int SemanaGestacao { get; set; }
    }
}