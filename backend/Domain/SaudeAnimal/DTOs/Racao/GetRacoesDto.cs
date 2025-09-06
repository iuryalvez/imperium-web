public class GetRacoesDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public bool IsPorcentagem { get; set; }
    public double Umidade { get; set; }
    public double ProteinaBruta { get; set; }
    public double ExtratoEtereo { get; set; }
    public double MateriaMineral { get; set; }
    public double FibraBruta { get; set; }
    public double MateriaSeca { get; set; }
}