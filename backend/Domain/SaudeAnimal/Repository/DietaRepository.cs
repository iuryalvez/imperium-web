using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.SaudeAnimal.Repository
{
    public class DietaRepository : IDietaRepository
    {
        private readonly AppDbContext _context;

        public DietaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dieta?> GetByAvaliacaoIdAsync(int avaliacaoId)
        {
            return await _context.Dietas
                .Include(d => d.Racao)
                .Include(d => d.Avaliacao)
                .FirstOrDefaultAsync(d => d.AvaliacaoId == avaliacaoId);
        }

        public async Task<Dieta?> FindByAvaliacaoIdAsync(int avaliacaoId)
        {
            // This is a specific query to support the "update-or-insert" (upsert) logic.
            // It checks for the existence of a diet based on its foreign key relationship with the report.
            return await _context.Dietas
                .FirstOrDefaultAsync(d => d.AvaliacaoId == avaliacaoId);
        }

        public async Task<Dieta?> FindEmAndamentoByAcompanhamentoIdAsync(int acompanhamentoId)
        {
            return await _context.Dietas
                .FirstOrDefaultAsync(d => d.Avaliacao!.AcompanhamentoId == acompanhamentoId && d.Status == backend.Domain.SaudeAnimal.Enums.StatusDieta.EmAndamento);
        }

        public async Task<IEnumerable<Dieta>> GetAllByAcompanhamentoIdAsync(int acompanhamentoId)
        {
            // Busca todas as dietas cujos relatórios pertencem ao acompanhamento especificado
            return await _context.Dietas
                .Include(d => d.Racao) // Inclui a ração para exibir o nome
                .Where(d => d.Avaliacao!.AcompanhamentoId == acompanhamentoId)
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();
        }

        public async Task<Dieta?> GetByIdWithIncludesAsync(int dietaId)
        {
            // Inclui a cadeia de navegação para permitir a validação de segurança no serviço
            // Dieta -> Avaliacao -> Consulta -> ClinicaId
            return await _context.Dietas
                .Include(d => d.Racao)
                .Include(d => d.RegistrosPeso)
                .Include(d => d.Avaliacao)
                    .ThenInclude(r => r.Consulta)
                        .ThenInclude(c => c.Animal)
                .FirstOrDefaultAsync(d => d.Id == dietaId);
        }

        public async Task AddAsync(Dieta dieta)
        {
            await _context.Dietas.AddAsync(dieta);
        }

        public async Task UpdateAsync(Dieta dieta)
        {
            _context.Dietas.Update(dieta);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Delete(Dieta dieta)
        {
            _context.Dietas.Remove(dieta);
        }

        public async Task AddRegistroPesoAsync(RegistroPesoDieta registro)
        {
            await _context.RegistrosPesoDieta.AddAsync(registro);
        }

        public void RemoveRegistroPeso(RegistroPesoDieta registro)
        {
            _context.RegistrosPesoDieta.Remove(registro);
        }
    }
}