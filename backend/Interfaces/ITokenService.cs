using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend.Interfaces
{
    public interface ITokenService
    {
        string GenerateTokenVinculo(string usuarioPerfilClinicaId, string usuarioIp, string usuarioAgent);
        string GenerateTokenLogin(string usuarioId, string usuarioIp, string usuarioAgent);
        Task RenovaToken(TokenValidatedContext context);
    }
}