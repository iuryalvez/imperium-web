namespace backend.Models {
    public class AcompanhamentoInd
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public int DietaPersonalizadaId { get; set; }
        public DietaPersonalizada? DietaPersonalizada { get; set; }
        public int ConsultaId { get; set; }
        public Consulta? Consulta { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
    }
}
