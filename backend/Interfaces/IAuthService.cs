using System.Security.Claims;

namespace backend.Interfaces {
    public interface IAuthService {
        public ClaimsPrincipal ValidateTokenFromCookie(string token);
        public Task ValidateUserClaims(ClaimsPrincipal principal, HttpContext httpContext);
    }
}