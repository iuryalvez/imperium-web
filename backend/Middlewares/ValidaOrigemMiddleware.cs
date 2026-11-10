namespace backend.Middlewares
{
    /// <summary>
    /// Proteção contra CSRF. A sessão vai em cookie, que o navegador envia sozinho; por isso toda requisição que
    /// altera dados (POST, PUT, PATCH, DELETE) e traz o cabeçalho Origin precisa vir de uma origem permitida
    /// (a mesma lista do CORS). O navegador sempre envia Origin nesses métodos e a página não consegue forjá-lo.
    /// Requisição sem Origin (chamada fora de navegador) segue, porque não carrega o cookie de ninguém.
    /// </summary>
    public class ValidaOrigemMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _origensPermitidas;
        private readonly ILogger<ValidaOrigemMiddleware> _logger;

        public ValidaOrigemMiddleware(RequestDelegate next, HashSet<string> origensPermitidas, ILogger<ValidaOrigemMiddleware> logger)
        {
            _next = next;
            _origensPermitidas = origensPermitidas;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var metodo = context.Request.Method;
            var alteraDados = HttpMethods.IsPost(metodo) || HttpMethods.IsPut(metodo)
                           || HttpMethods.IsPatch(metodo) || HttpMethods.IsDelete(metodo);

            if (alteraDados)
            {
                var origem = context.Request.Headers.Origin.ToString();
                if (!string.IsNullOrEmpty(origem) && !_origensPermitidas.Contains(origem.TrimEnd('/')))
                {
                    _logger.LogWarning("Requisição {Metodo} {Caminho} recusada: origem {Origem} não permitida.",
                        metodo, context.Request.Path, origem);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\":\"Origem não permitida.\"}");
                    return;
                }
            }

            await _next(context);
        }
    }
}
