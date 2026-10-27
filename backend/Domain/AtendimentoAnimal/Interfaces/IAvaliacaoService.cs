using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IAvaliacaoService
    {
       // --- Métodos de Escrita (Write) ---
        Task<ServiceResult> UpsertAsync(int consultaId, UpsertAvaliacaoDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int avaliacaoId, int clinicaId);
        Task<ServiceResult> AtualizarEstadoComDadosAvaliacaoAsync(int animalId, int clinicaId, UpsertAvaliacaoDto dadosAvaliacao);

        // --- Métodos de Leitura (Read) ---

        // Retorna o DTO para o frontend.
        Task<ServiceResult> GetDtoByConsultaIdAsync(int consultaId, int clinicaId);

        // NOVOS MÉTODOS para uso interno entre serviços. Retornam a entidade completa.
        Task<ServiceResult> ObterPorIdEIdAcompanhamentoAsync(int avaliacaoId, int acompanhamentoId, int clinicaId);
        Task<ServiceResult> ObterPorIdAsync(int avaliacaoId, int clinicaId);
        Task<ServiceResult> ObterPorConsultaIdAsync(int consultaId, int clinicaId);
    }
}