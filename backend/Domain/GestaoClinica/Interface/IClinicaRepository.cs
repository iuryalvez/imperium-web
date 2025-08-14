using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.GestaoClinica.Interface
{
    public interface IClinicaRepository
    {
        Task<Clinica?> GetByIdAsync(int clinicaId);
        Task<IEnumerable<Clinica>> GetAllAsync();
        Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId);
        Task AddAsync(Clinica clinica);
        void Delete(Clinica clinica);
        Task<IEnumerable<object>> SearchByNameAsync(string nome);
        Task<int> SaveChangesAsync();
    }
}