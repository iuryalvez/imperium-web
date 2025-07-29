using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;

public class AtualizarRacaoDto
{
    [Required]
    public int Id { get; set; }
    public EspecieAnimal? Especie { get; set; }
    public string? Nome { get; set; }
    public float? Umidade { get; set; }
    public float? ProteinaBruta { get; set; }
    public float? ExtratoEtereo { get; set; }
    public float? MateriaMineral { get; set; }
    public float? FibraBruta { get; set; }
}