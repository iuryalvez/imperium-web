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

        public async Task<IEnumerable<Tutor>> GetAllByClinicIdWithIncludesAsync(int clinicaId)
        {
            return await _context.Tutores
                // Inclui a tabela de junção Tutor-Animal
                .Include(t => t.TutoresAnimais)
                    // A partir da junção, inclui a entidade Animal
                    .ThenInclude(ta => ta.Animal)
                // Filtra apenas os tutores que têm um vínculo com a clínica especificada
                .Where(t => t.TutoresClinicas.Any(tc => tc.ClinicaId == clinicaId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AllTutorsExistInClinicAsync(List<int> tutorIds, int clinicaId)
        {
            if (tutorIds == null || !tutorIds.Any())
            {
                return true; // Se não há IDs para validar, a validação passa.
            }

            // Conta quantos dos IDs fornecidos existem na tabela de vínculo Tutor-Clínica
            var validTutorsCount = await _context.TutoresClinicas
                .AsNoTracking()
                .CountAsync(tc => tc.ClinicaId == clinicaId && tutorIds.Contains(tc.TutorId));

            // A validação é bem-sucedida se a contagem de tutores válidos for igual ao número
            // de IDs únicos que recebemos.
            return validTutorsCount == tutorIds.Distinct().Count();
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