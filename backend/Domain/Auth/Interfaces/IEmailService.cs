namespace backend.Domain.Auth.Interfaces {
    public interface IEmailService
    {
        /// <summary>Se o SMTP tem servidor, usuário e senha configurados.</summary>
        bool Configurado { get; }

        public Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
