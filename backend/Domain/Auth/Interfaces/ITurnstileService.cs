namespace backend.Domain.Auth.Interfaces
{
    public interface ITurnstileService
    {
        Task<bool> ValidarTokenAsync(string token, string? remoteIp);
    }
}