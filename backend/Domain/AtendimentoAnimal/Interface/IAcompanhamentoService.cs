using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.SaudeAnimal.DTOs.Dieta;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public interface IAcompanhamentoService
    {
        Task<ServiceResult> CreateAsync(int animalId);
        Task<AcompanhamentoIndividualizado?> GetByAnimalIdAsync(int animalId);
        Task<ServiceResult> CriarDietaParaRelatorioAsync(int acompanhamentoId, CreateDietaDto dto, int clinicaId);
    }
}
