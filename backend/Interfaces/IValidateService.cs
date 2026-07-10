using System.Security.Claims;

namespace backend.Interfaces {
    public interface IValidateService {
        public ClaimsPrincipal ValidateTokenFromCookie(string token);
        public Task ValidateUserClaims(ClaimsPrincipal principal, HttpContext httpContext);
    }
}