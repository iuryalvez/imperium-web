using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.SaudeAnimal.Repository
{
    public class RacaoRepository : IRacaoRepository
    {
        private readonly AppDbContext _context;
        public RacaoRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Racao>> GetAllByClinicIdAsync(int clinicaId, EspecieAnimal? especie)
        {
            var query = _context.Racoes.AsNoTracking().Where(r => r.ClinicaId == clinicaId);

            if (especie.HasValue)
            {
                query = query.Where(r => r.Especie == especie.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Racao?> GetByIdAndClinicIdAsync(int racaoId, int clinicaId) =>
            await _context.Racoes.FirstOrDefaultAsync(r => r.Id == racaoId && r.ClinicaId == clinicaId);

        public async Task AddAsync(Racao racao) => await _context.Racoes.AddAsync(racao);

        public void Delete(Racao racao) => _context.Racoes.Remove(racao);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}