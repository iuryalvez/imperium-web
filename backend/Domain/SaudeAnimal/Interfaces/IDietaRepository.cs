using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IDietaRepository
    {
        Task<DietaPersonalizada?> GetByIdWithIncludesAsync(int dietaId);
        Task<DietaPersonalizada?> GetByRelatorioIdAsync(int relatorioId);
        Task<IEnumerable<DietaPersonalizada>> GetAllByAcompanhamentoIdAsync(int acompanhamentoId);
        Task<DietaPersonalizada?> FindByRelatorioIdAsync(int relatorioId);
        Task AddAsync(DietaPersonalizada dieta);
        Task UpdateAsync(DietaPersonalizada dieta);
        Task SaveChangesAsync();
        void Delete(DietaPersonalizada dieta);
    }
}