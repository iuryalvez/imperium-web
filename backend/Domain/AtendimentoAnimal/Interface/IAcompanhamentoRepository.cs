using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IAcompanhamentoRepository
    {
        Task<Acompanhamento?> GetByIdAsync(int id);
        Task<Acompanhamento?> GetByAnimalIdAsync(int animalId);
        Task AddAsync(Acompanhamento acompanhamento);
        Task UpdateAsync(Acompanhamento acompanhamento);
        Task<bool> ExistsByAnimalIdAsync(int animalId);
        Task<int> SaveChangesAsync();
    }
}
