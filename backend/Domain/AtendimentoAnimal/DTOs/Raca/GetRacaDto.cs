namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class GetRacaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty; // Returning string for API friendliness
        public float? PesoMinimoReferencia { get; set; }
        public float? PesoMaximoReferencia { get; set; }
    }
}