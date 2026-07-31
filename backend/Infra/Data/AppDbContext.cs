using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using backend.Domain.Usuarios.Entities;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Infra.Data
{
    public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Acompanhamento> AcompanhamentosInds { get; set; }
        public DbSet<Animal> Animais { get; set; }
        public DbSet<Clinica> Clinicas { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<DietaPersonalizada> DietasPersonalizadas { get; set; }
        public DbSet<Racao> Racoes { get; set; }
        public DbSet<Relatorio> Relatorios { get; set; }
        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioPerfilClinica> UsuariosPerfisClinicas { get; set; }
        public DbSet<TutorClinica> TutoresClinicas { get; set; }
        public DbSet<AnimalClinica> AnimaisClinicas { get; set; }
        public DbSet<TutorAnimal> TutoresAnimais { get; set; }
        public DbSet<Raca> Racas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<IdentityRole>();
            modelBuilder.Ignore<IdentityUserRole<string>>();
            modelBuilder.Ignore<IdentityUserClaim<string>>();
            modelBuilder.Ignore<IdentityUserLogin<string>>();
            modelBuilder.Ignore<IdentityUserToken<string>>();

            modelBuilder.Entity<Usuario>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Clinica>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UsuarioPerfilClinica>()
                .Property(upc => upc.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Animal>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AnimalClinica>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Tutor>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TutorClinica>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TutorAnimal>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Consulta>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Racao>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<DietaPersonalizada>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Acompanhamento>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Raca>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            // AcompanhamentoInd 1:1 Animal
            modelBuilder.Entity<Acompanhamento>()
                .HasOne(ai => ai.Animal)
                .WithOne(a => a.Acompanhamento)
                .HasForeignKey<Acompanhamento>(ai => ai.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // AcompanhamentoInd 1:N Relatorio
            modelBuilder.Entity<Acompanhamento>()
                .HasMany(ai => ai.Relatorios)
                .WithOne(r => r.Acompanhamento)
                .HasForeignKey(r => r.AcompanhamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relatório 1:1 Dieta
            modelBuilder.Entity<Relatorio>()
                .HasOne(r => r.DietaPersonalizada)
                .WithOne(d => d.Relatorio)
                .HasForeignKey<DietaPersonalizada>(dp => dp.RelatorioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Animal 1:N AnimalClinica
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.AnimaisClinicas)
                .WithOne(ac => ac.Animal)
                .HasForeignKey(ac => ac.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Animal 1:N TutorAnimal
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.TutoresAnimais)
                .WithOne(ta => ta.Animal)
                .HasForeignKey(ta => ta.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Animal 1:N Consulta
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.Consultas)
                .WithOne(c => c.Animal)
                .HasForeignKey(c => c.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Raca 1:N Animal
            modelBuilder.Entity<Raca>()
                .HasMany(r => r.Animais)
                .WithOne(a => a.Raca)
                .HasForeignKey(a => a.RacaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Clinica 1:N Racao
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.Racoes)
                .WithOne(r => r.Clinica)
                .HasForeignKey(r => r.ClinicaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Clinica 1:N Raca
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.Racas)
                .WithOne(r => r.Clinica)
                .HasForeignKey(r => r.ClinicaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Clinica 1:N Consulta
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.Consultas)
                .WithOne(con => con.Clinica)
                .HasForeignKey(con => con.ClinicaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Clinica 1:N TutorClinica
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.TutoresClinicas)
                .WithOne(tc => tc.Clinica)
                .HasForeignKey(tc => tc.ClinicaId)
                .OnDelete(DeleteBehavior.Cascade);


            // Clinica 1:N UsuarioPerfilClinica
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.UsuariosPerfisClinicas)
                .WithOne(upc => upc.Clinica)
                .HasForeignKey(upc => upc.ClinicaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Consulta 1:1 Relatorio
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Relatorio)
                .WithOne(r => r.Consulta)
                .HasForeignKey<Relatorio>(r => r.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade); // Correct: a report is meaningless without its consultation.

            // Racao 1:N DietaPersonalizada
            modelBuilder.Entity<Racao>()
                .HasMany(r => r.DietasPersonalizadas)
                .WithOne(dp => dp.Racao)
                .HasForeignKey(dp => dp.RacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Racao 1:N Relatorio
            modelBuilder.Entity<Racao>()
                .HasMany(r => r.Relatorios)
                .WithOne(rel => rel.Racao)
                .HasForeignKey(rel => rel.RacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tutor 1:N Consulta
            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.Consultas)
                .WithOne(c => c.Tutor)
                .HasForeignKey(c => c.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tutor 1:N TutorAnimal
            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.TutoresAnimais)
                .WithOne(ta => ta.Tutor)
                .HasForeignKey(ta => ta.TutorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tutor 1:N TutorClinica
            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.TutoresClinicas)
                .WithOne(tc => tc.Tutor)
                .HasForeignKey(tc => tc.TutorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario 1:N Consulta
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Consultas)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario 1:N UsuarioPerfilClinica
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.UsuariosPerfisClinicas)
                .WithOne(upc => upc.Usuario)
                .HasForeignKey(upc => upc.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            DatabaseSeeder.Seed(modelBuilder);

        }
    }
}
