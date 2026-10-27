namespace backend.Domain.Utils
{
    // Classe auxiliar para o resultado do serviço
    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
