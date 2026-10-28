using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IAnimalRepository
    {
        Task<Animal?> GetByIdAsync(int animalId);
        Task<Animal?> GetByIdAndClinicIdWithDetailsAsync(int animalId, int clinicaId);
        Task<IEnumerable<Animal>> GetAllByClinicaIdWithDetailsAsync(int clinicaId);
        Task<bool> IsAnimalInClinicAsync(int animalId, int clinicaId);
        Task<bool> AllAnimalsExistInClinicAsync(List<int> animalIds, int clinicaId);
        Task AddAsync(Animal animal);
        Task UpdateAsync(Animal animal);
        Task<Animal?> GetByIdAndClinicaIdWithIncludesAsync(int animalId, int clinicaId);
        Task<Animal?> GetHistoricoCompletoAsync(int animalId, int clinicaId);
        void RemoveTutorLinks(IEnumerable<TutorAnimal> links);
        Task AddTutorLinkAsync(TutorAnimal link);
        Task<bool> HasConsultasAsync(int animalId);
        void Remove(Animal animal);
        Task<int> SaveChangesAsync();
    }
}
