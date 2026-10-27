using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IAvaliacaoRepository
    {
        Task<Avaliacao?> GetByIdWithAnimalAsync(int avaliacaoId);
        Task<Avaliacao?> GetByIdAsync(int avaliacaoId);
        Task<Avaliacao?> GetByConsultaIdAsync(int consultaId);
        Task<Avaliacao?> GetByIdWithConsultaAsync(int avaliacaoId);
        Task<Avaliacao?> GetByIdAndAcompanhamentoIdAsync(int avaliacaoId, int acompanhamentoId);
        Task AddAsync(Avaliacao avaliacao);
        void Delete(Avaliacao avaliacao);
        // SaveChanges será chamado pelo serviço para garantir a transação
    }
}