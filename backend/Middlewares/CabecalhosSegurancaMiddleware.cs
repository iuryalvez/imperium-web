namespace backend.Middlewares
{
    /// <summary>
    /// Cabeçalhos de segurança em todas as respostas da API (os mesmos do nginx.conf do frontend):
    /// nosniff (o navegador respeita o tipo informado), X-Frame-Options (não abre em iframe de outro site),
    /// Referrer-Policy (não vaza caminho nem query para outros sites) e HSTS (só HTTPS).
    /// O HSTS fica de fora em desenvolvimento, que roda em http://localhost.
    /// </summary>
    public class CabecalhosSegurancaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _enviarHsts;

        public CabecalhosSegurancaMiddleware(RequestDelegate next, IHostEnvironment ambiente)
        {
            _next = next;
            _enviarHsts = !ambiente.IsDevelopment();
        }

        public Task InvokeAsync(HttpContext context)
        {
            var cabecalhos = context.Response.Headers;
            cabecalhos["X-Content-Type-Options"] = "nosniff";
            cabecalhos["X-Frame-Options"] = "DENY";
            cabecalhos["Referrer-Policy"] = "strict-origin-when-cross-origin";
            if (_enviarHsts) cabecalhos["Strict-Transport-Security"] = "max-age=31536000";

            return _next(context);
        }
    }
}
