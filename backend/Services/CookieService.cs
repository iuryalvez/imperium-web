using backend.Interfaces;

namespace backend.Services {
    public class CookieService : ICookieService {
        private readonly IConfiguration _configuration;

        public CookieService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void SetCookieToken(HttpContext context, string token)
        {
            context.Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Caso esteja em produção com HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:ExpiresInMinutes"]))
            });
        }
    }
}