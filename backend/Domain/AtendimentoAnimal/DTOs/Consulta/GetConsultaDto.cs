using backend.Domain.AtendimentoAnimal.Enums;

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
        public int AcompanhamentoId { get; set; }

        // Referências da Raça
        public string? NomeRaca { get; set; }
        public double? PesoMinimoRaca { get; set; }
        public double? PesoMaximoRaca { get; set; }

        // Informações do Tutor
        public int TutorId { get; set; }
        public string? NomeTutor { get; set; }

        // Informações do Veterinário
        public int UsuarioId { get; set; }
        public string? NomeUsuario { get; set; }

        // Status do Relatório e Dieta
        public bool RelatorioPreenchido { get; set; }
        public int? DietaId { get; set; }
    }
}