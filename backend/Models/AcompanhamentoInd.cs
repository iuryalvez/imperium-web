namespace backend.Models {
    public class AcompanhamentoInd
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public virtual ICollection<DietaPersonalizada>? DietasPersonalizadas { get; set; }
        public virtual ICollection<Consulta>? Consultas { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
    }
}
