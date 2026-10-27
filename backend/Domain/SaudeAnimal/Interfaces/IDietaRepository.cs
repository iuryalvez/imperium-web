using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IDietaRepository
    {
        Task<Dieta?> GetByIdWithIncludesAsync(int dietaId);
        Task<Dieta?> GetByAvaliacaoIdAsync(int avaliacaoId);
        Task<IEnumerable<Dieta>> GetAllByAcompanhamentoIdAsync(int acompanhamentoId);
        Task<Dieta?> FindByAvaliacaoIdAsync(int avaliacaoId);
        Task<Dieta?> FindEmAndamentoByAcompanhamentoIdAsync(int acompanhamentoId);
        Task AddAsync(Dieta dieta);
        Task UpdateAsync(Dieta dieta);
        Task SaveChangesAsync();
        void Delete(Dieta dieta);
        Task AddRegistroPesoAsync(RegistroPesoDieta registro);
        void RemoveRegistroPeso(RegistroPesoDieta registro);
    }
}