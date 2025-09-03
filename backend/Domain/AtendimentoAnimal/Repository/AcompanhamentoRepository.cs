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

        public async Task<AcompanhamentoIndividualizado?> GetByAnimalIdAsync(int animalId)
        {
            return await _context.AcompanhamentosInds
                .Include(a => a.Relatorios)
                .FirstOrDefaultAsync(a => a.AnimalId == animalId);
        }

        public async Task AddAsync(AcompanhamentoIndividualizado acompanhamento)
        {
            await _context.AcompanhamentosInds.AddAsync(acompanhamento);
        }

        public async Task UpdateAsync(AcompanhamentoIndividualizado acompanhamento)
        {
            _context.AcompanhamentosInds.Update(acompanhamento);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsByAnimalIdAsync(int animalId)
        {
            return await _context.AcompanhamentosInds.AnyAsync(a => a.AnimalId == animalId);
        }
    }
}