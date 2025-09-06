using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRelatorioRepository
    {
        Task<Relatorio?> GetByIdWithAnimalAsync(int relatorioId);
        Task<Relatorio?> GetByIdAsync(int relatorioId);
        Task<Relatorio?> GetByConsultaIdAsync(int consultaId);
        Task<Relatorio?> GetByIdWithConsultaAsync(int relatorioId);
        Task<Relatorio?> GetByIdAndAcompanhamentoIdAsync(int relatorioId, int acompanhamentoId);
        Task AddAsync(Relatorio relatorio);
        void Delete(Relatorio relatorio);
        // SaveChanges será chamado pelo serviço para garantir a transação
    }
}