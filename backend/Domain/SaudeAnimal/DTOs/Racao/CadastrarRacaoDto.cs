using System.ComponentModel.DataAnnotations;
using backend.Domain.AtendimentoAnimal.Enums;

namespace backend.Domain.SaudeAnimal.DTOs.Racao;

public class CadastrarRacaoDto
{
    [Required(ErrorMessage = "O nome da ração é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A espécie é obrigatória.")]
    [EnumDataType(typeof(EspecieAnimal), ErrorMessage = "A espécie é inválida.")]
    public EspecieAnimal Especie { get; set; }

    [Required(ErrorMessage = "Informe se os valores estão em porcentagem.")]
    public bool IsPorcentagem { get; set; }

    [Required(ErrorMessage = "A umidade é obrigatória.")]
    public double Umidade { get; set; }
    [Required(ErrorMessage = "A proteína bruta é obrigatória.")]
    public double ProteinaBruta { get; set; }
    [Required(ErrorMessage = "O extrato etéreo é obrigatório.")]
    public double ExtratoEtereo { get; set; }
    [Required(ErrorMessage = "A matéria mineral é obrigatória.")]
    public double MateriaMineral { get; set; }
    [Required(ErrorMessage = "A fibra bruta é obrigatória.")]
    public double FibraBruta { get; set; }
}