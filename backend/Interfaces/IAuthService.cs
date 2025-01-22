using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend.Interfaces {
    public interface IAuthService {
        public Task<bool> VerificarPermissaoClinica(int clinicaId);
        public Task<bool> ValidaUsuario(string token);
        public ClaimsPrincipal ValidateTokenFromCookie(string token);
        public Task ValidaContext(TokenValidatedContext context);
    }
}