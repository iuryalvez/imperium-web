namespace backend.Interfaces
{
    public interface ITurnstileService
    {
        Task<bool> ValidarTokenAsync(string token, string? remoteIp);
    }
}