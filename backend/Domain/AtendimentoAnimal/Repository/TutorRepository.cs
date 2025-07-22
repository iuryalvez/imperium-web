using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Infra.Data.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly AppDbContext _context;

        public TutorRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca um tutor pelo ID, mas apenas se ele estiver vinculado à clínica especificada.
        /// Inclui os vínculos com animais (TutoresAnimais) para a lógica de sincronização.
        /// </summary>
        public async Task<Tutor?> GetByIdWithIncludesAsync(int tutorId, int clinicaId)
        {
            return await _context.Tutores
                // Inclui a coleção de vínculos com animais na consulta
                .Include(t => t.TutoresAnimais) 
                // Garante que o tutor pertence à clínica especificada
                .Where(t => t.TutoresClinicas.Any(tc => tc.ClinicaId == clinicaId))
                // Busca o tutor específico pelo ID
                .FirstOrDefaultAsync(t => t.Id == tutorId);
        }

        /// <summary>
        /// Adiciona um novo vínculo Tutor-Animal ao contexto do EF.
        /// </summary>
        public async Task AddAnimalLinkAsync(TutorAnimal link)
        {
            await _context.TutoresAnimais.AddAsync(link);
        }

        /// <summary>
        /// Marca uma coleção de vínculos Tutor-Animal para remoção.
        /// </summary>
        public void RemoveAnimalLinks(IEnumerable<TutorAnimal> links)
        {
            _context.TutoresAnimais.RemoveRange(links);
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