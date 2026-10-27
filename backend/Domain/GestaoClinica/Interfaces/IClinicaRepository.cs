using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.DTOs.Dashboard;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.GestaoClinica.Interfaces
{
    public interface IClinicaRepository
    {
        Task<Clinica?> GetByIdAsync(int clinicaId);
        Task<Clinica?> GetByIdWithDetailsAsync(int clinicaId);
        Task<GetClinicaDashboardDto?> GetClinicaDashboardAsync(int clinicaId);
        Task<IEnumerable<Clinica>> GetAllAsync();
        Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId);
        Task AddAsync(Clinica clinica);
        void Delete(Clinica clinica);
        Task<int> SaveChangesAsync();
    }
}