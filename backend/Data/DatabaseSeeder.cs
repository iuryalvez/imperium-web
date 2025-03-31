using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace backend.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<Usuario>();

            // Usuários
            var usuarios = new[]
            {
                new Usuario
                {
                    Nome = "Iury Dev",
                    UserName = "admin.iury",
                    Email = "iuryalves774@gmail.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "IURYALVES774@GMAIL.COM",
                    NormalizedUserName = "ADMIN.IURY",
                    DataCad = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Desenvolvedor do sistema",
                    IsAdmin = true
                },
                new Usuario
                {
                    Nome = "Rafael Dev",
                    UserName = "admin.rafael",
                    Email = "rafaelnmcosta@gmail.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    NormalizedUserName = "ADMIN.RAFAEL",
                    DataCad = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Desenvolvedor do sistema",
                    IsAdmin = true
                },
                new Usuario
                {
                    Nome = "Professor Ceron",
                    UserName = "admin.ceron",
                    Email = "ceron@ufg.br",
                    EmailConfirmed = true,
                    NormalizedEmail = "CERON@UFG.BR",
                    NormalizedUserName = "ADMIN.CERON",
                    DataCad = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Administrador do sistema",
                    IsAdmin = true
                },
                new Usuario
                {
                    Nome = "Professor Hugo",
                    UserName = "admin.hugo",
                    Email = "hadn@ufg.br",
                    EmailConfirmed = true,
                    NormalizedEmail = "HADN@UFG.BR",
                    NormalizedUserName = "ADMIN.HUGO",
                    DataCad = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Desenvolvedor do sistema",
                    IsAdmin = true
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
