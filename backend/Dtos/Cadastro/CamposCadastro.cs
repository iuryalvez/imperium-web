namespace backend.Dtos {
    public class CamposCadastroDto
    {
        public string? Name { get; set; }         // Nome do campo (ex: "email")
        public string? Label { get; set; }        // Rótulo exibido (ex: "E-mail")
        public string? Type { get; set; }         // Tipo do campo (ex: "text", "email", "password", "select")
        public string? Placeholder { get; set; }  // Placeholder opcional (ex: "Digite seu e-mail")
        public List<OpcaoDto>? Options { get; set; }  // Se for um select, lista de opções disponíveis
        public List<RuleDto>? Rules { get; set; }  // Regras de validação (ex: campo obrigatório)
    }
}
