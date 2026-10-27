using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.SaudeAnimal.DTOs.Racao;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IRacaoService
    {
        // --- Métodos de Leitura (para Controllers) ---
        Task<ServiceResult> ObterTodosDtosPorClinicaIdEEspecieAsync(int clinicaId, EspecieAnimal? especie);
        Task<ServiceResult> ObterDtoPorIdAsync(int racaoId, int clinicaId);

        // --- Método de Leitura (para outros Serviços) ---
        Task<ServiceResult> ObterPorIdEClinicaIdAsync(int racaoId, int clinicaId);

        // --- Métodos de Escrita ---
        Task<ServiceResult> CreateAsync(CadastrarRacaoDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(AtualizarRacaoDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int racaoId, int clinicaId);
    }
}