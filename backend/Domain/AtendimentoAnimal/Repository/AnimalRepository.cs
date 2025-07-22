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

        /// <summary>
        /// Busca um animal pelo ID, mas apenas se ele estiver vinculado à clínica especificada.
        /// Inclui os vínculos com tutores (TutoresAnimais) para a lógica de sincronização.
        /// </summary>
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

        /// <summary>
        /// Adiciona um novo vínculo Tutor-Animal ao contexto do EF.
        /// </summary>
        public async Task AddTutorLinkAsync(TutorAnimal link)
        {
            await _context.TutoresAnimais.AddAsync(link);
        }

        /// <summary>
        /// Marca uma coleção de vínculos Tutor-Animal para remoção.
        /// </summary>
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

        /// <summary>
        /// Salva todas as alterações pendentes no banco de dados.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}