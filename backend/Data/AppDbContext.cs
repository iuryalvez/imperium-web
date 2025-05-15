using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using backend.Dtos.Clinica;

namespace backend.Data
{
    public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
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
        public DbSet<Racao> Racoes { get; set; }
        public DbSet<Relatorio> Relatorios { get; set; }
        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioPerfilClinica> UsuariosPerfisClinicas { get; set; }
        public DbSet<TutorClinica> TutoresClinicas { get; set; }
        public DbSet<AnimalClinica> AnimaisClinicas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

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
            
            modelBuilder.Entity<Consulta>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Racao>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<DietaPersonalizada>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<AcompanhamentoInd>()
                .Property(entidade => entidade.Id)
                .ValueGeneratedOnAdd();

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

            // 1-N Animal e Clinicas
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.AnimaisClinicas)
                .WithOne(ac => ac.Animal)
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

            // 1-N Clinica e ACs
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.AnimaisClinicas)
                .WithOne(tac => tac.Clinica)
                .HasForeignKey(tac => tac.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Clinica e TCs
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.TutoresClinicas)
                .WithOne(tc => tc.Clinica)
                .HasForeignKey(tc => tc.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-N Tutor e TCs
            modelBuilder.Entity<Tutor>()
                .HasMany(tutores => tutores.TutoresClinicas)
                .WithOne(tc => tc.Tutor)
                .HasForeignKey(tc => tc.TutorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // 1-N Clinica e Racoes
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.Racoes)
                .WithOne(r => r.Clinica)
                .HasForeignKey(r => r.ClinicaId)
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

            // TutorClinica 1:N Tutor
            modelBuilder.Entity<TutorClinica>()
                .HasOne(tc => tc.Tutor) // tc tem um tutor
                .WithMany(tutor => tutor.TutoresClinicas) // um tutor tem muitos upc
                .HasForeignKey(tc => tc.TutorId) // a chave estrangeira de tc é tutorId
                .OnDelete(DeleteBehavior.Restrict);
            
            // TutorClinica 1:N Clinica
            modelBuilder.Entity<TutorClinica>()
                .HasOne(tc => tc.Clinica) // tc tem uma clinica
                .WithMany(clinica => clinica.TutoresClinicas) // uma clinica tem muitos tc
                .HasForeignKey(tc => tc.ClinicaId) // a chave estrangeira de tc é clinicaId
                .OnDelete(DeleteBehavior.Restrict);

            // UsuarioPerfilClinica 1:N Usuario
            modelBuilder.Entity<UsuarioPerfilClinica>()
                .HasOne(upc => upc.Usuario) // upc tem um usuário
                .WithMany(u => u.UsuariosPerfisClinicas) // um usuário tem muitos upc
                .HasForeignKey(upc => upc.UsuarioId) // a chave estrangeira de upc é usuarioId
                .OnDelete(DeleteBehavior.Restrict);

            // UsuarioPerfilClinica 1:N Clinica
            modelBuilder.Entity<UsuarioPerfilClinica>()
                .HasOne(upc => upc.Clinica)
                .WithMany(c => c.UsuariosPerfisClinicas)
                .HasForeignKey(upc => upc.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);
            
            

            DatabaseSeeder.Seed(modelBuilder);

        }
    }
}
            // modelBuilder.Entity<TutorAnimalClinica>()
            //     .Property(tac => tac.Id)
            //     .ValueGeneratedOnAdd();

            // // TutorAnimalClinica 1:N Tutor
            // modelBuilder.Entity<TutorAnimalClinica>()
            //     .HasOne(tac => tac.Tutor) // tac tem um tutor
            //     .WithMany(t => t.TutoresAnimaisClinicas) // um tutor tem muitos tac
            //     .HasForeignKey(tac => tac.TutorId) // a chave estrangeira de tac é tutorId
            //     .OnDelete(DeleteBehavior.Restrict);
            
            // // TutorAnimalClinica 1:N Animal
            // modelBuilder.Entity<TutorAnimalClinica>()
            //     .HasOne(tac => tac.Animal) // tac tem um animal
            //     .WithMany(a => a.TutoresAnimaisClinicas) // um animal tem muitos tac
            //     .HasForeignKey(tac => tac.AnimalId) // a chave estrangeira de tac é animalId
            //     .OnDelete(DeleteBehavior.Restrict);
            
            // // TutorAnimalClinica 1:N Clinica
            // modelBuilder.Entity<TutorAnimalClinica>()
            //     .HasOne(tac => tac.Clinica) // tac tem uma clinica
            //     .WithMany(c => c.TutoresAnimaisClinicas) // uma clinica tem muitos tac
            //     .HasForeignKey(tac => tac.ClinicaId) // a chave estrangeira de tac é clinicaId
            //     .OnDelete(DeleteBehavior.Restrict);
