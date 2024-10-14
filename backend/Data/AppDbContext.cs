using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public class AppDbContext : IdentityDbContext<Usuario>
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
        // public DbSet<Senha> Senhas { get; set; }
        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioPerfilClinica> UsuariosPerfisClinicas { get; set; }
        public DbSet<TutorAnimalClinica> TutoresAnimaisClinicas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // AcompanhamentoInd
            modelBuilder.Entity<AcompanhamentoInd>()
                .HasOne(ai => ai.Animal) 
                .WithOne(a => a.AcompanhamentoInd)
                .HasForeignKey<AcompanhamentoInd>(ai => ai.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AcompanhamentoInd>()
                .HasMany(ai => ai.Consultas)
                .WithOne(c => c.AcompanhamentoInd)
                .HasForeignKey(c => c.AcompanhamentoIndId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AcompanhamentoInd>()
                .HasMany(ai => ai.DietasPersonalizadas)
                .WithOne(dp => dp.AcompanhamentoInd)
                .HasForeignKey(dp => dp.AcompanhamentoIndId)
                .OnDelete(DeleteBehavior.Restrict);                

            // Animal
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.TutoresAnimaisClinicas)
                .WithOne(tac => tac.Animal)
                .HasForeignKey(tac => tac.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Clinica
            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.UsuariosPerfisClinicas)
                .WithOne(upc => upc.Clinica)
                .HasForeignKey(upc => upc.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Clinica>()
                .HasMany(c => c.TutoresAnimaisClinicas)
                .WithOne(tac => tac.Clinica)
                .HasForeignKey(tac => tac.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consulta
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Tutor) // consulta tem um tutor
                .WithMany(t => t.Consultas) // tutor tem muitas consultas
                .HasForeignKey(c => c.TutorId) // consulta tem a chave estrangeira do tutor
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Relatorio)
                .WithOne(r => r.Consulta)
                .HasForeignKey<Relatorio>(r => r.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            // DietaPersonalizada
            modelBuilder.Entity<DietaPersonalizada>()
                .HasOne(dp => dp.Racao)
                .WithOne(r => r.DietaPersonalizada)
                .HasForeignKey<DietaPersonalizada>(dp => dp.RacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Perfil
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
                .HasOne(tac => tac.Clinica) // tac tem um animal
                .WithMany(c => c.TutoresAnimaisClinicas) // um animal tem muitos tac
                .HasForeignKey(tac => tac.ClinicaId) // a chave estrangeira de tac é animalId
                .OnDelete(DeleteBehavior.Restrict);

            List<IdentityRole> roles = new List<IdentityRole> {
                new IdentityRole {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole {
                    Name = "Gestor",
                    NormalizedName = "GESTOR"
                },
                new IdentityRole {
                    Name = "Veterinário",
                    NormalizedName = "VETERINÁRIO"
                },
                new IdentityRole {
                    Name = "Secretário",
                    NormalizedName = "SECRETÁRIO"
                },
                new IdentityRole {
                    Name = "Técnico",
                    NormalizedName = "TÉCNICO"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
