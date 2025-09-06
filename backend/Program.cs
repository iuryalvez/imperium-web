using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using backend.Infra.Data;
using backend.Interfaces;
using backend.Services;
using Microsoft.Extensions.Logging.Console;
using backend.Domain.GestaoClinica.Interface;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.AtendimentoAnimal.Impl;
using backend.Domain.AtendimentoAnimal.Repository;
using backend.Domain.AtendimentoAnimal.Mappings;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data.Repositories;
using backend.Domain.GestaoClinica.Impl;
using backend.Domain.SaudeAnimal.Impl;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.SaudeAnimal.Repository;
using backend.Domain.AtendimentoAnimal.Services;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAutoMapper(typeof(Program));

        // adiciona serviços ao contêiner
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<IClinicaService, ClinicaService>();
        builder.Services.AddScoped<IClinicaRepository, ClinicaRepository>();
        builder.Services.AddScoped<IConsultaService, ConsultaService>();
        builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
        builder.Services.AddScoped<IDietaRepository, DietaRepository>();
        builder.Services.AddScoped<DietaService>();
        builder.Services.AddScoped<IAnimalService, AnimalService>();
        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddScoped<IAcompanhamentoRepository, AcompanhamentoRepository>();
        builder.Services.AddScoped<IAcompanhamentoService, AcompanhamentoService>();
        builder.Services.AddScoped<ITutorService, TutorService>();
        builder.Services.AddScoped<ITutorRepository, TutorRepository>();
        builder.Services.AddScoped<IRacaService, RacaService>();
        builder.Services.AddScoped<IRacaRepository, RacaRepository>();
        builder.Services.AddScoped<IRacaoService, RacaoService>();
        builder.Services.AddScoped<IRacaoRepository, RacaoRepository>();
        builder.Services.AddScoped<IRelatorioService, RelatorioService>();
        builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();

        // configuração do CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins(
                        "https://200.137.215.116:3001",
                        "https://frontend-imperium:3001",
                        "http://localhost:5050",
                        "http://localhost:3000"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMPERIUM WEB API", Version = "v1" });
        });

        // configura o DbContext para usar o Npgsql (PostgreSQL) - pgAdmin
        // builder.Services.AddDbContext<AppDbContext>(opt =>
        //     opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Configura o DbContext para PostgreSQL
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentityCore<Usuario>(options =>
        {
            options.Password.RequiredLength = 6;
            // options.Password.RequireDigit = true;
            // options.Password.RequireLowercase = true;
            // options.Password.RequireUppercase = true;
            // options.Password.RequireNonAlphanumeric = true;
            // options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.AllowedForNewUsers = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        /// configuração da autenticação
        builder.Services.AddAuthentication(options =>
        {
            // define o esquema de autenticação padrão como JWT
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // define os parâmetros de validação do token
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // valida a chave de assinatura do token
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])), // chave secreta para validação
                ValidateIssuer = true, // valida o emissor do token
                ValidIssuer = builder.Configuration["JWT:Issuer"], // emissor esperado
                ValidateAudience = true, // valida o público do token
                ValidAudience = builder.Configuration["JWT:Audience"], // público esperado
                ValidateLifetime = true, // valida a expiração do token
                // RoleClaimType = ClaimTypes.Role, // define a claim de role
                ClockSkew = TimeSpan.Zero // remove tolerância padrão de 5 minutos para expiração
            };

            // define eventos personalizados no ciclo de autenticação
            options.Events = new JwtBearerEvents
            {
                // evento chamado ao receber o token
                OnMessageReceived = context =>
                {
                    // busca o token JWT a partir do cookie HTTP-Only
                    var token = context.Request.Cookies["auth_token"];
                    if (string.IsNullOrEmpty(token))
                    {
                        Console.WriteLine("Token ausente no cookie.");
                    }
                    else
                    {
                        context.Token = token;
                        Console.WriteLine("Token encontrado no cookie.");
                    }
                    return Task.CompletedTask;
                },

                // evento chamado após o token ser validado com sucesso
                OnTokenValidated = async context =>
                {

                    // verifica se o token está presente
                    var token = context.SecurityToken;
                    if (token == null)
                    {
                        Console.WriteLine("Token inválido: objeto de segurança nulo.");
                        context.Fail("Token inválido!");
                        return;
                    }

                    Console.WriteLine("Token validado com sucesso. Verificando renovação e consistência...");

                    try
                    {
                        // validações adicionais de consistência (IP e User-Agent)
                        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                        var principal = authService.ValidateTokenFromCookie(context.Request.Cookies["auth_token"]);

                        authService.ValidateUserClaims(principal, context.HttpContext);

                        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

                        tokenService.RenovaToken(context);

                        Console.WriteLine("Validações de consistência concluídas com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro durante a validação do token ou renovações.");
                        context.Fail("Erro durante a validação ou renovação do token.");
                    }
                }
            };
        });

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

        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(12); // Define a validade do token para 12 horas
        });

        // configuração de Logging para o Console
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
        {
            if (builder.Environment.IsDevelopment())
            {
                options.FormatterName = ConsoleFormatterNames.Simple; // formato simples para desenvolvimento
                options.IncludeScopes = true; // incluindo o escopo para facilitar a depuração em dev
            }
            else if (builder.Environment.IsProduction())
            {
                options.FormatterName = ConsoleFormatterNames.Systemd; // formato systemd para produção
            }
        });
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging")); // configura o logging a partir do appsettings.json

        var app = builder.Build();

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseSwagger();

        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });

        app.Run();
    }
}
