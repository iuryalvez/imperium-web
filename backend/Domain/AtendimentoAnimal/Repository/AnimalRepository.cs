using Microsoft.EntityFrameworkCore;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;

namespace backend.Infra.Data.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly AppDbContext _context;

        public AnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Animal?> GetByIdWithIncludesAsync(int animalId, int clinicaId)
        {
            return await _context.Animais
                // Inclui a coleção de vínculos com tutores na consulta
                .Include(a => a.TutoresAnimais)
                // Garante que o animal pertence à clínica especificada
                .Where(a => a.AnimaisClinicas.Any(ac => ac.ClinicaId == clinicaId))
                // Busca o animal específico pelo ID
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        public async Task AddTutorLinkAsync(TutorAnimal link)
        {
            await _context.TutoresAnimais.AddAsync(link);
        }

        public async Task<bool> AllAnimalsExistInClinicAsync(List<int> animalIds, int clinicaId)
        {
            if (animalIds == null || !animalIds.Any())
            {
                return true; // Lista vazia é considerada válida
            }

            // Conta quantos dos IDs fornecidos existem na tabela de vínculo Animal-Clínica
            var validAnimalsCount = await _context.AnimaisClinicas
                .AsNoTracking()
                .CountAsync(ac => ac.ClinicaId == clinicaId && animalIds.Contains(ac.AnimalId));

            // A validação passa se a contagem for igual ao número de IDs únicos que recebemos.
            return validAnimalsCount == animalIds.Distinct().Count();
        }

        public void RemoveTutorLinks(IEnumerable<TutorAnimal> links)
        {
            _context.TutoresAnimais.RemoveRange(links);
        }

        public async Task<IEnumerable<Animal>> GetAnimaisByClinicIdWithIncludesAsync(int clinicaId)
        {
            return await _context.Animais
                .Include(a => a.TutoresAnimais).ThenInclude(ta => ta.Tutor)
                .Include(a => a.Raca) // Incluindo a Raça
                .Where(a => a.AnimaisClinicas.Any(ac => ac.ClinicaId == clinicaId))
                .ToListAsync();
        }

        public async Task<Animal?> GetAnimalDetailsAsync(int animalId, int clinicaId)
        {
            return await _context.Animais
                .Include(a => a.TutoresAnimais).ThenInclude(ta => ta.Tutor)
                .Include(a => a.Raca)
                .Where(a => a.AnimaisClinicas.Any(ac => ac.ClinicaId == clinicaId))
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        public async Task AddAsync(Animal animal)
        {
            await _context.Animais.AddAsync(animal);
        }

        public async Task AddVinculoClinicaAsync(AnimalClinica vinculo)
        {
            await _context.AnimaisClinicas.AddAsync(vinculo);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}