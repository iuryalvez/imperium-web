using System.Text;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Logging.Console;
using NuGet.Protocol;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // adiciona serviços ao contêiner
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddTransient<ITokenService, TokenService>();
        builder.Services.AddTransient<ICookieService, CookieService>();
        builder.Services.AddTransient<IAuthService, AuthService>();

        // Configuração de CORS
        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy("AllowSwaggerOrigins", policy =>
        //     {
        //         policy.WithOrigins("http://localhost:5050") // Origem do Swagger
        //             .AllowAnyHeader()
        //             .AllowAnyMethod()
        //             .AllowCredentials(); // Permite cookies/tokens no cabeçalho
        //     });
        // });

        // Configuração do CORS
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

            // Definir HTTPS como padrão
            // c.AddServer(new OpenApiServer { Url = "https://localhost:5050" });
            
            // Configura a segurança para JWT no Swagger
            // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //      In = ParameterLocation.Header,
            //      Description = "Insira 'Bearer' [espaço] e o token JWT no campo abaixo.\n\nExemplo: 'Bearer {token}'",
            //      Name = "Authorization",
            //      Type = SecuritySchemeType.ApiKey,
            //      BearerFormat = "JWT",
            //      Scheme = "Bearer"
            //  });
            // c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //  {
            //      {
            //          new OpenApiSecurityScheme
            //          {
            //              Reference = new OpenApiReference
            //              {
            //                  Type = ReferenceType.SecurityScheme,
            //                  Id = "Bearer"
            //              }
            //          },
            //          Array.Empty<string>()
            //      }
            //  });

            // Configuração de segurança para cookies
            // c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
            // {
            //     Name = "auth_token",
            //     Type = SecuritySchemeType.ApiKey,
            //     In = ParameterLocation.Cookie,
            //     Description = "Token JWT armazenado em cookie. Faça login para obter o cookie."
            // });

            // c.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "cookieAuth"
            //             }
            //         },
            //         new string[] { }
            //     }
            // });
        });

        // configura o DbContext para usar o Npgsql (PostgreSQL) - pgAdmin
        builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<Usuario, Perfil>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true; // Garante que o email seja único
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        /// Configuração da autenticação
        builder.Services.AddAuthentication(options =>
        {
            // Define o esquema de autenticação padrão como JWT
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Define os parâmetros de validação do token
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // Valida a chave de assinatura do token
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])), // Chave secreta para validação
                ValidateIssuer = true, // Valida o emissor do token
                ValidIssuer = builder.Configuration["JWT:Issuer"], // Emissor esperado
                ValidateAudience = true, // Valida o público do token
                ValidAudience = builder.Configuration["JWT:Audience"], // Público esperado
                ValidateLifetime = true, // Valida a expiração do token
                ClockSkew = TimeSpan.Zero // Remove tolerância padrão de 5 minutos para expiração
            };
            
            // Define eventos personalizados no ciclo de autenticação
            options.Events = new JwtBearerEvents
            {
                // Evento chamado ao receber o token
                OnMessageReceived = context =>
                {
                    // Busca o token JWT a partir do cookie HTTP-Only
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

                // Evento chamado após o token ser validado com sucesso
                OnTokenValidated = async context =>
                {
                    var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                    var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

                    // Valida os dados do token
                    await authService.ValidaContext(context);
                    await tokenService.RenovaToken(context);
                }
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            // Define as policies para cada perfil
            options.AddPolicy("GerenciaSistema", policy => policy.RequireRole("ADMIN"));
            options.AddPolicy("GerenciaClinica", policy => policy.RequireRole("ADMIN,GESTOR"));
            options.AddPolicy("GerenciaAnimais", policy => policy.RequireRole("ADMIN,VETERINARIO"));
            options.AddPolicy("GerenciaTutores", policy => policy.RequireRole("ADMIN,VETERINARIO,SECRETARIO"));
            options.AddPolicy("GerenciaConsultas", policy => policy.RequireRole("ADMIN,VETERINARIO,SECRETARIO"));
            options.AddPolicy("VisualizaTudo", policy => policy.RequireRole("ADMIN,GESTOR,VETERINARIO,SECRETARIO,TECNICO"));
            options.AddPolicy("AcessoLoginPolicy", policy => policy.RequireClaim("AcessoLogin", "true"));
            
            // Policy de operação OU entre AcessoLogin e TodosUsuarios (para as listas de vinculos) 
            options.AddPolicy("AcessoLoginOuTodosUsuarios", policy =>
            {
                policy.RequireAssertion(context =>
                    // Verifica se o token é de login
                    context.User.HasClaim(c => c.Type == "AcessoLogin" && c.Value == "true")
                    ||
                    // Se não é de login, verifica se o token tem qualquer valor na claim "role"
                    context.User.HasClaim(c => c.Type == ClaimTypes.Role && !string.IsNullOrEmpty(c.Value))
                );
            });
        });

        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(30); // Define a validade do token para 30 minutos
        });

        // Habilita o redirecionamento para HTTPS
        // builder.Services.AddHttpsRedirection(options =>
        // {
        //     options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        //     options.HttpsPort = 5050; // Porta configurada para HTTPS
        // });

        // Configuração de Logging para o Console
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

        // ambiente de produção inacessível
        // if (app.Environment.IsProduction())
        // {
        //     app.UseExceptionHandler("/Clinica/Error");
        //     app.UseHsts();
        // }

        // // "dotnet watch run" abre o swagger ao rodar a aplicação no ambiente de desenvolvimento
        // if (app.Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI(c =>{c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMPERIUM WEB API");});
        // }

        app.UseCors("AllowSpecificOrigin");

        // app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}