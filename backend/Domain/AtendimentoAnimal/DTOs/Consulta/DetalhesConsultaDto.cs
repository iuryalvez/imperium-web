namespace backend.Domain.AtendimentoAnimal.DTOs.Consulta
{
    public class DetalhesConsultaDto
    {
        public int Id { get; set; }
        public DateTime DataConsulta { get; set; }

        public int AnimalId { get; set; }
        public string? NomeAnimal { get; set; }

        public int TutorId { get; set; }
        public string? NomeTutor { get; set; }
        public string? CpfTutor { get; set; }
        public string? EmailTutor { get; set; }
        public string? TelefoneTutor { get; set; }

        public int UsuarioId { get; set; }
        public string? NomeVeterinario { get; set; }

        public int? AvaliacaoId { get; set; } // ID do relatório, se existir

        // Futuramente, podemos incluir aqui o objeto completo do relatório
        // public GetAvaliacaoDto Avaliacao { get; set; }
    }
}