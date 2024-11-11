using backend.Dtos.Consulta;
using backend.Dtos.DietaPersonalizada;
using backend.Dtos.Relatorio;

namespace backend.Dtos.AcompanhamentoInd
{
    public class AcompanhamentoIndDto
    {
        public int AnimalId { get; set; }
        public virtual ICollection<DietaPersonalizadaDto>? DietasPersonalizadas { get; set; }
        public virtual ICollection<RelatorioDto>? Relatorios { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
    }
}