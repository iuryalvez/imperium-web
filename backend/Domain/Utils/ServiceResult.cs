using System.Text.Json.Serialization;

namespace backend.Domain.Utils
{
    // Classe auxiliar para o resultado do serviço
    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// Tipo da falha (opcional), usado só pelo controller para escolher 404 ou 409.
        /// Não vai na resposta JSON, então o formato que o frontend consome não muda.
        /// </summary>
        [JsonIgnore]
        public TipoFalha? Falha { get; set; }
    }
}
