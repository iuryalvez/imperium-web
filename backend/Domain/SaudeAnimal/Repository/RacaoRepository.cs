using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.Utils.Pagination;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.SaudeAnimal.Repository
{
    public class RacaoRepository : IRacaoRepository
    {
        private readonly AppDbContext _context;
        public RacaoRepository(AppDbContext context) { _context = context; }

        public async Task<PagedResponse<Racao>> GetAllByClinicIdAndEspecieAsync(int clinicaId, EspecieAnimal? especie, int page, int pageSize, string? searchNome)
        {
            var query = _context.Racoes.AsNoTracking().Where(r => r.ClinicaId == clinicaId);

            if (especie.HasValue)
            {
                query = query.Where(r => r.Especie == especie.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchNome))
            {
                query = query.Where(r => r.Nome != null && r.Nome.ToLower().Contains(searchNome.ToLower()));
            }

            query = query.OrderBy(r => r.Nome);

            return await query.ToPagedResponseAsync(page, pageSize);
        }

        public async Task<Racao?> GetByIdAndClinicaIdAsync(int racaoId, int clinicaId)
        {
            // Instead of a simple FindAsync(id), this method adds a security layer,
            // ensuring that the retrieved feed belongs to the correct clinic.
            return await _context.Racoes
                .FirstOrDefaultAsync(r => r.Id == racaoId && r.ClinicaId == clinicaId);
        }

        public async Task<Racao?> GetByIdAndClinicIdAsync(int racaoId, int clinicaId) =>
            await _context.Racoes.FirstOrDefaultAsync(r => r.Id == racaoId && r.ClinicaId == clinicaId);

        public async Task AddAsync(Racao racao) => await _context.Racoes.AddAsync(racao);

        public void Delete(Racao racao) => _context.Racoes.Remove(racao);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}