using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRelatorioRepository
    {
        Task<Relatorio?> GetByIdAsync(int relatorioId);
        Task<Relatorio?> GetByConsultaIdAsync(int consultaId);
        Task<Relatorio?> GetByIdWithConsultaAsync(int relatorioId);
        Task AddAsync(Relatorio relatorio);
        void Delete(Relatorio relatorio);
        // SaveChanges será chamado pelo serviço para garantir a transação
    }
}