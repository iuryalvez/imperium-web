using System.Net;
using System.Net.Mail;

using backend.Domain.Auth.Interfaces;

namespace backend.Domain.Auth.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool Configurado =>
            !string.IsNullOrWhiteSpace(_configuration["EmailSettings:SmtpServer"])
            && !string.IsNullOrWhiteSpace(_configuration["EmailSettings:SmtpUser"])
            && !string.IsNullOrWhiteSpace(_configuration["EmailSettings:SmtpPass"]);

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out var porta) ? porta : 587;
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPass"];

                // Remetente que aparece para quem recebe. Pode ser outro endereço (ex.: o do grupo), desde que
                // a conta do SmtpUser tenha permissão de enviar como ele; senão o Gmail troca pelo SmtpUser.
                var remetente = _configuration["EmailSettings:FromAddress"];
                if (string.IsNullOrWhiteSpace(remetente)) remetente = smtpUser;
                var nomeRemetente = _configuration["EmailSettings:FromName"];
                if (string.IsNullOrWhiteSpace(nomeRemetente)) nomeRemetente = "Suporte IMPERIUM/BRIS";

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(remetente!, nomeRemetente),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail para {ToEmail}", toEmail);
                throw new InvalidOperationException("Falha no envio de e-mail.");
            }
        }
    }
}
