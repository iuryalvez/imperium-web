using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IRacaoService
    {
        Task<IEnumerable<GetRacoesDto>> GetAllByClinicIdAsync(int clinicaId, EspecieAnimal? especie);
        Task<GetRacoesDto?> GetByIdAsync(int racaoId, int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarRacaoDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(AtualizarRacaoDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int racaoId, int clinicaId);
    }
}