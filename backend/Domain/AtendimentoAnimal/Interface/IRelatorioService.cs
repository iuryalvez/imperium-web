using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRelatorioService
    {
        // Um método Upsert (Create/Update) é ideal para a relação 1-para-1 com Consulta
        Task<ServiceResult> UpsertAsync(int consultaId, UpsertRelatorioDto dto, int clinicaId);
        Task<GetRelatorioDto?> GetByConsultaIdAsync(int consultaId, int clinicaId);
        Task<ServiceResult> DeleteAsync(int relatorioId, int clinicaId);
    }
}