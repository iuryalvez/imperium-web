using System.Net;

namespace backend.Domain.Auth.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// IP do cliente. Atrás do Nginx (https-server) vem em X-Real-IP, que o proxy sempre sobrescreve com o
        /// endereço real da conexão. O X-Forwarded-For não é usado: o primeiro item dele é enviado pelo próprio
        /// cliente e pode ser falsificado. Sem proxy (desenvolvimento local), vale o IP da conexão.
        /// </summary>
        public static string GetClienteIp(this HttpContext context)
        {
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIp) && IPAddress.TryParse(realIp.Trim(), out var ip))
                return ip.ToString();

            return context.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";
        }
    }
}
