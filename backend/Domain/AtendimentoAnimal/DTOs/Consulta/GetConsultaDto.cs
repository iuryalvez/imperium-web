using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    public class GetConsultaDto
    {
        public int Id { get; set; }
        public DateTime DataConsulta { get; set; }

        // Informações do Animal
        public int AnimalId { get; set; }
        public string? NomeAnimal { get; set; }
        public EspecieAnimal EspecieAnimal { get; set; }
        public Sexo SexoAnimal { get; set; } 

        // Informações do Tutor
        public int TutorId { get; set; }
        public string? NomeTutor { get; set; }

        // Informações do Veterinário
        public int UsuarioId { get; set; }
        public string? NomeVeterinario { get; set; }

        // Status do Relatório
        public bool RelatorioPreenchido { get; set; }
    }
}