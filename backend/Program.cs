using System.Text;
// Using statements para funcionalidades do ASP.NET Core, EF Core e JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Using statements para as camadas de Domínio e Infraestrutura do seu projeto
using backend.Domain.AtendimentoAnimal.Impl;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.AtendimentoAnimal.Repository;
using backend.Domain.AtendimentoAnimal.Services;
using backend.Domain.GestaoClinica.Impl;
using backend.Domain.GestaoClinica.Interface;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.SaudeAnimal.Impl;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.SaudeAnimal.Repository;
using backend.Infra.Data;
using backend.Infra.Data.Repositories;
using backend.Interfaces;
using backend.Services;
using backend.Domain.SaudeAnimal.Services;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ========================================================================
        // 1. REGISTRO DE SERVIÇOS ESSENCIAIS E CONFIGURAÇÕES
        // ========================================================================

        #region Serviços Essenciais

        /// <summary>
        /// Adiciona o AutoMapper, uma ferramenta para mapear (converter) objetos de um tipo para outro.
        /// Ex: Converter uma entidade do banco (Animal) para um DTO (AnimalDto).
        /// </summary>
        builder.Services.AddAutoMapper(config => { }, typeof(Program));

        /// <summary>
        /// Adiciona os serviços necessários para que a API funcione com Controllers (padrão MVC/API).
        /// </summary>
        builder.Services.AddControllers();

        /// <summary>
        /// Habilita a exploração de endpoints da API, essencial para ferramentas como o Swagger.
        /// </summary>
        builder.Services.AddEndpointsApiExplorer();

        /// <summary>
        /// Configura o Swagger, uma ferramenta que gera uma documentação interativa da sua API.
        /// Facilita o teste e o entendimento dos endpoints disponíveis.
        /// </summary>
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMPERIUM WEB API", Version = "v1" });
        });

        #endregion

        // ========================================================================
        // 2. INJEÇÃO DE DEPENDÊNCIA (SERVICES E REPOSITORIES)
        // ========================================================================

        #region Injeção de Dependência
        // Explicação: Aqui você está registrando suas classes no contêiner de Injeção de Dependência (DI).
        // builder.Services.AddScoped<Interface, Implementacao>();
        // "Scoped" significa que uma nova instância da classe será criada para cada requisição HTTP.
        // Isso garante que os dados de um usuário não se misturem com os de outro durante a mesma requisição.

        // Gestão de Acesso e Autenticação
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();

        // Gestão da Clínica
        builder.Services.AddScoped<IClinicaService, ClinicaService>();
        builder.Services.AddScoped<IClinicaRepository, ClinicaRepository>();

        // Atendimento ao Animal
        builder.Services.AddScoped<IAnimalService, AnimalService>();
        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddScoped<ITutorService, TutorService>();
        builder.Services.AddScoped<ITutorRepository, TutorRepository>();
        builder.Services.AddScoped<IRacaService, RacaService>();
        builder.Services.AddScoped<IRacaRepository, RacaRepository>();

        // Saúde Animal
        builder.Services.AddScoped<IConsultaService, ConsultaService>();
        builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
        builder.Services.AddScoped<IDietaRepository, DietaRepository>();
        builder.Services.AddScoped<IDietaService, DietaService>();
        builder.Services.AddScoped<IAcompanhamentoRepository, AcompanhamentoRepository>();
        builder.Services.AddScoped<IAcompanhamentoService, AcompanhamentoService>();
        builder.Services.AddScoped<IRacaoService, RacaoService>();
        builder.Services.AddScoped<IRacaoRepository, RacaoRepository>();
        builder.Services.AddScoped<IRelatorioService, RelatorioService>();
        builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();
        builder.Services.AddScoped<ICalculoNecessidadeEnergetica, CalculoNecessidadeEnergetica>();

        // Serviços Utilitários
        builder.Services.AddScoped<IEmailService, EmailService>();

        #endregion

        // ========================================================================
        // 3. CONFIGURAÇÃO DE POLÍTICA DE CORS
        // ========================================================================

        #region CORS (Cross-Origin Resource Sharing)
        /// <summary>
        /// Configura o CORS para permitir que o seu frontend (rodando em outras origens/endereços)
        /// possa fazer requisições para esta API. Sem isso, o navegador bloquearia as chamadas por segurança.
        /// </summary>
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
            {
                policyBuilder.WithOrigins(
                    "https://200.137.215.116:3001", // Origem de produção
                    "https://frontend-imperium:3001", // Origem via Docker Compose/rede interna
                    "http://localhost:5050", // Origem de desenvolvimento (ex: Swagger)
                    "http://localhost:3000", // Origem de desenvolvimento (ex: React, Angular)
                    "http://localhost:8080" // Docker production local testing
                )
                .AllowAnyHeader() // Permite qualquer cabeçalho na requisição (ex: Content-Type, Authorization)
                .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, PUT, DELETE)
                .AllowCredentials(); // Permite o envio de cookies entre origens, essencial para autenticação baseada em cookies.
            });
        });
        #endregion

        // ========================================================================
        // 4. CONFIGURAÇÃO DO BANCO DE DADOS (ENTITY FRAMEWORK CORE)
        // ========================================================================

        #region Banco de Dados
        /// <summary>
        /// Configura o Entity Framework Core para usar o PostgreSQL como provedor de banco de dados.
        /// AddDbContextFactory é uma abordagem otimizada que cria instâncias do DbContext sob demanda,
        /// sendo mais eficiente em alguns cenários (como Blazor Server ou serviços de longa duração) do que AddDbContext.
        /// A string de conexão é lida do arquivo appsettings.json.
        /// </summary>
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        #endregion

        // ========================================================================
        // 5. CONFIGURAÇÃO DE IDENTIDADE, AUTENTICAÇÃO E AUTORIZAÇÃO
        // ========================================================================

        #region Identity (Gerenciamento de Usuários)
        /// <summary>
        /// Configura o ASP.NET Core Identity, o sistema para gerenciar usuários, senhas, perfis, etc.
        /// Define a classe 'Usuario' como a entidade de usuário e relaxa as regras de senha para o desenvolvimento.
        /// </summary>
        builder.Services.AddIdentityCore<Usuario>(options =>
        {
            options.Password.RequiredLength = 6;
            options.SignIn.RequireConfirmedEmail = false; // Não exige confirmação de e-mail para login
            options.Lockout.AllowedForNewUsers = false; // Desativa o bloqueio de conta para novos usuários
        })
        .AddEntityFrameworkStores<AppDbContext>() // Conecta o Identity ao seu banco de dados via EF Core
        .AddSignInManager() // Adiciona o gerenciador de login (ex: PasswordSignInAsync)
        .AddDefaultTokenProviders(); // Adiciona provedores para gerar tokens (ex: reset de senha)

        /// <summary>
        /// Define a validade dos tokens gerados pelo Identity (ex: token de reset de senha).
        /// </summary>
        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(24);
        });
        #endregion

        #region Authentication (JWT - JSON Web Token)
        /// <summary>
        /// Configura o sistema de autenticação, definindo o JWT (Bearer) como o método padrão.
        /// A API irá esperar um token JWT para autenticar as requisições.
        /// </summary>
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Define as regras para validar um token JWT recebido.
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // Garante que o token foi assinado com a chave secreta correta
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),

                ValidateIssuer = true, // Garante que o token foi emitido pelo servidor correto
                ValidIssuer = builder.Configuration["JWT:Issuer"],

                ValidateAudience = true, // Garante que o token é destinado a esta aplicação
                ValidAudience = builder.Configuration["JWT:Audience"],

                ValidateLifetime = true, // Garante que o token não expirou
                ClockSkew = TimeSpan.Zero // Remove a tolerância de tempo padrão na validação da expiração
            };

            // Eventos customizados para controlar o fluxo de autenticação.
            options.Events = new JwtBearerEvents
            {
                // Este evento é disparado quando uma requisição chega.
                // Sua função aqui é extrair o token JWT de um cookie HttpOnly chamado "auth_token".
                // Isso é uma medida de segurança para proteger o token contra ataques XSS no frontend.
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["auth_token"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                },

                // Este evento é disparado APÓS o token ter sido validado com sucesso pelas regras acima.
                // Aqui você implementa lógicas de negócio adicionais, como renovação de token.
                OnTokenValidated = async context =>
                {
                    try
                    {
                        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

                        // Validações de consistência (ex: IP, User-Agent)
                        authService.ValidateUserClaims(context.Principal, context.HttpContext);

                        // Lógica para renovar o token se ele estiver perto de expirar
                        tokenService.RenovaToken(context);
                    }
                    catch (Exception)
                    {
                        context.Fail("Erro durante validações de consistência ou renovação do token.");
                    }
                }
            };
        });
        #endregion


        builder.Services.AddAuthorization(options =>
        {
            // Define as policies verificando a claim "Perfis"
            options.AddPolicy("VisualizaTudo", policy =>
                policy.RequireAssertion(context =>
                    context.User.TemPerfil("isAdmin", "GESTOR", "VETERINARIO", "SECRETARIO", "TECNICO")));

            options.AddPolicy("GerenciaClinica", policy =>
                policy.RequireAssertion(context =>
                    context.User.TemPerfil("isAdmin", "GESTOR")));

            options.AddPolicy("GerenciaAnimais", policy =>
                policy.RequireAssertion(context =>
                    context.User.TemPerfil("isAdmin", "GESTOR", "VETERINARIO")));

            options.AddPolicy("GerenciaTutores", policy =>
                policy.RequireAssertion(context =>
                    context.User.TemPerfil("isAdmin", "GESTOR", "VETERINARIO", "SECRETARIO")));

            options.AddPolicy("GerenciaConsultas", policy =>
                policy.RequireAssertion(context =>
                    context.User.TemPerfil("isAdmin", "GESTOR", "VETERINARIO", "SECRETARIO")));

            options.AddPolicy("GerenciaSistema", policy =>
                policy.RequireClaim("isAdmin", "true"));

            options.AddPolicy("AcessoLoginPolicy", policy =>
                policy.RequireClaim("acessoLogin", "true"));

            // Policy para validar AcessoLogin ou se o usuário tem algum perfil ativo
            options.AddPolicy("AcessoLoginOuTodosUsuarios", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "acessoLogin" && c.Value == "true") ||
                    context.User.TemAlgumPerfil()));
        });

        // ========================================================================
        // 6. CONFIGURAÇÃO DE LOGGING
        // ========================================================================

        #region Logging
        /// <summary>
        /// Configura o sistema de logging da aplicação.
        /// </summary>
        builder.Logging.ClearProviders(); // Remove provedores de log padrão
        builder.Logging.AddConsole(options => // Adiciona o provedor de log para o console
        {
            // Define o formato do log com base no ambiente (mais legível em dev, mais estruturado em prod)
            if (builder.Environment.IsDevelopment())
            {
                options.FormatterName = "simple";
            }
            else
            {
                options.FormatterName = "systemd";
            }
        });
        // Carrega as configurações de nível de log (ex: "Default": "Debug") do appsettings.json
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
        #endregion

        // ========================================================================
        // 7. CONSTRUÇÃO E CONFIGURAÇÃO DO PIPELINE DE REQUISIÇÃO HTTP
        // ========================================================================

        var app = builder.Build();

        // O pipeline define a ORDEM em que uma requisição HTTP é processada. A ordem é CRÍTICA.

        // Register custom middleware
        app.UseMiddleware<backend.Middlewares.SwaggerBasicAuthMiddleware>();

        app.UseCors("AllowSpecificOrigin"); // 1. Aplica a política de CORS

        app.UseAuthentication(); // 2. Tenta autenticar o usuário (lê o cookie, valida o token)
        app.UseAuthorization(); // 3. Verifica se o usuário autenticado tem permissão para acessar o endpoint

        app.MapControllers(); // 4. Roteia a requisição para o Controller apropriado

        // Habilita o Swagger e a UI do Swagger apenas em ambiente de desenvolvimento
        // if (app.Environment.IsDevelopment())
        // {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
        // }

        app.Run();
    }
}
