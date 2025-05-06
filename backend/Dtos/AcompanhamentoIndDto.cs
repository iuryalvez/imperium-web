using backend.Dtos.DietaPersonalizada;
using backend.Dtos.Relatorio;

namespace backend.Dtos.AcompanhamentoInd
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