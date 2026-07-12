using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend.Domain.Auth.Interfaces
{
    public interface ITokenService
    {
        string GenerateTokenLogin(string usuarioId, string usuarioIp, string usuarioAgent, bool rememberMe = false);
        string GenerateTokenVinculo(string usuarioPerfilClinicaId, string usuarioIp, string usuarioAgent);
        string GenerateTokenVinculoAdmin(int clinicaId, List<string> perfisAtivos, string usuarioIp, string usuarioAgent);
        public void SetCookieToken(HttpContext context, string token, bool rememberMe = false);
        Task RenovaToken(TokenValidatedContext context);
    }
}