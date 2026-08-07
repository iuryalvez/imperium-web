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

        public async Task<Animal?> GetByIdAsync(int animalId)
        {
            return await _context.Animais
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        public async Task<Animal?> GetByIdAndClinicaIdWithIncludesAsync(int animalId, int clinicaId)
        {
            return await _context.Animais
                // Inclui a coleção de vínculos com tutores na consulta
                .Include(a => a.TutoresAnimais)
                // Garante que o animal pertence à clínica especificada
                .Where(a => a.AnimaisClinicas!.Any(ac => ac.ClinicaId == clinicaId))
                // Busca o animal específico pelo ID
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        public async Task<IEnumerable<Animal>> GetAllByClinicaIdWithDetailsAsync(int clinicaId)
        {
            return await _context.Animais
                .Include(a => a.TutoresAnimais!).ThenInclude(ta => ta.Tutor)
                .Include(a => a.Acompanhamento) // Incluindo o Acompanhamento Individualizado
                .Include(a => a.Raca) // Incluindo a Raça
                .Where(a => a.AnimaisClinicas!.Any(ac => ac.ClinicaId == clinicaId))
                .ToListAsync();
        }

        public async Task<bool> IsAnimalInClinicAsync(int animalId, int clinicaId)
        {
            // Verifica se existe um vínculo ATIVO entre o animal e a clínica
            return await _context.AnimaisClinicas
                .AnyAsync(ac =>
                    ac.AnimalId == animalId &&
                    ac.ClinicaId == clinicaId &&
                    ac.IsAtivo);
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

        public async Task<Animal?> GetHistoricoCompletoAsync(int animalId, int clinicaId)
        {
            return await _context.Animais
                // Carrega dados diretos do animal
                .Include(a => a.Raca)

                // Carrega os tutores através da tabela de junção
                .Include(a => a.TutoresAnimais!)
                    .ThenInclude(ta => ta.Tutor)

                // Carrega o acompanhamento
                .Include(a => a.Acompanhamento)

                // Carrega as consultas do animal (a linha do tempo)
                .Include(a => a.Consultas!.OrderByDescending(c => c.DataConsulta))
                    .ThenInclude(c => c.Avaliacao)
                        .ThenInclude(av => av!.Dieta)
                            .ThenInclude(d => d!.Racao)

                // A partir de cada consulta, carrega o usuário responsável
                .Include(a => a.Consultas)
                    .ThenInclude(c => c.Usuario)

                // Garante que estamos pegando o animal correto da clínica correta
                .Where(a => a.Id == animalId && a.AnimaisClinicas!.Any(ac => ac.ClinicaId == clinicaId))
                .FirstOrDefaultAsync();
        }

        public async Task<Animal?> GetByIdAndClinicIdWithDetailsAsync(int animalId, int clinicaId)
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

        public async Task<AnimalClinica?> GetVinculoAnimalClinicaAsync(int animalId, int clinicaId)
        {
            return await _context.AnimaisClinicas
                .FirstOrDefaultAsync(ac => ac.AnimalId == animalId && ac.ClinicaId == clinicaId);
        }

        public void UpdateVinculo(AnimalClinica vinculo)
        {
            _context.AnimaisClinicas.Update(vinculo);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}