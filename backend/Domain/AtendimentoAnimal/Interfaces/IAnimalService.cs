using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IAnimalService
    {
        // --- Métodos de Leitura (para Controllers, retornam DTOs) ---
        Task<ServiceResult> ObterPorIdAsync(int animalId);
        Task<PaginaResultado<GetAnimaisClinicaDto>> ListarAsync(AnimalFiltro filtro, int clinicaId);
        Task<List<OpcaoDto>> ListarOpcoesAsync(int clinicaId, string? busca);
        Task<ServiceResult> GetDtoDetailsByIdAsync(int animalId, int clinicaId);
        Task<ServiceResult> ObterHistoricoCompletoAnimalAsync(int animalId, int clinicaId);

        // --- Método de Leitura (para outros Serviços, retorna Entidade) ---
        Task<ServiceResult> ObterPorIdEClinicaIdAsync(int animalId, int clinicaId);

        // --- Métodos de Escrita ---
        Task<ServiceResult> CreateAsync(CadastrarAnimalClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(EditarAnimalClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateEntityAsync(Animal animal);
        // Task<ServiceResult> AtualizarEstadoPeloAvaliacaoAsync(int animalId, int clinicaId, UpsertAvaliacaoDto dadosAvaliacao);
        Task<ServiceResult> SetStatusAsync(int animalId, int clinicaId, bool ativo);
        Task<ServiceResult> DeleteAsync(int animalId, int clinicaId);

        // --- Método de Validação ---
        Task<bool> AnimalExisteNaClinicaAsync(int animalId, int clinicaId); // Manter este retorno bool é aceitável para validações rápidas.
        Task<bool> AnimaisExistemNaClinicaAsync(List<int> animaisIds, int clinicaId);
    }
}
