using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.Utils.Pagination;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IRacaoRepository
    {
        Task<PagedResponse<Racao>> GetAllByClinicIdAndEspecieAsync(int clinicaId, EspecieAnimal? especie, int page, int pageSize, string? searchNome);
        Task<Racao?> GetByIdAndClinicIdAsync(int racaoId, int clinicaId);
        Task AddAsync(Racao racao);
        void Delete(Racao racao);
        Task<int> SaveChangesAsync();
        Task<Racao?> GetByIdAndClinicaIdAsync(int racaoId, int clinicaId);
    }
}