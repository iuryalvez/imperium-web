using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace backend.Data
{
    public static class DatabaseSeeder_sample
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<Usuario>();

            // Usuários
            var usuarios = new[]
            {
                new Usuario
                {
                    Id = -1,
                    Nome = "Administrador",
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    NormalizedUserName = "ADMIN",
                    DataCad = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Admninistrador do Sistema",
                    IsAdmin = true
                }
                // outros usuários...
            };

            foreach (var usuario in usuarios)
            {
                usuario.PasswordHash = passwordHasher.HashPassword(usuario, "Uma!Senha2Muito3Dificil@"); // Define a senha padrão
            }

            modelBuilder.Entity<Usuario>().HasData(usuarios);

        }
    }
}
