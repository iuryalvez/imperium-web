using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.SaudeAnimal.DTOs.Racao;

public class AtualizarRacaoDto
{
    [Required(ErrorMessage = "O id da ração é obrigatório.")]
    public int Id { get; set; }
    [EnumDataType(typeof(EspecieAnimal), ErrorMessage = "A espécie é inválida.")]
    public EspecieAnimal? Especie { get; set; }
    public string? Nome { get; set; }
    public bool? IsPorcentagem { get; set; } // nulo mantém o valor atual (edição parcial)
    public double? Umidade { get; set; }
    public double? ProteinaBruta { get; set; }
    public double? ExtratoEtereo { get; set; }
    public double? MateriaMineral { get; set; }
    public double? FibraBruta { get; set; }
}