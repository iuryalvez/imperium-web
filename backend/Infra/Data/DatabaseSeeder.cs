using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.AtendimentoAnimal.Entities;

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

            modelBuilder.Entity<Raca>().HasData(

                // --- Raças de Cães (EspecieAnimal.Cao) ---
                new Raca { Id = 1, Nome = "Affenpinscher", Especie = EspecieAnimal.Cao },
                new Raca { Id = 2, Nome = "Afghan Hound", Especie = EspecieAnimal.Cao },
                new Raca { Id = 3, Nome = "Airedale Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 4, Nome = "Akita", Especie = EspecieAnimal.Cao },
                new Raca { Id = 5, Nome = "Alaskan Malamute", Especie = EspecieAnimal.Cao },
                new Raca { Id = 6, Nome = "American Staffordshire Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 7, Nome = "Australian Shepherd", Especie = EspecieAnimal.Cao },
                new Raca { Id = 8, Nome = "Basset Hound", Especie = EspecieAnimal.Cao },
                new Raca { Id = 9, Nome = "Beagle", Especie = EspecieAnimal.Cao },
                new Raca { Id = 10, Nome = "Bernese Mountain Dog", Especie = EspecieAnimal.Cao },
                new Raca { Id = 11, Nome = "Bichon Frise", Especie = EspecieAnimal.Cao },
                new Raca { Id = 12, Nome = "Border Collie", Especie = EspecieAnimal.Cao },
                new Raca { Id = 13, Nome = "Boston Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 14, Nome = "Boxer", Especie = EspecieAnimal.Cao },
                new Raca { Id = 15, Nome = "Bulldog", Especie = EspecieAnimal.Cao },
                new Raca { Id = 16, Nome = "Bull Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 17, Nome = "Cavalier King Charles Spaniel", Especie = EspecieAnimal.Cao },
                new Raca { Id = 18, Nome = "Chihuahua", Especie = EspecieAnimal.Cao },
                new Raca { Id = 19, Nome = "Chow Chow", Especie = EspecieAnimal.Cao },
                new Raca { Id = 20, Nome = "Cocker Spaniel", Especie = EspecieAnimal.Cao },
                new Raca { Id = 21, Nome = "Collie", Especie = EspecieAnimal.Cao },
                new Raca { Id = 22, Nome = "Dachshund", Especie = EspecieAnimal.Cao },
                new Raca { Id = 23, Nome = "Dalmatian", Especie = EspecieAnimal.Cao },
                new Raca { Id = 24, Nome = "Doberman Pinscher", Especie = EspecieAnimal.Cao },
                new Raca { Id = 25, Nome = "English Springer Spaniel", Especie = EspecieAnimal.Cao },
                new Raca { Id = 26, Nome = "French Bulldog", Especie = EspecieAnimal.Cao },
                new Raca { Id = 27, Nome = "German Shepherd", Especie = EspecieAnimal.Cao },
                new Raca { Id = 28, Nome = "Golden Retriever", Especie = EspecieAnimal.Cao },
                new Raca { Id = 29, Nome = "Great Dane", Especie = EspecieAnimal.Cao },
                new Raca { Id = 30, Nome = "Irish Setter", Especie = EspecieAnimal.Cao },
                new Raca { Id = 31, Nome = "Jack Russell Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 32, Nome = "Labrador Retriever", Especie = EspecieAnimal.Cao },
                new Raca { Id = 33, Nome = "Lhasa Apso", Especie = EspecieAnimal.Cao },
                new Raca { Id = 34, Nome = "Maltese", Especie = EspecieAnimal.Cao },
                new Raca { Id = 35, Nome = "Mastiff", Especie = EspecieAnimal.Cao },
                new Raca { Id = 36, Nome = "Miniature Schnauzer", Especie = EspecieAnimal.Cao },
                new Raca { Id = 37, Nome = "Newfoundland", Especie = EspecieAnimal.Cao },
                new Raca { Id = 38, Nome = "Pomeranian", Especie = EspecieAnimal.Cao },
                new Raca { Id = 39, Nome = "Poodle", Especie = EspecieAnimal.Cao },
                new Raca { Id = 40, Nome = "Pug", Especie = EspecieAnimal.Cao },
                new Raca { Id = 41, Nome = "Rottweiler", Especie = EspecieAnimal.Cao },
                new Raca { Id = 42, Nome = "Saint Bernard", Especie = EspecieAnimal.Cao },
                new Raca { Id = 43, Nome = "Samoyed", Especie = EspecieAnimal.Cao },
                new Raca { Id = 44, Nome = "Scottish Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 45, Nome = "Shetland Sheepdog", Especie = EspecieAnimal.Cao },
                new Raca { Id = 46, Nome = "Shiba Inu", Especie = EspecieAnimal.Cao },
                new Raca { Id = 47, Nome = "Shih Tzu", Especie = EspecieAnimal.Cao },
                new Raca { Id = 48, Nome = "Siberian Husky", Especie = EspecieAnimal.Cao },
                new Raca { Id = 49, Nome = "Weimaraner", Especie = EspecieAnimal.Cao },
                new Raca { Id = 50, Nome = "Welsh Corgi (Cardigan)", Especie = EspecieAnimal.Cao },
                new Raca { Id = 51, Nome = "Welsh Corgi (Pembroke)", Especie = EspecieAnimal.Cao },
                new Raca { Id = 52, Nome = "West Highland White Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 53, Nome = "Whippet", Especie = EspecieAnimal.Cao },
                new Raca { Id = 54, Nome = "Yorkshire Terrier", Especie = EspecieAnimal.Cao },
                new Raca { Id = 55, Nome = "Sem Raça Definida (SRD)", Especie = EspecieAnimal.Cao },

                // --- Raças de Gatos (EspecieAnimal.Gato) ---
                new Raca { Id = 101, Nome = "Abyssinian", Especie = EspecieAnimal.Gato },
                new Raca { Id = 102, Nome = "American Bobtail", Especie = EspecieAnimal.Gato },
                new Raca { Id = 103, Nome = "American Curl", Especie = EspecieAnimal.Gato },
                new Raca { Id = 104, Nome = "American Shorthair", Especie = EspecieAnimal.Gato },
                new Raca { Id = 105, Nome = "American Wirehair", Especie = EspecieAnimal.Gato },
                new Raca { Id = 106, Nome = "Balinese", Especie = EspecieAnimal.Gato },
                new Raca { Id = 107, Nome = "Bengal", Especie = EspecieAnimal.Gato },
                new Raca { Id = 108, Nome = "Birman", Especie = EspecieAnimal.Gato },
                new Raca { Id = 109, Nome = "Bombay", Especie = EspecieAnimal.Gato },
                new Raca { Id = 110, Nome = "British Shorthair", Especie = EspecieAnimal.Gato },
                new Raca { Id = 111, Nome = "Burmese", Especie = EspecieAnimal.Gato },
                new Raca { Id = 112, Nome = "Chartreux", Especie = EspecieAnimal.Gato },
                new Raca { Id = 113, Nome = "Cornish Rex", Especie = EspecieAnimal.Gato },
                new Raca { Id = 114, Nome = "Devon Rex", Especie = EspecieAnimal.Gato },
                new Raca { Id = 115, Nome = "Egyptian Mau", Especie = EspecieAnimal.Gato },
                new Raca { Id = 116, Nome = "Exotic Shorthair", Especie = EspecieAnimal.Gato },
                new Raca { Id = 117, Nome = "Havana Brown", Especie = EspecieAnimal.Gato },
                new Raca { Id = 118, Nome = "Himalayan", Especie = EspecieAnimal.Gato },
                new Raca { Id = 119, Nome = "Japanese Bobtail", Especie = EspecieAnimal.Gato },
                new Raca { Id = 120, Nome = "Korat", Especie = EspecieAnimal.Gato },
                new Raca { Id = 121, Nome = "LaPerm", Especie = EspecieAnimal.Gato },
                new Raca { Id = 122, Nome = "Maine Coon", Especie = EspecieAnimal.Gato },
                new Raca { Id = 123, Nome = "Manx", Especie = EspecieAnimal.Gato },
                new Raca { Id = 124, Nome = "Norwegian Forest Cat", Especie = EspecieAnimal.Gato },
                new Raca { Id = 125, Nome = "Ocicat", Especie = EspecieAnimal.Gato },
                new Raca { Id = 126, Nome = "Oriental", Especie = EspecieAnimal.Gato },
                new Raca { Id = 127, Nome = "Persian", Especie = EspecieAnimal.Gato },
                new Raca { Id = 128, Nome = "Ragamuffin", Especie = EspecieAnimal.Gato },
                new Raca { Id = 129, Nome = "Ragdoll", Especie = EspecieAnimal.Gato },
                new Raca { Id = 130, Nome = "Russian Blue", Especie = EspecieAnimal.Gato },
                new Raca { Id = 131, Nome = "Scottish Fold", Especie = EspecieAnimal.Gato },
                new Raca { Id = 132, Nome = "Selkirk Rex", Especie = EspecieAnimal.Gato },
                new Raca { Id = 133, Nome = "Siamese", Especie = EspecieAnimal.Gato },
                new Raca { Id = 134, Nome = "Siberian", Especie = EspecieAnimal.Gato },
                new Raca { Id = 135, Nome = "Singapura", Especie = EspecieAnimal.Gato },
                new Raca { Id = 136, Nome = "Somali", Especie = EspecieAnimal.Gato },
                new Raca { Id = 137, Nome = "Sphynx", Especie = EspecieAnimal.Gato },
                new Raca { Id = 138, Nome = "Tonkinese", Especie = EspecieAnimal.Gato },
                new Raca { Id = 139, Nome = "Turkish Angora", Especie = EspecieAnimal.Gato },
                new Raca { Id = 140, Nome = "Turkish Van", Especie = EspecieAnimal.Gato },
                new Raca { Id = 141, Nome = "Sem Raça Definida (SRD)", Especie = EspecieAnimal.Gato }
            );
        }
    }
}
