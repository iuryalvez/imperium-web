using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Entities;

public class CadastrarRacaoDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public EspecieAnimal Especie { get; set; }

    [Required]
    public bool IsPorcentagem { get; set; }

    [Required]
    public double Umidade { get; set; }
    [Required]
    public double ProteinaBruta { get; set; }
    [Required]
    public double ExtratoEtereo { get; set; }
    [Required]
    public double MateriaMineral { get; set; }
    [Required]
    public double FibraBruta { get; set; }
}