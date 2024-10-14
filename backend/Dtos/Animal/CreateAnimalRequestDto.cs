using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.TutorAnimalClinica;

namespace backend.Dtos.Animal
{
    public class CreateAnimalRequestDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public byte Especie { get; set; } // 1-cao, 2-gato
        // public DateTime Nascimento { get; set; } 
        public DateTime DataCad { get; set; } = DateTime.UtcNow;
        // public DateTime DataCadModificado { get; set; } = DateTime.UtcNow;
        // public int Idade { get; set; } // derivado da data de nascimento
        // public int FaseVida { get; set; } // derivado da idade
        public int Sexo { get; set; } // 1-macho, 2-fêmea
        // public bool Castrado { get; set; }
        // public float Peso { get; set; }
        // public int CondicaoCorporal { get; set; } // valores: 1-10
        // public int GrauAtividade { get; set; } // 1-inativo, 2-moderado, 3-ativo
        // public bool Gestacao { get; set; }
        // public int Lactacao { get; set; } // numero de filhotes
        // public int SemanaLactacao { get; set; }
        // public int SemanaGestacao { get; set; }
        // public AcompanhamentoInd? AcompanhamentoInd { get; set; } // referência de navegação com o dependente
        public virtual ICollection<TutorAnimalClinicaDto> TutoresAnimaisClinicas { get; set; } = new List<TutorAnimalClinicaDto>(); // (1-N)
    }
}