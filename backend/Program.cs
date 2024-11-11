using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // adiciona serviços ao contêiner
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.AddTransient<ITokenService, TokenService>();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMPERIUM WEB API", Version = "v1" });

            // Configura a segurança para JWT no Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                 In = ParameterLocation.Header,
                 Description = "Insira 'Bearer' [espaço] e o token JWT no campo abaixo.\n\nExemplo: 'Bearer {token}'",
                 Name = "Authorization",
                 Type = SecuritySchemeType.ApiKey,
                 BearerFormat = "JWT",
                 Scheme = "Bearer"
             });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                 {
                     new OpenApiSecurityScheme
                     {
                         Reference = new OpenApiReference
                         {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                         }
                     },
                     Array.Empty<string>()
                 }
             });
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
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // configuração da autorização
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Reduz a tolerância para expiração do token
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

        });

        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(30); // Define a validade do token para 30 minutos
        });

        var app = builder.Build();

        // ambiente de produção inacessível
        if (app.Environment.IsProduction())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // "dotnet watch run" abre o swagger ao rodar a aplicação no ambiente de desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>{c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMPERIUM WEB API");});
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}