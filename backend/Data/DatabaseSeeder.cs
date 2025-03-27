using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<Usuario>();

            // Perfis de Usuário
            modelBuilder.Entity<Perfil>().HasData(
                new Perfil { Name = "Administrador", NormalizedName = "ADMINISTRADOR", Tipo = 1, Descricao = "Administrador do sistema"},
                new Perfil { Name = "Gestor", Tipo = 2, NormalizedName = "GESTOR", Descricao = "Gestor de clínica" },
                new Perfil { Name = "Veterinário", Tipo = 3, NormalizedName = "VETERINÁRIO", Descricao = "Veterinário de clínica" },
                new Perfil { Name = "Secretário", Tipo = 4, NormalizedName = "SECRETÁRIO", Descricao = "Secretário de clínica" },
                new Perfil { Name = "Técnico", Tipo = 5, NormalizedName = "TÉCNICO", Descricao = "Técnico de clínica" },
                new Perfil { Name = "Visualizador", Tipo = 6, NormalizedName = "VISUALIZADOR", Descricao = "Usuário apenas visualizador" }
            );

            // Usuários
            var usuarios = new[]
            {
                new Usuario
                {
                    Nome = "Administrador",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    DataCad = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Administrador do sistema"
                }
            };

            foreach (var usuario in usuarios)
            {
                usuario.PasswordHash = passwordHasher.HashPassword(usuario, "Senha123!"); // Define a senha padrão
            }

            modelBuilder.Entity<Usuario>().HasData(usuarios);

        }
    }
}
