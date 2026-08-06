using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Utils.Pagination;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IConsultaRepository
    {
        Task<Consulta?> GetByIdAsync(int consultaId);
        Task<Consulta?> GetByIdWithDetailsAsync(int consultaId);
        Task<PagedResponse<Consulta>> GetAllByClinicIdAsync(int clinicaId, int page, int pageSize, string? searchNome, DateTime? searchDataInicio, DateTime? searchDataFim);
        Task<Consulta?> GetByIdAndClinicIdAsync(int consultaId, int clinicaId);
        Task AddAsync(Consulta consulta);
        void Delete(Consulta consulta);
        Task<int> SaveChangesAsync();
    }
}