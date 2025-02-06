using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Dtos.Usuario;
using backend.Dtos.Vinculo;
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
                new Perfil { Id = 1, Name = "ADMIN", Tipo = 1, Descricao = "Administrador do sistema"},
                new Perfil { Id = 2, Name = "GESTOR", Tipo = 2, Descricao = "Gestor de clínica" },
                new Perfil { Id = 3, Name = "VETERINARIO", Tipo = 3, Descricao = "Veterinário de clínica" },
                new Perfil { Id = 4, Name = "SECRETARIO", Tipo = 4, Descricao = "Secretário de clínica" },
                new Perfil { Id = 5, Name = "TECNICO", Tipo = 5, Descricao = "Técnico de clínica" },
                new Perfil { Id = 6, Name = "VISUALIZADOR", Tipo = 6, Descricao = "Usuário apenas visualizador" },
                new Perfil { Id = 98, Name = "PENDENTE", Tipo = 98, Descricao = "Usuário pendente de ativação" },
                new Perfil { Id = 99, Name = "INATIVO", Tipo = 99, Descricao = "Usuário inativo" }
            );

            // Clínicas
            modelBuilder.Entity<Clinica>().HasData(
                new Clinica { Id = 1, NomeFantasia = "Clinica EVZ / UFG", NomeClinica = "Clinica EVZ LTDA", Cnpj = "00000000000100" },
                new Clinica { Id = 2, NomeFantasia = "Clinica Ceron", NomeClinica = "Clinica Ceron Empreendimentos", Cnpj = "00000000000200" },
                new Clinica { Id = 3, NomeFantasia = "Clinica Nascimento", NomeClinica = "Clinica HADN S.A.", Cnpj = "00000000000300" }
            );

            // Usuários
            var usuarios = new[]
            {
                new Usuario
                {
                    Id = 1,
                    Nome = "Administrador",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(), // Adiciona um SecurityStamp único
                    Formacao = "Administrador do sistema"
                },
                new Usuario
                {
                    Id = 2,
                    Nome = "Gestor da Clínica",
                    UserName = "gestor@gmail.com",
                    Email = "gestor@gmail.com",
                    NormalizedEmail = "GESTOR@GMAIL.COM",
                    NormalizedUserName = "GESTOR@GMAIL.COM",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(), // Adiciona um SecurityStamp único
                    Formacao = "Gestão Clínica"
                },
                new Usuario
                {
                    Id = 3,
                    Nome = "Veterinário",
                    UserName = "veterinario@gmail.com",
                    Email = "veterinario@gmail.com",
                    NormalizedEmail = "VETERINARIO@GMAIL.COM",
                    NormalizedUserName = "VETERINARIO@GMAIL.COM",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(), // Adiciona um SecurityStamp único
                    Formacao = "Medicina Veterinária"
                },
                new Usuario
                {
                    Id = 4,
                    Nome = "Secretário",
                    UserName = "secretario@gmail.com",
                    Email = "secretario@gmail.com",
                    NormalizedEmail = "SECRETARIO@GMAIL.COM",
                    NormalizedUserName = "SECRETARIO@GMAIL.COM",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(), // Adiciona um SecurityStamp único
                    Formacao = "Secretariado"
                },
                new Usuario
                {
                    Id = 5,
                    Nome = "Técnico",
                    UserName = "tecnico@gmail.com",
                    Email = "tecnico@gmail.com",
                    NormalizedEmail = "TECNICO@GMAIL.COM",
                    NormalizedUserName = "TECNICO@GMAIL.COM",
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(), // Adiciona um SecurityStamp único
                    Formacao = "Técnico Veterinário"
                }
            };

            foreach (var usuario in usuarios)
            {
                usuario.PasswordHash = passwordHasher.HashPassword(usuario, "Senha123!"); // Define a senha padrão
            }

            modelBuilder.Entity<Usuario>().HasData(usuarios);

            // Relacionamentos Usuário-Perfil-Clínica
            modelBuilder.Entity<UsuarioPerfilClinica>().HasData(
                new UsuarioPerfilClinica { Id = 1, UsuarioId = 1, PerfilId = 1, ClinicaId = 1, Habilitado = true},
                new UsuarioPerfilClinica { Id = 2, UsuarioId = 1, PerfilId = 1, ClinicaId = 2, Habilitado = true},
                new UsuarioPerfilClinica { Id = 3, UsuarioId = 1, PerfilId = 1, ClinicaId = 3, Habilitado = true},
                new UsuarioPerfilClinica { Id = 4, UsuarioId = 2, PerfilId = 2, ClinicaId = 1, Habilitado = true},
                new UsuarioPerfilClinica { Id = 5, UsuarioId = 2, PerfilId = 2, ClinicaId = 2, Habilitado = true},
                new UsuarioPerfilClinica { Id = 6, UsuarioId = 2, PerfilId = 2, ClinicaId = 3, Habilitado = true},
                new UsuarioPerfilClinica { Id = 7, UsuarioId = 3, PerfilId = 3, ClinicaId = 2, Habilitado = true},
                new UsuarioPerfilClinica { Id = 8, UsuarioId = 4, PerfilId = 4, ClinicaId = 2, Habilitado = true},
                new UsuarioPerfilClinica { Id = 9, UsuarioId = 5, PerfilId = 5, ClinicaId = 3, Habilitado = true}
            );
        }
    }
}
