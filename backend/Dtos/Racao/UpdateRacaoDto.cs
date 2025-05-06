using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Racao
{
    public class UpdateRacaoDto
    {
        [Required(ErrorMessage = "A clínica vinculada é obrigatório.")]
        public int ClinicaId { get; set; }
        [Required(ErrorMessage = "O nome da ração é obrigatório.")]
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage = "A umidade da ração é obrigatória")]
        public float Umidade { get; set; }

        [Required(ErrorMessage = "A proteína bruta da ração é obrigatória.")]
        public float ProteinaBruta { get; set; } // unidade: utilizar % ou g
        
        [Required(ErrorMessage = "O extrato etéreo da ração é obrigatório.")]
        public float ExtratoEtereo { get; set; }

        [Required(ErrorMessage = "A matéria mineral da ração é obrigatória.")]
        public float MateriaMineral { get; set; }

        [Required(ErrorMessage = "A fibra bruta da ração é obrigatória.")]
        public float FibraBruta { get; set; }
        public float MateriaSeca { get; set; } // MS calculada ou determina
    }
}