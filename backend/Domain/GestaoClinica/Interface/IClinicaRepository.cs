using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.GestaoClinica.Interface
{
    public interface IClinicaRepository
    {
        Task<Clinica?> GetByIdAsync(int clinicaId);
        Task<IEnumerable<GetClinicaDto>> GetAllAsync();
        Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId);
        Task<int> SaveChangesAsync();
    }
}