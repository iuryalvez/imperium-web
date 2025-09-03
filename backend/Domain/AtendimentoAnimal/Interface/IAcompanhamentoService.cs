using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public interface IAcompanhamentoService
    {
        Task<ServiceResult> CreateAsync(int animalId);
        Task<AcompanhamentoIndividualizado?> GetByAnimalIdAsync(int animalId);
    }
}
