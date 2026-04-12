namespace backend.Domain.SaudeAnimal.Entities
{
    // Classe que será mapeada como JSON no banco de dados
    public class ParametrosFormula
    {
        public Dictionary<string, double> Constantes { get; set; } = new();
        public List<double> FatoresSemana { get; set; } = new();
        public Dictionary<string, double> FatoresFilhotesGatos { get; set; } = new();
        
        // Específico para regras condicionais de filhotes (Gatos)
        public double? FatorFilhoteBaixo { get; set; } // < 3
        public double? FatorFilhoteMedio { get; set; } // 3 ou 4
        public double? FatorFilhoteAlto { get; set; }  // > 4
    }
}