using System.Collections.Concurrent;
using backend.Domain.Auth.Interfaces;

namespace backend.Tests.Infra
{
    /// <summary>
    /// Captcha de teste: não chama a Cloudflare. Todo token passa, menos <see cref="TokenInvalido"/>.
    /// </summary>
    public class TurnstileFalso : ITurnstileService
    {
        public const string TokenValido = "captcha-de-teste";
        public const string TokenInvalido = "captcha-invalido";

        public Task<bool> ValidarTokenAsync(string token, string? remoteIp) =>
            Task.FromResult(!string.IsNullOrWhiteSpace(token) && token != TokenInvalido);
    }

    public record EmailEnviado(string Para, string Assunto, string Corpo);

    /// <summary>E-mail de teste: nada é enviado; as mensagens ficam guardadas para o teste conferir.</summary>
    public class EmailFalso : IEmailService
    {
        private readonly ConcurrentQueue<EmailEnviado> _enviados = new();

        public bool Configurado => true;

        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            _enviados.Enqueue(new EmailEnviado(toEmail, subject, body));
            return Task.CompletedTask;
        }

        public List<EmailEnviado> Para(string email) => _enviados.Where(e => e.Para == email).ToList();

        /// <summary>O envio é em segundo plano: espera (até 5 s) o e-mail com o assunto chegar.</summary>
        public async Task<EmailEnviado?> EsperarAsync(string email, string trechoDoAssunto)
        {
            for (var tentativa = 0; tentativa < 100; tentativa++)
            {
                var achado = Para(email).FirstOrDefault(e => e.Assunto.Contains(trechoDoAssunto));
                if (achado != null) return achado;
                await Task.Delay(50);
            }
            return null;
        }
    }
}
