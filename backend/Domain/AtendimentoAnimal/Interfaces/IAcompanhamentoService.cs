using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.SaudeAnimal.DTOs.Dieta;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IAcompanhamentoService
    {
        // --- Métodos de Escrita ---
        Task<ServiceResult> CreateAsync(int animalId);
        Task<ServiceResult> EncerrarAcompanhamentoAsync(int acompanhamentoId, int clinicaId);
        Task<ServiceResult> CriarDietaParaAvaliacaoAsync(int acompanhamentoId, CreateDietaDto dto, int clinicaId);

        // --- Métodos de Leitura ---
        Task<ServiceResult> ObterPorIdAsync(int acompanhamentoId, int clinicaId);
        Task<ServiceResult> ObterPorAnimalIdAsync(int animalId, int clinicaId);
        Task<ServiceResult> ObterHistoricoCompletoAnimalAsync(int acompanhamentoId, int clinicaId, bool incluirDadosDoTutor = true);
        Task<ServiceResult> UpdateDietaAsync(int dietaId, UpdateDietaDto dto, int clinicaId);
        Task<ServiceResult> DeleteDietaAsync(int dietaId, int clinicaId);
    }
}
