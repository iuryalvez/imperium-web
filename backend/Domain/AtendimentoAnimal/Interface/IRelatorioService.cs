using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRelatorioService
    {
       // --- Métodos de Escrita (Write) ---
        Task<ServiceResult> UpsertAsync(int consultaId, UpsertRelatorioDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int relatorioId, int clinicaId);
        Task<ServiceResult> AtualizarEstadoComDadosRelatorioAsync(int animalId, int clinicaId, UpsertRelatorioDto dadosRelatorio);

        // --- Métodos de Leitura (Read) ---

        // Retorna o DTO para o frontend.
        Task<ServiceResult> GetDtoByConsultaIdAsync(int consultaId, int clinicaId);

        // NOVOS MÉTODOS para uso interno entre serviços. Retornam a entidade completa.
        Task<ServiceResult> ObterPorIdEIdAcompanhamentoAsync(int relatorioId, int acompanhamentoId, int clinicaId);
        Task<ServiceResult> ObterPorIdAsync(int relatorioId, int clinicaId);
        Task<ServiceResult> ObterPorConsultaIdAsync(int consultaId, int clinicaId);
    }
}