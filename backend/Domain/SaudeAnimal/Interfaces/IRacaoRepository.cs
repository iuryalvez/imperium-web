using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IRacaoRepository
    {
        Task<IEnumerable<Racao>> GetAllByClinicIdAndEspecieAsync(int clinicaId, EspecieAnimal? especie);
        Task<Racao?> GetByIdAndClinicIdAsync(int racaoId, int clinicaId);
        Task AddAsync(Racao racao);
        void Delete(Racao racao);
        Task<int> SaveChangesAsync();
        Task<Racao?> GetByIdAndClinicaIdAsync(int racaoId, int clinicaId);
    }
}