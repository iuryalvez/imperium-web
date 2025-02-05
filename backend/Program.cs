using System.Text;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;

using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Services;
using Microsoft.Build.Logging;
using Microsoft.Extensions.Logging.Console;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // adiciona serviços ao contêiner
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ICookieService, CookieService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        // configuração do CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://localhost:5050")
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

        builder.Services.AddIdentity<Usuario, Perfil>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
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
                RoleClaimType = ClaimTypes.Role, // define a claim de role
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
            // define as policies para cada perfil
            options.AddPolicy("VisualizaTudo", policy => policy.RequireRole("ADMIN","GESTOR","VETERINARIO","SECRETARIO","TECNICO"));
            options.AddPolicy("GerenciaClinica", policy => policy.RequireRole("ADMIN","GESTOR"));
            options.AddPolicy("GerenciaAnimais", policy => policy.RequireRole("ADMIN","VETERINARIO"));
            options.AddPolicy("GerenciaTutores", policy => policy.RequireRole("ADMIN","VETERINARIO","SECRETARIO"));
            options.AddPolicy("GerenciaConsultas", policy => policy.RequireRole("ADMIN","VETERINARIO","SECRETARIO"));
            options.AddPolicy("GerenciaSistema", policy => policy.RequireClaim("isAdmin", "true"));
            options.AddPolicy("AcessoLoginPolicy", policy => policy.RequireClaim("acessoLogin", "true"));
            
            // policy de operação OU entre AcessoLogin e TodosUsuarios (para as listas de vinculos) 
            options.AddPolicy("AcessoLoginOuTodosUsuarios", policy =>
            {
                policy.RequireAssertion(context =>
                    // verifica se o token é de login
                    context.User.HasClaim(c => c.Type == "acessoLogin" && c.Value == "true")
                    ||
                    // se não é de login, verifica se o token tem qualquer valor na claim "role"
                    context.User.HasClaim(c => c.Type == ClaimTypes.Role && !string.IsNullOrEmpty(c.Value))
                );
            });
        });

        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(30); // Define a validade do token para 30 minutos
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

        // app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}