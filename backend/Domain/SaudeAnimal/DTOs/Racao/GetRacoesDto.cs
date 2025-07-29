public class GetRacoesDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public float Umidade { get; set; }
    public float ProteinaBruta { get; set; }
    public float ExtratoEtereo { get; set; }
    public float MateriaMineral { get; set; }
    public float FibraBruta { get; set; }
    public float MateriaSeca { get; set; }
}