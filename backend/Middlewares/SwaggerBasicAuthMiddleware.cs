using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
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
            var ehSwagger = context.Request.Path.StartsWithSegments("/api/docs") || context.Request.Path.StartsWithSegments("/swagger");
            if (!ehSwagger || CredenciaisValidas(context.Request.Headers.Authorization.ToString()))
            {
                await _next(context);
                return;
            }

            // Sem credenciais válidas (ou cabeçalho malformado): pede usuário e senha
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        private bool CredenciaisValidas(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return false;

            var usuarioConfigurado = _configuration["SwaggerAuth:Username"];
            var senhaConfigurada = _configuration["SwaggerAuth:Password"];
            if (string.IsNullOrEmpty(usuarioConfigurado) || string.IsNullOrEmpty(senhaConfigurada))
                return false;

            try
            {
                var parametro = AuthenticationHeaderValue.Parse(authHeader).Parameter ?? string.Empty;
                var credenciais = Encoding.UTF8.GetString(Convert.FromBase64String(parametro));
                var separador = credenciais.IndexOf(':');
                if (separador < 0) return false;

                // Comparação em tempo constante: o tempo de resposta não ajuda a adivinhar a senha
                var usuarioOk = Iguais(credenciais[..separador], usuarioConfigurado);
                var senhaOk = Iguais(credenciais[(separador + 1)..], senhaConfigurada);
                return usuarioOk & senhaOk;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool Iguais(string recebido, string esperado) =>
            CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(recebido), Encoding.UTF8.GetBytes(esperado));
    }
}
