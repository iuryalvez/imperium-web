using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;

public class AtualizarRacaoDto
{
    [Required]
    public int Id { get; set; }
    public EspecieAnimal? Especie { get; set; }
    public string? Nome { get; set; }
    public bool IsPorcentagem { get; set; }
    public double? Umidade { get; set; }
    public double? ProteinaBruta { get; set; }
    public double? ExtratoEtereo { get; set; }
    public double? MateriaMineral { get; set; }
    public double? FibraBruta { get; set; }
}