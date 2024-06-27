using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public byte Especie { get; set; } // 0-cao, 1-gato
        public DateTime Nascimento { get; set; }
        public DateTime Data_cad { get; set; } = DateTime.Now;
        public DateTime Data_cad_modificado { get; set; } = DateTime.Now;
        public byte Idade { get; set; } // derivado
        public bool Filhote { get; set; } // derivado
        public bool Adulto { get; set; } // derivado
        public bool Idoso { get; set; } // derivado
        public byte Sexo { get; set; } // 0-macho, 1-fêmea
        public bool Castrado { get; set; }
        public float Peso { get; set; }
        public byte Condicao_corporal { get; set; } // valores: 1-10
        public byte Grau_atividade { get; set; } // 0-inativo, 1-moderado, 2-ativo
        public bool Gestacao { get; set; }
        public byte Lactacao { get; set; } // numero de filhotes
        public byte Semana_lactacao { get; set; }
        public byte Semana_gestacao { get; set; }
        public int TutorId { get; set; } // chave estrangeira do tutor
        public Tutor? Tutor { get; set; } // representar a relação de muitos-para-um
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }

    }
}