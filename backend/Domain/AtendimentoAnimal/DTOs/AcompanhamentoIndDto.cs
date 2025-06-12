using backend.Domain.SaudeAnimal.DTOs;

namespace backend.Domain.AtendimentoAnimal.DTOs
{
    public class AcompanhamentoIndDto
    {
        public int AnimalId { get; set; }
        public virtual ICollection<DietaPersonalizadaDto>? DietasPersonalizadas { get; set; }
        public virtual ICollection<RelatorioDto>? Relatorios { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}