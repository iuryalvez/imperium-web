using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IRacaRepository
    {
        Task<List<Raca>> GetAllSystemAsync();
        Task<List<Raca>> ListByClinicaIdAsync(int clinicaId);
        Task<List<Raca>> ListAvailableForClinicaAsync(int clinicaId);
        Task<Raca?> GetByIdAsync(int id);
        Task AddAsync(Raca raca);
        void Update(Raca raca); // Update is often synchronous at the model level
        void Delete(Raca raca);
        Task<int> SaveChangesAsync(); // To commit the changes
    }
}
