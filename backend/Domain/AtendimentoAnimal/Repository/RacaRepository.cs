using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class RacaRepository : IRacaRepository
    {
        private readonly AppDbContext _context;

        public RacaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Raca>> GetAllSystemAsync() {
            return await _context.Racas
                .Where(r => r.ClinicaId == null)
                .ToListAsync();
        }

        public async Task<Raca?> GetByIdAsync(int id)
        {
            return await _context.Racas.FindAsync(id);
        }

        public async Task<List<Raca>> ListByClinicaIdAsync(int clinicaId)
        {
            return await _context.Racas
                .Where(r => r.ClinicaId == clinicaId)
                .ToListAsync();
        }

        public async Task AddAsync(Raca raca)
        {
            await _context.Racas.AddAsync(raca);
        }

        public void Update(Raca raca)
        {
            _context.Racas.Update(raca);
        }

        public void Delete(Raca raca)
        {
            _context.Racas.Remove(raca);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}