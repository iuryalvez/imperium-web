using backend.Domain.AtendimentoAnimal;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IAnimalService
    {
        // --- Métodos de Leitura (para Controllers, retornam DTOs) ---
        Task<ServiceResult> ObterPorIdAsync(int animalId);
        Task<ServiceResult> GetAllDtosByClinicIdAsync(int clinicaId);
        Task<ServiceResult> GetDtoDetailsByIdAsync(int animalId, int clinicaId);
        Task<ServiceResult> ObterHistoricoCompletoAnimalAsync(int animalId, int clinicaId);

        // --- Método de Leitura (para outros Serviços, retorna Entidade) ---
        Task<ServiceResult> ObterPorIdEClinicaIdAsync(int animalId, int clinicaId);

        // --- Métodos de Escrita ---
        Task<ServiceResult> CreateAsync(CadastrarAnimalClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(EditarAnimalClinicaDto dto, int clinicaId);
        // Task<ServiceResult> AtualizarEstadoPeloRelatorioAsync(int animalId, int clinicaId, UpsertRelatorioDto dadosRelatorio);
        Task<ServiceResult> DesativarVinculoAnimalAsync(int animalId, int clinicaId);

        // --- Método de Validação ---
        Task<bool> AnimalExisteNaClinicaAsync(int animalId, int clinicaId); // Manter este retorno bool é aceitável para validações rápidas.
        Task<bool> AnimaisExistemNaClinicaAsync(List<int> animaisIds, int clinicaId);
    }
}
