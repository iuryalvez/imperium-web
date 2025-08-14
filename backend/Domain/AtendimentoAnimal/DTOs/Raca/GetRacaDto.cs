namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class GetRacaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty; // Returning string for API friendliness
        public float? PesoMinimoReferenciaFemea { get; set; }
        public float? PesoMaximoReferenciaFemea { get; set; }
        public float? PesoMinimoReferenciaMacho { get; set; }
        public float? PesoMaximoReferenciaMacho { get; set; }
    }
}