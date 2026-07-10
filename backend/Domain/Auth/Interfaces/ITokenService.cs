using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend.Domain.Auth.Interfaces
{
    public interface ITokenService
    {
        string GenerateTokenLogin(string usuarioId, string usuarioIp, string usuarioAgent);
        string GenerateTokenVinculo(string usuarioPerfilClinicaId, string usuarioIp, string usuarioAgent);
        string GenerateTokenVinculoAdmin(int clinicaId, List<string> perfisAtivos, string usuarioIp, string usuarioAgent);
        public void SetCookieToken(HttpContext context, string token);
        Task RenovaToken(TokenValidatedContext context);
    }
}