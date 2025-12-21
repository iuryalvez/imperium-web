using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace backend.Middlewares
{
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if request is for swagger endpoint
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Get credentials from header
                    var header = AuthenticationHeaderValue.Parse(authHeader);
                    var inBytes = Convert.FromBase64String(header.Parameter);
                    var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];

                    // Read credentials from appsettings
                    var configUsername = _configuration["SwaggerAuth:Username"];
                    var configPassword = _configuration["SwaggerAuth:Password"];

                    // Validate credentials
                    if (username.Equals(configUsername) && password.Equals(configPassword))
                    {
                        await _next(context);
                        return;
                    }
                }

                // Return 401 Unauthorized and request credentials
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next(context);
            }
        }
    }
}