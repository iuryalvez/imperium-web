using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using backend.Data;
using backend.Services;
using backend.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configura o DbContext para usar o Npgsql (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura o CORS para permitir requisições do frontend
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowSpecificOrigin",
//         builder =>
//         {
//             builder.WithOrigins("http://localhost:3000")
//                    .AllowAnyHeader()
//                    .AllowAnyMethod();
//         });
// });

// Configuração do serviço de token
// builder.Services.AddScoped<ITokenService, TokenService>();

// Configuração da Autenticação JWT
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })

// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"],
//         ValidAudience = builder.Configuration["Jwt:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//     };
// });

// Configuração da Autorização
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
//     options.AddPolicy("Profissional", policy => policy.RequireRole("Profissional"));
//     options.AddPolicy("Secretário", policy => policy.RequireRole("Secretário"));
// });

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();

// Adiciona a geração de documentação Swagger
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Popula o banco de dados com dados iniciais usando o seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Chama o seeder para popular o banco de dados
    var seeder = new Seeder(context);
    seeder.SeedData();
}

// Configura o pipeline de requisição HTTP
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// Adiciona suporte ao CORS no pipeline
// app.UseCors("AllowAllOrigins");

// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
