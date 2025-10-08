using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal
{
    public class GetAnimaisClinicaDto
    {
        public int Id { get; set; }
        public int? RacaId { get; set; }
        public string? RacaNome { get; set; }
        public List<int> TutorIds { get; set; }
        public int AcompanhamentoIndividualizadoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public EspecieAnimal Especie { get; set; } // 1-cao, 2-gato
        public DateTime Nascimento { get; set; }
        public Sexo Sexo { get; set; } // 1-macho, 2-fêmea
        public bool IsCastrado { get; set; }
        public double? Peso { get; set; }
        public CondicaoCorporal? CondicaoCorporal { get; set; }
        public GrauAtividade? GrauAtividade { get; set; }
        public bool? Gestacao { get; set; }
        public int? Lactacao { get; set; }
        public int? SemanaLactacao { get; set; }
        public int? SemanaGestacao { get; set; }
        public List<TutorInfoSimplesDto>? Tutores { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
