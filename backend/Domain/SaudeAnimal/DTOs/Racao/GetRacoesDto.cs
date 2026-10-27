
namespace backend.Domain.SaudeAnimal.DTOs.Racao;
/// <summary>
/// DTO para representar os dados detalhados de uma ração, incluindo sua composição garantida, 
/// valores energéticos calculados e a distribuição dessa energia.
/// </summary>
public class GetRacoesDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty; // "Cao" ou "Gato"

    // --- Composição Garantida (Valores de Entrada) ---
    
    /// <summary>
    /// Indica se os valores de composição (Proteína, Fibra, etc.) estão em porcentagem (se não estão, convertemos).
    /// </summary>
    public bool IsPorcentagem { get; set; }
    public double Umidade { get; set; }
    public double ProteinaBruta { get; set; }
    public double ExtratoEtereo { get; set; }
    public double MateriaMineral { get; set; }
    public double FibraBruta { get; set; }
    
    /// <summary>
    /// Matéria Seca calculada (100 - Umidade).
    /// </summary>
    public double? MateriaSeca { get; set; }

    // --- Valores Calculados Principais ---

    /// <summary>
    /// Carboidratos (calculado).
    /// </summary>
    public double? ExtrativoNaoNitrogenado { get; set; }
    public double? EnergiaBruta { get; set; }
    public double? FibraDieteticaTotal { get; set; }
    public double? CoeficienteDigestibilidadeEnergia { get; set; }
    public double? EnergiaDigestivel { get; set; }
    
    /// <summary>
    /// Valor crucial para cálculos de dieta (kcal/g).
    /// </summary>
    public double? EnergiaMetabolizavel { get; set; }

    // --- Distribuição Energética (ex: kcal/100g) ---

    /// <summary>
    /// Energia proveniente da Proteína (kcal/100g).
    /// </summary>
    public double? EnergiaProteina { get; set; }
    
    /// <summary>
    /// Energia proveniente do Extrato Etéreo (gordura) (kcal/100g).
    /// </summary>
    public double? EnergiaExtratoEtereo { get; set; }
    
    /// <summary>
    /// Energia proveniente do Extrativo Não Nitrogenado (carboidratos) (kcal/100g).
    /// </summary>
    public double? EnergiaExtrativoNaoNitrogenado { get; set; }
    
    /// <summary>
    /// Soma das energias dos macronutrientes (kcal/100g).
    /// </summary>
    public double? EnergiaTotalEstimativa { get; set; }
}