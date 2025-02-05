using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces {
    public interface IAuthService {
        public ClaimsPrincipal ValidateTokenFromCookie(string token);
        public Task ValidateUserClaims(ClaimsPrincipal principal, HttpContext httpContext);
    }
}