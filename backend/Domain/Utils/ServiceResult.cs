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
        /// Tipo da falha (opcional), usado só pelo controller para escolher 404 ou 409 (ver ServiceResultHttpExtensions).
        /// Não vai na resposta JSON, então o formato que o frontend consome não muda.
        /// </summary>
        [JsonIgnore]
        public TipoFalha? Falha { get; set; }

        /// <summary>Falha de registro inexistente ou de outra clínica (404).</summary>
        public static ServiceResult NaoEncontrado(string mensagem) =>
            new() { Succeeded = false, Message = mensagem, Falha = TipoFalha.NaoEncontrado };

        /// <summary>Falha de conflito com dado existente: duplicado ou em uso (409).</summary>
        public static ServiceResult Conflito(string mensagem) =>
            new() { Succeeded = false, Message = mensagem, Falha = TipoFalha.Conflito };

        /// <summary>Falha de regra ou validação (400).</summary>
        public static ServiceResult Invalido(string mensagem) =>
            new() { Succeeded = false, Message = mensagem };
    }
}
