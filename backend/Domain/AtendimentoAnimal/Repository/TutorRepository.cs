using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interfaces;
using Microsoft.EntityFrameworkCore;
using backend.Infra.Data;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class TutorRepository : ITutorRepository
    {
        private readonly AppDbContext _context;

        public TutorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tutor?> FindByCpfAsync(string cpf)
        {
            return await _context.Tutores.FirstOrDefaultAsync(t => t.Cpf == cpf);
        }

        public async Task<Tutor?> GetByIdAndClinicIdWithIncludesAsync(int tutorId, int clinicaId)
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

        public async Task<bool> AllTutorsExistInClinicAsync(List<int> tutoresIds, int clinicaId)
        {
            if (tutoresIds == null || !tutoresIds.Any())
            {
                return true; // Se não há IDs para validar, a validação passa.
            }

            // Conta quantos dos IDs fornecidos existem na tabela de vínculo Tutor-Clínica
            var validTutorsCount = await _context.TutoresClinicas
                .AsNoTracking()
                .CountAsync(tc => tc.ClinicaId == clinicaId && tutoresIds.Contains(tc.TutorId));

            // A validação é bem-sucedida se a contagem de tutores válidos for igual ao número
            // de IDs únicos que recebemos.
            return validTutorsCount == tutoresIds.Distinct().Count();
        }

        public async Task AddAsync(Tutor tutor)
        {
            await _context.Tutores.AddAsync(tutor);
        }

        public void Remove(Tutor tutor)
        {
            _context.Tutores.Remove(tutor);
        }

        public async Task AddClinicLinkAsync(TutorClinica link)
        {
            await _context.TutoresClinicas.AddAsync(link);
        }

        public async Task<TutorClinica?> GetClinicLinkAsync(int tutorId, int clinicaId)
        {
            return await _context.TutoresClinicas
                .FirstOrDefaultAsync(tc => tc.TutorId == tutorId && tc.ClinicaId == clinicaId);
        }

        public async Task UpdateClinicLinkAsync(TutorClinica link)
        {
            _context.TutoresClinicas.Update(link);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTutorLinkedToClinicAsync(int tutorId, int clinicaId)
        {
            return await _context.TutoresClinicas
                .AsNoTracking()
                .AnyAsync(tc => tc.TutorId == tutorId && tc.ClinicaId == clinicaId);
        }

        public void RemoveClinicLink(TutorClinica link)
        {
            _context.TutoresClinicas.Remove(link);
        }

        public async Task AddAnimalLinkAsync(TutorAnimal link)
        {
            await _context.TutoresAnimais.AddAsync(link);
        }

        public void RemoveAnimalLinks(IEnumerable<TutorAnimal> links)
        {
            _context.TutoresAnimais.RemoveRange(links);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}