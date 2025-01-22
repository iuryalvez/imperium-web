using System.Security.Claims;

namespace backend.Interfaces {
    public interface IAuthService {
        public Task<bool> VerificarPermissaoClinica(int clinicaId);
        public Task<bool> ValidaUsuario(string token);
        public ClaimsPrincipal ValidateTokenFromCookie(string token);
        public Task ValidateUserClaims(ClaimsPrincipal principal, HttpContext httpContext);
    }
}