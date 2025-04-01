using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend.Interfaces
{
    public interface ITokenService
    {
        string GenerateTokenLogin(string usuarioId, string usuarioIp, string usuarioAgent);
        string GenerateTokenVinculo(string usuarioPerfilClinicaId, string usuarioIp, string usuarioAgent);
        string GenerateTokenVinculoAdmin(int clinicaId, Dictionary<string, bool> perfisAtivos, string usuarioIp, string usuarioAgent);
        public void SetCookieToken(HttpContext context, string token);
        Task RenovaToken(TokenValidatedContext context);
    }
}