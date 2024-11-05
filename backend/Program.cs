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
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMPERIUM WEB API", Version = "v1" });

            // Configura a segurança para JWT no Swagger
            // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     Name = "Authorization",
            //     Type = SecuritySchemeType.ApiKey,
            //     Scheme = "Bearer",
            //     BearerFormat = "JWT",
            //     In = ParameterLocation.Header,
            //     Description = "Insira 'Bearer' [espaço] e o token JWT no campo abaixo.\n\nExemplo: 'Bearer {token}'"
            // });

            // c.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             }
            //         },
            //         Array.Empty<string>()
            //     }
            // });
        });

        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.AddTransient<ITokenService, TokenService>();

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
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            string jwtSecret = builder.Configuration["JWT:Secret"] ?? throw new ArgumentNullException("JWT:Secret", "O segredo JWT não pode ser nulo ou vazio.");

            byte[] secretBytes = Encoding.UTF8.GetBytes(jwtSecret);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["JWT:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretBytes)
            };
        });

        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(30); // Define a validade do token para 30 minutos
        });

        var app = builder.Build();

        // app.Use(async (context, next) =>
        // {
        //     if (context.Request.Path.StartsWithSegments("/swagger") && !context.User.Identity.IsAuthenticated)
        //     {
        //         // Redireciona para uma página de login básica
        //         context.Response.ContentType = "text/html";
        //         await context.Response.WriteAsync(@"<!DOCTYPE html>
        //     <html>
        //     <head>
        //         <title>Login</title>
        //         <style>
        //             body {
        //                 font-family: Arial, sans-serif;
        //                 background-color: #f4f4f4;
        //                 display: flex;
        //                 justify-content: center;
        //                 align-items: center;
        //                 height: 100vh;
        //                 margin: 0;
        //             }
        //             .login-card {
        //                 background-color: #fff;
        //                 padding: 20px;
        //                 border-radius: 8px;
        //                 box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        //                 max-width: 400px;
        //                 width: 100%;
        //             }
        //             .login-card h2 {
        //                 margin-top: 0;
        //             }
        //             .login-card label {
        //                 display: block;
        //                 margin-bottom: 8px;
        //             }
        //             .login-card input[type='email'],
        //             .login-card input[type='password'] {
        //                 width: 100%;
        //                 padding: 10px;
        //                 margin-bottom: 10px;
        //                 border: 1px solid #ccc;
        //                 border-radius: 4px;
        //             }
        //             .login-card input[type='checkbox'] {
        //                 margin-right: 8px;
        //             }
        //             .login-card button {
        //                 width: 100%;
        //                 padding: 10px;
        //                 background-color: #007bff;
        //                 border: none;
        //                 border-radius: 4px;
        //                 color: #fff;
        //                 font-size: 16px;
        //                 cursor: pointer;
        //             }
        //             .login-card button:hover {
        //                 background-color: #0056b3;
        //             }
        //         </style>
        //     </head>
        //     <body>
        //         <div class='login-card'>
        //             <h2>Login</h2>
        //             <form method='post' action='/api/auth/login-swagger'>
        //                 <label>Email:</label>
        //                 <input type='email' name='email' required />
        //                 <label>Senha:</label>
        //                 <input type='password' name='senha' required />
        //                 <label>
        //                     <input type='checkbox' name='rememberMe' /> Remember Me
        //                 </label>
        //                 <button type='submit'>Login</button>
        //             </form>
        //         </div>
        //     </body>
        //     </html>");
        //         return;
        //     }

        //     await next.Invoke();
        // });

        // "dotnet watch run" abre o swagger ao rodar a aplicação no ambiente de desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // ambiente de produção inacessível
        if (app.Environment.IsProduction())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}