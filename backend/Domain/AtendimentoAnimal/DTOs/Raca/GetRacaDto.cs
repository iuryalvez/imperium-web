namespace backend.Domain.AtendimentoAnimal.DTOs.Raca
{
    public class GetRacaDto
    {
        public int Id { get; set; }
        public int? ClinicaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public double? PesoMinimoReferenciaFemea { get; set; }
        public double? PesoMaximoReferenciaFemea { get; set; }
        public double? PesoMinimoReferenciaMacho { get; set; }
        public double? PesoMaximoReferenciaMacho { get; set; }
    }
}