using System.Security.Claims;

namespace backend.Interfaces {
    public interface IValidateService {
        public Task ValidateUserClaims(ClaimsPrincipal principal, HttpContext httpContext);
    }
}
