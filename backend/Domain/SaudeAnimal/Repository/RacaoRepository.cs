using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.SaudeAnimal.Repository
{
    public class RacaoRepository : IRacaoRepository
    {
        private readonly AppDbContext _context;
        public RacaoRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Racao>> GetAllAvailableByClinicIdAndEspecieAsync(int clinicaId, EspecieAnimal? especie)
        {
            // Rações do sistema (ClinicaId nulo) + rações da própria clínica
            var query = _context.Racoes.AsNoTracking().Where(r => r.ClinicaId == null || r.ClinicaId == clinicaId);

            if (especie.HasValue)
            {
                query = query.Where(r => r.Especie == especie.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Racao?> GetAvailableByIdAndClinicIdAsync(int racaoId, int clinicaId)
        {
            // Instead of a simple FindAsync(id), this method adds a security layer:
            // the feed must be a system feed (ClinicaId null) or belong to the clinic. Feeds of other clinics are never returned.
            return await _context.Racoes
                .FirstOrDefaultAsync(r => r.Id == racaoId && (r.ClinicaId == null || r.ClinicaId == clinicaId));
        }

        public async Task AddAsync(Racao racao) => await _context.Racoes.AddAsync(racao);

        public void Delete(Racao racao) => _context.Racoes.Remove(racao);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}