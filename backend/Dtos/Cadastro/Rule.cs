namespace backend.Dtos
{
    public class RuleDto
    {
        public string? Type { get; set; }   // Ex: "required", "email", "minLength"
        public string? Message { get; set; }  // Ex: "O campo é obrigatório."
    }
}