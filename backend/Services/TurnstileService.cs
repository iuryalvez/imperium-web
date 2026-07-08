using System.Text.Json.Serialization;
using backend.Interfaces;

namespace backend.Services;

public class TurnstileService : ITurnstileService
{
    private const string VerifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TurnstileService> _logger;

    public TurnstileService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TurnstileService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> ValidarTokenAsync(string token, string? remoteIp)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Turnstile: token vazio na requisicao");
            return false;
        }

        var secret = _configuration["TURNSTILE_SECRET_KEY"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            _logger.LogError("Turnstile: TURNSTILE_SECRET_KEY nao configurada no .env");
            return false;
        }

        var form = new Dictionary<string, string>
        {
            ["secret"] = secret,
            ["response"] = token
        };
        if (!string.IsNullOrWhiteSpace(remoteIp))
            form["remoteip"] = remoteIp;

        try
        {
            var response = await _httpClient.PostAsync(VerifyUrl, new FormUrlEncodedContent(form));
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TurnstileVerifyResponse>();

            if (result is null || !result.Success)
            {
                _logger.LogWarning(
                    "Turnstile: validacao falhou. ErrorCodes: {Codes}",
                    string.Join(", ", result?.ErrorCodes ?? Array.Empty<string>()));
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Turnstile: erro ao chamar siteverify");
            return false;
        }
    }

    private class TurnstileVerifyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}