using backend.Dtos.Consulta;
using backend.Dtos.DietaPersonalizada;

namespace backend.Dtos.AcompanhamentoInd
{
    public class AcompanhamentoIndDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public virtual ICollection<DietaPersonalizadaDto>? DietasPersonalizadas { get; set; }
        public virtual ICollection<ConsultaDto>? Consultas { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
    }
}