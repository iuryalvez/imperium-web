using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class AcompanhamentoRepository : IAcompanhamentoRepository
    {
        private readonly AppDbContext _context;

        public AcompanhamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Acompanhamento?> GetByIdAsync(int id)
        {
            return await _context.AcompanhamentosInds
                // Inclui o animal ao qual o acompanhamento pertence
                .Include(a => a.Animal)
                // Inclui a lista de relatórios do acompanhamento
                .Include(a => a.Avaliacoes!)
                    // Para cada relatório, inclui a dieta personalizada associada
                    .ThenInclude(r => r.Dieta)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Acompanhamento?> GetByAnimalIdAsync(int animalId)
        {
            return await _context.AcompanhamentosInds
                .Include(a => a.Avaliacoes)
                .FirstOrDefaultAsync(a => a.AnimalId == animalId);
        }

        public async Task AddAsync(Acompanhamento acompanhamento)
        {
            await _context.AcompanhamentosInds.AddAsync(acompanhamento);
        }

        public async Task UpdateAsync(Acompanhamento acompanhamento)
        {
            _context.AcompanhamentosInds.Update(acompanhamento);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsByAnimalIdAsync(int animalId)
        {
            return await _context.AcompanhamentosInds.AnyAsync(a => a.AnimalId == animalId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}