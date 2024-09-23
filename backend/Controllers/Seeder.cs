using backend.Data;
using backend.Models;

namespace backend.Controllers {
//     public class Seeder
//     {
//         private readonly AppDbContext _context;

//         public Seeder(AppDbContext context)
//         {
//             _context = context;
//         }

//         public void SeedData()
//         {
//             if (!_context.Perfis.Any())
//             {
//                 var perfis = new List<Perfil>
//                 {
//                     new Perfil { Nome = "Administrador", Descricao = "Perfil de Administrador", Tipo = 1 },
//                     new Perfil { Nome = "Profissional", Descricao = "Perfil de Profissional", Tipo = 2 }
//                 };

//                 _context.Perfis.AddRange(perfis);
//             }

//             if (!_context.Clinicas.Any())
//             {
//                 var clinicas = new List<Clinica>
//                 {
//                     new Clinica { NomeClinica = "Clinica A", NomeFantasia = "Clinica Fantasia A", Cnpj = "12345678901234" },
//                     new Clinica { NomeClinica = "Clinica B", NomeFantasia = "Clinica Fantasia B", Cnpj = "98765432109876" }
//                 };

//                 _context.Clinicas.AddRange(clinicas);
//             }

//             if (!_context.Usuarios.Any())
//             {
//                 var usuarios = new List<Usuario>
//                 {
//                     new Usuario { Nome = "João", Email = "joao@teste.com", Ativo = true },
//                     new Usuario { Nome = "Maria", Email = "maria@teste.com", Ativo = true }
//                 };

//                 _context.Usuarios.AddRange(usuarios);
//             }

//             if (!_context.Tutores.Any())
//             {
//                 var tutores = new List<Tutor>
//                 {
//                     new Tutor { Nome = "José" },
//                     new Tutor { Nome = "Ana" }
//                 };

//                 _context.Tutores.AddRange(tutores);
//             }

//             if (!_context.Animais.Any())
//             {
//                 var animais = new List<Animal>
//                 {
//                     new Animal { Nome = "Rex", Especie = 1 },
//                     new Animal { Nome = "Miau", Especie = 2 }
//                 };

//                 _context.Animais.AddRange(animais);
//             }

//             // Vincular Usuários, Perfis e Clínicas na tabela UsuarioPerfilClinica
//             if (!_context.UsuariosPerfisClinicas.Any())
//             {
//                 var usuariosPerfisClinicas = new List<UsuarioPerfilClinica>
//                 {
//                     new UsuarioPerfilClinica { UsuarioId = 1, PerfilId = 1, ClinicaId = 1 },
//                     new UsuarioPerfilClinica { UsuarioId = 2, PerfilId = 2, ClinicaId = 2 }
//                 };

//                 _context.UsuariosPerfisClinicas.AddRange(usuariosPerfisClinicas);
//             }

//             // Vincular Tutores, Animais e Clínicas na tabela TutorAnimalClinica
//             if (!_context.TutoresAnimaisClinicas.Any())
//             {
//                 var tutoresAnimaisClinicas = new List<TutorAnimalClinica>
//                 {
//                     new TutorAnimalClinica { TutorId = 1, AnimalId = 1, ClinicaId = 1 },
//                     new TutorAnimalClinica { TutorId = 2, AnimalId = 2, ClinicaId = 2 }
//                 };

//                 _context.TutoresAnimaisClinicas.AddRange(tutoresAnimaisClinicas);
//             }

//             _context.SaveChanges();
//         }
//     }
}
