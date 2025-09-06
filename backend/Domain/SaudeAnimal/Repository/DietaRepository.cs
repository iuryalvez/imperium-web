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

        public async Task<DietaPersonalizada?> GetByRelatorioIdAsync(int relatorioId)
        {
            return await _context.DietasPersonalizadas
                .Include(d => d.Racao)
                .Include(d => d.Relatorio)
                .FirstOrDefaultAsync(d => d.RelatorioId == relatorioId);
        }

        public async Task<DietaPersonalizada?> FindByRelatorioIdAsync(int relatorioId)
        {
            // This is a specific query to support the "update-or-insert" (upsert) logic.
            // It checks for the existence of a diet based on its foreign key relationship with the report.
            return await _context.DietasPersonalizadas
                .FirstOrDefaultAsync(d => d.RelatorioId == relatorioId);
        }

        public async Task<IEnumerable<DietaPersonalizada>> GetAllByAcompanhamentoIdAsync(int acompanhamentoId)
        {
            // Busca todas as dietas cujos relatórios pertencem ao acompanhamento especificado
            return await _context.DietasPersonalizadas
                .Include(d => d.Racao) // Inclui a ração para exibir o nome
                .Where(d => d.Relatorio!.AcompanhamentoIndividualizadoId == acompanhamentoId)
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();
        }

        public async Task<DietaPersonalizada?> GetByIdWithIncludesAsync(int dietaId)
        {
            // Inclui a cadeia de navegação para permitir a validação de segurança no serviço
            // Dieta -> Relatorio -> Consulta -> ClinicaId
            return await _context.DietasPersonalizadas
                .Include(d => d.Racao)
                .Include(d => d.Relatorio)
                    .ThenInclude(r => r.Consulta)
                .FirstOrDefaultAsync(d => d.Id == dietaId);
        }

        public async Task AddAsync(DietaPersonalizada dieta)
        {
            await _context.DietasPersonalizadas.AddAsync(dieta);
        }

        public async Task UpdateAsync(DietaPersonalizada dieta)
        {
            _context.DietasPersonalizadas.Update(dieta);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Delete(DietaPersonalizada dieta)
        {
            _context.DietasPersonalizadas.Remove(dieta);
        }
    }
}