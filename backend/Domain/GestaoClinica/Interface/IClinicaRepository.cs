using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.Utils.Pagination;

namespace backend.Domain.GestaoClinica.Interface
{
    public interface IClinicaRepository
    {
        Task<Clinica?> GetByIdAsync(int clinicaId);
        Task<Clinica?> GetByIdWithDetailsAsync(int clinicaId);
        Task<GetClinicaDashboardDto?> GetClinicaDashboardAsync(int clinicaId);
        Task<PagedResponse<Clinica>> GetAllAsync(int page, int pageSize, string? searchNome);
        Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId);
        Task AddAsync(Clinica clinica);
        void Delete(Clinica clinica);
        Task<int> SaveChangesAsync();
    }
}