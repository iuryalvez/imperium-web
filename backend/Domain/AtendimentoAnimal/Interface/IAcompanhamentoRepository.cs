using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IAcompanhamentoRepository
    {
        Task<AcompanhamentoIndividualizado?> GetByAnimalIdAsync(int animalId);
        Task AddAsync(AcompanhamentoIndividualizado acompanhamento);
        Task UpdateAsync(AcompanhamentoIndividualizado acompanhamento);
        Task<bool> ExistsByAnimalIdAsync(int animalId);
    }
}
