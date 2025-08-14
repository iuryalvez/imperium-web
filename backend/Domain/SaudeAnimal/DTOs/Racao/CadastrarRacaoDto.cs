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
    public float Umidade { get; set; }
    [Required]
    public float ProteinaBruta { get; set; }
    [Required]
    public float ExtratoEtereo { get; set; }
    [Required]
    public float MateriaMineral { get; set; }
    [Required]
    public float FibraBruta { get; set; }
}