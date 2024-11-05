using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : IdentityDbContext<Usuario,Perfil,int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<AcompanhamentoInd> AcompanhamentosInds { get; set; }
        public DbSet<Animal> Animais { get; set; }
        public DbSet<Clinica> Clinicas { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<DietaPersonalizada> DietasPersonalizadas { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<Racao> Racoes { get; set; }
        public DbSet<Relatorio> Relatorios { get; set; }
        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioPerfilClinica> UsuariosPerfisClinicas { get; set; }
        public DbSet<TutorAnimalClinica> TutoresAnimaisClinicas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // 1-1 Acompanhamento e Animal
            modelBuilder.Entity<AcompanhamentoInd>()
                .HasOne(ai => ai.Animal) 
                .WithOne(a => a.AcompanhamentoInd)
                .HasForeignKey<AcompanhamentoInd>(ai => ai.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Acompanhamento e Consultas
            modelBuilder.Entity<AcompanhamentoInd>()
                .HasMany(ai => ai.Relatorios)
                .WithOne(c => c.AcompanhamentoInd)
                .HasForeignKey(c => c.AcompanhamentoIndId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Acompanhamento e Dietas
            modelBuilder.Entity<AcompanhamentoInd>()
                .HasMany(ai => ai.DietasPersonalizadas)
                .WithOne(dp => dp.AcompanhamentoInd)
                .HasForeignKey(dp => dp.AcompanhamentoIndId)
                .OnDelete(DeleteBehavior.Restrict);                

            // 1-N Animal e TACs
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.TutoresAnimaisClinicas)
                .WithOne(tac => tac.Animal)
                .HasForeignKey(tac => tac.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Clinica e Consultas
            modelBuilder.Entity<Clinica>()
                .HasMany(cli => cli.Consultas)
                .WithOne(con => con.Clinica)
                .HasForeignKey(con => con.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Clinica e UPCs
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.UsuariosPerfisClinicas)
                .WithOne(upc => upc.Clinica)
                .HasForeignKey(upc => upc.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Clinica e TACs
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.TutoresAnimaisClinicas)
                .WithOne(tac => tac.Clinica)
                .HasForeignKey(tac => tac.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            // N-1 Consultas e Usuario
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Consultas)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // N-1 Consultas e Tutor
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Tutor) // consulta tem um tutor
                .WithMany(t => t.Consultas) // tutor tem muitas consultas
                .HasForeignKey(c => c.TutorId) // consulta tem a chave estrangeira do tutor
                .OnDelete(DeleteBehavior.Restrict);

            // 1-1 Consulta e Relatorio
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Relatorio)
                .WithOne(r => r.Consulta)
                .HasForeignKey<Relatorio>(r => r.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-1 Dieta e Racao
            modelBuilder.Entity<DietaPersonalizada>()
                .HasOne(dp => dp.Racao)
                .WithOne(r => r.DietaPersonalizada)
                .HasForeignKey<DietaPersonalizada>(dp => dp.RacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Perfil e UPCs
            modelBuilder.Entity<Perfil>()
                .HasMany(p => p.UsuariosPerfisClinicas)
                .WithOne(upc => upc.Perfil)
                .HasForeignKey(upc => upc.PerfilId)
                .OnDelete(DeleteBehavior.Restrict);

            // UsuarioPerfilClinica - Chave composta para tabela de associação
            modelBuilder.Entity<UsuarioPerfilClinica>()
                .HasKey(upc => new { upc.UsuarioId, upc.PerfilId, upc.ClinicaId }); // Chave composta
            
            // UsuarioPerfilClinica 1:N Usuario
            modelBuilder.Entity<UsuarioPerfilClinica>()
                .HasOne(upc => upc.Usuario) // upc tem um usuário
                .WithMany(u => u.UsuariosPerfisClinicas) // um usuário tem muitos upc
                .HasForeignKey(upc => upc.UsuarioId) // a chave estrangeira de upc é usuarioId
                .OnDelete(DeleteBehavior.Restrict);

            // UsuarioPerfilClinica 1:N Perfil
            modelBuilder.Entity<UsuarioPerfilClinica>()
                .HasOne(upc => upc.Perfil)
                .WithMany(p => p.UsuariosPerfisClinicas)
                .HasForeignKey(upc => upc.PerfilId)
                .OnDelete(DeleteBehavior.Restrict);

            // UsuarioPerfilClinica 1:N Clinica
            modelBuilder.Entity<UsuarioPerfilClinica>()
                .HasOne(upc => upc.Clinica)
                .WithMany(c => c.UsuariosPerfisClinicas)
                .HasForeignKey(upc => upc.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            // TutorAnimalClinica - Chave composta para tabela de associação
            modelBuilder.Entity<TutorAnimalClinica>()
                .HasKey(tac => new { tac.TutorId, tac.AnimalId, tac.ClinicaId }); // Chave composta
            
            // TutorAnimalClinica 1:N Tutor
            modelBuilder.Entity<TutorAnimalClinica>()
                .HasOne(tac => tac.Tutor) // tac tem um tutor
                .WithMany(t => t.TutoresAnimaisClinicas) // um tutor tem muitos tac
                .HasForeignKey(tac => tac.TutorId) // a chave estrangeira de tac é tutorId
                .OnDelete(DeleteBehavior.Restrict);
            
            // TutorAnimalClinica 1:N Animal
            modelBuilder.Entity<TutorAnimalClinica>()
                .HasOne(tac => tac.Animal) // tac tem um animal
                .WithMany(a => a.TutoresAnimaisClinicas) // um animal tem muitos tac
                .HasForeignKey(tac => tac.AnimalId) // a chave estrangeira de tac é animalId
                .OnDelete(DeleteBehavior.Restrict);
            
            // TutorAnimalClinica 1:N Clinica
            modelBuilder.Entity<TutorAnimalClinica>()
                .HasOne(tac => tac.Clinica) // tac tem uma consulta
                .WithMany(c => c.TutoresAnimaisClinicas) // uma consulta tem muitos tac
                .HasForeignKey(tac => tac.ClinicaId) // a chave estrangeira de tac é clinicaId
                .OnDelete(DeleteBehavior.Restrict);

            // Inserção de dados iniciais para Perfil
            modelBuilder.Entity<Perfil>().HasData(
                new Perfil { Id = 1, Name = "ADMIN", Tipo = 1, Descricao = "Administrador do sistema" },
                new Perfil { Id = 2, Name = "GESTOR", Tipo = 2, Descricao = "Gestor de clínica" },
                new Perfil { Id = 3, Name = "VETERINARIO", Tipo = 3, Descricao = "Veterinário de clínica" },
                new Perfil { Id = 4, Name = "SECRETARIO", Tipo = 4, Descricao = "Secretario de clínica" },
                new Perfil { Id = 5, Name = "TECNICO", Tipo = 5, Descricao = "Tecnico de clinica" },
                new Perfil { Id = 98, Name = "PENDENTE", Tipo = 98, Descricao = "Usuário pendente de ativação" },
                new Perfil { Id = 99, Name = "INATIVO", Tipo = 99, Descricao = "Usuário inativo" }
            );

            // Dados iniciais para Clinica
            // modelBuilder.Entity<Clinica>().HasData(
            //     new Clinica { Id = 1, NomeFantasia = "Clinica Teste", NomeClinica = "Clinica Animais", Cnpj = "00000000000100"}
            // );

        }
    }
}
