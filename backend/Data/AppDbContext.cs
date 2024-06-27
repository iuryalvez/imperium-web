using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Perfil> Perfis { get; set; } = null!;
        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; } = null!;
        public DbSet<Animal> Animais { get; set; } = null!;
        public DbSet<Clinica> Clinicas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura a entidade Clinica
            modelBuilder.Entity<Clinica>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nome_clinica)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Nome_fantasia)
                    .HasMaxLength(200);

                entity.Property(e => e.Cnpj)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.Property(e => e.Endereco)
                    .HasMaxLength(300);

                entity.Property(e => e.End_numero)
                    .IsRequired();

                entity.Property(e => e.End_estado)
                    .HasMaxLength(50);

                entity.Property(e => e.End_cidade)
                    .HasMaxLength(100);

                entity.Property(e => e.Cep)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.Telefone1)
                    .IsRequired()
                    .HasMaxLength(13);

                entity.Property(e => e.Whatsapp)
                    .IsRequired()
                    .HasMaxLength(13);

                entity.Property(e => e.Email)
                    .HasMaxLength(200);

                entity.HasMany(e => e.Animais)
                    .WithOne(a => a.Clinica)
                    .HasForeignKey(a => a.ClinicaId);
            });

            // Configura a entidade Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Senha_hash)
                    .IsRequired();

                entity.Property(u => u.Data_cad)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(u => u.Data_ultimoacesso)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(u => u.Habilitado)
                    .IsRequired();

                entity.Property(u => u.Formacao)
                    .HasMaxLength(100);

                // Configura o relacionamento muitos-para-muitos entre Usuario e UsuarioPerfil
                entity.HasMany(u => u.UsuarioPerfis)
                      .WithOne(up => up.Usuario)
                      .HasForeignKey(up => up.UsuarioId);
            });

            // Configura a entidade Perfil
            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(e => e.UsuarioPerfis)
                    .WithOne(up => up.Perfil)
                    .HasForeignKey(up => up.PerfilId);
            });

            // Configura a entidade UsuarioPerfil
            modelBuilder.Entity<UsuarioPerfil>(entity =>
            {
                entity.HasKey(up => new { up.UsuarioId, up.PerfilId });

                entity.HasOne(up => up.Usuario)
                      .WithMany(u => u.UsuarioPerfis)
                      .HasForeignKey(up => up.UsuarioId);

                entity.HasOne(up => up.Perfil)
                      .WithMany(p => p.UsuarioPerfis)
                      .HasForeignKey(up => up.PerfilId);
            });

            // Configura a entidade Animal
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Especie)
                    .IsRequired();

                entity.Property(e => e.Nascimento)
                    .IsRequired();

                entity.Property(e => e.Filhote)
                    .IsRequired();

                entity.Property(e => e.Adulto)
                    .IsRequired();

                entity.Property(e => e.Idoso)
                    .IsRequired();

                entity.Property(e => e.Sexo)
                    .IsRequired();

                entity.Property(e => e.Castrado)
                    .IsRequired();

                entity.Property(e => e.Peso)
                    .IsRequired();

                entity.Property(e => e.Condicao_corporal)
                    .IsRequired();

                entity.Property(e => e.Grau_atividade)
                    .IsRequired();

                entity.HasOne(e => e.Tutor)
                    .WithMany(t => t.Animais)
                    .HasForeignKey(e => e.TutorId);
                
                entity.HasOne(e => e.Clinica)
                    .WithMany(c => c.Animais)
                    .HasForeignKey(e => e.ClinicaId);
            });
        }
    }
}
