using backend.Domain.Auth.Interfaces;
using backend.Infra.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace backend.Tests.Infra
{
    /// <summary>
    /// Sobe a API real (Program.cs) em memória, apontando para um PostgreSQL descartável do Testcontainers.
    /// É criada uma vez por execução (collection fixture "api") e aplica as migrations no banco novo.
    /// Não usa o .env, o banco local nem o de homologação.
    /// </summary>
    public class ImperiumApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        // Configuração só de teste. Nunca use a chave real aqui.
        public const string JwtKey = "chave-jwt-somente-para-testes-automatizados-do-imperium-0123456789";
        public const string JwtIssuer = "imperium-testes";
        public const string JwtAudience = "imperium-testes";
        public const string FrontendUrl = "https://imperium.teste.local";

        /// <summary>Limite de requisições por IP nas rotas de auth durante os testes (por janela de 60 s).</summary>
        public const int LimiteAuth = 20;

        /// <summary>E-mails "enviados" pela API durante os testes.</summary>
        public EmailFalso Emails { get; } = new();

        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("imperium_testes")
            .WithUsername("imperium")
            .WithPassword("imperium_testes")
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            // O Program.cs lê a configuração do IConfiguration (variáveis de ambiente incluídas).
            // As variáveis são definidas antes de a API subir para valerem já na montagem do host.
            DefinirConfiguracao("ConnectionStrings__DefaultConnection", _postgres.GetConnectionString());
            DefinirConfiguracao("Jwt__Key", JwtKey);
            DefinirConfiguracao("Jwt__Issuer", JwtIssuer);
            DefinirConfiguracao("Jwt__Audience", JwtAudience);
            DefinirConfiguracao("Jwt__ExpiresInMinutes", "60");
            DefinirConfiguracao("Jwt__RenewInMinutesLeft", "5");
            DefinirConfiguracao("CookieSecure", "false");
            DefinirConfiguracao("FrontendUrl", FrontendUrl);
            DefinirConfiguracao("RateLimit__Auth__PermitLimit", LimiteAuth.ToString());
            DefinirConfiguracao("RateLimit__Auth__WindowSeconds", "60");
            DefinirConfiguracao("SwaggerAuth__Username", "usuario-teste");
            DefinirConfiguracao("SwaggerAuth__Password", "senha-teste");

            await using var contexto = CriarContexto();
            await contexto.Database.MigrateAsync();
        }

        public new async Task DisposeAsync()
        {
            await base.DisposeAsync();
            await _postgres.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Ambiente próprio: não liga o Swagger e não depende de appsettings de dev/produção
            builder.UseEnvironment("Testing");

            // Captcha e e-mail não saem para a internet nos testes
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ITurnstileService>();
                services.AddSingleton<ITurnstileService, TurnstileFalso>();
                services.RemoveAll<IEmailService>();
                services.AddSingleton<IEmailService>(Emails);
            });
        }

        /// <summary>DbContext do banco de teste, para montar cenários e conferir o que foi gravado.</summary>
        public AppDbContext CriarContexto()
        {
            var fabrica = Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
            return fabrica.CreateDbContext();
        }

        private static void DefinirConfiguracao(string chave, string valor) =>
            Environment.SetEnvironmentVariable(chave, valor);
    }

    [CollectionDefinition(Nome)]
    public class ApiCollection : ICollectionFixture<ImperiumApiFactory>
    {
        public const string Nome = "api";
    }
}
