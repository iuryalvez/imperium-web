using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using backend.Domain.GestaoDeAcesso.Entities;

namespace backend.Infra.Data
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
                    Id = -1,
                    Nome = "Admin Iury",
                    UserName = "admin.iury",
                    Email = "iury@gsmail.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "IURY@GSMAIL.COM",
                    NormalizedUserName = "IURY",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Desenvolvedor do Sistema",
                    Telefone1 = "62987654321",
                    Whatsapp = "62987654321",
                    Cpf = "01234567890",
                    IsAtivo = true,
                    IsAdmin = true
                },
                new Usuario
                {
                    Id = -2,
                    Nome = "Admin Rafael",
                    UserName = "admin.rafael",
                    Email = "rafael@gsmail.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "RAFAEL@GSMAIL.COM",
                    NormalizedUserName = "ADMIN RAFAEL",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Desenvolvedor do Sistema",
                    Telefone1 = "62912345678",
                    Whatsapp = "62912345678",
                    Cpf = "01234567891",
                    IsAtivo = true,
                    IsAdmin = true
                },
                new Usuario
                {
                    Id = -3,
                    Nome = "Admin Hugo",
                    UserName = "hugo",
                    Email = "hugo@gsmail.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "HUGO@GSMAIL.COM",
                    NormalizedUserName = "ADMIN HUGO",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Desenvolvedor do Sistema",
                    Telefone1 = "62910203040",
                    Whatsapp = "62910203040",
                    Cpf = "01234567892",
                    IsAtivo = true,
                    IsAdmin = true
                },
                new Usuario
                {
                    Id = -4,
                    Nome = "Admin Ceron",
                    UserName = "ceron",
                    Email = "ceron@gsmail.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "CERON@GSMAIL.COM",
                    NormalizedUserName = "ADMIN CERON",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Formacao = "Administrador do Sistema",
                    Telefone1 = "62940302010",
                    Whatsapp = "62940302010",
                    Cpf = "01234567893",
                    IsAtivo = true,
                    IsAdmin = true
                },
            };

            foreach (var usuario in usuarios)
            {
                usuario.PasswordHash = passwordHasher.HashPassword(usuario, "Caogato@1"); // Define a senha padrão
            }

            modelBuilder.Entity<Usuario>().HasData(usuarios);

        }
    }
}
