using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IAnimalRepository
    {
        Task<Animal?> GetByIdAsync(int animalId);
        Task<Animal?> GetByIdAndClinicIdWithDetailsAsync(int animalId, int clinicaId);
        Task<IEnumerable<Animal>> GetByClinicaIdWithIncludesAsync(int clinicaId);
        Task<AnimalClinica?> GetVinculoAnimalClinicaAsync(int animalId, int clinicaId);
        Task<bool> IsAnimalInClinicAsync(int animalId, int clinicaId);
        Task<bool> AllAnimalsExistInClinicAsync(List<int> animalIds, int clinicaId);
        Task AddAsync(Animal animal);
        Task AddVinculoClinicaAsync(AnimalClinica vinculo);
        Task<Animal?> GetByIdAndClinicaIdWithIncludesAsync(int animalId, int clinicaId);
        Task<Animal?> GetHistoricoCompletoAsync(int animalId, int clinicaId);
        void RemoveTutorLinks(IEnumerable<TutorAnimal> links);
        Task AddTutorLinkAsync(TutorAnimal link);
        void UpdateVinculo(AnimalClinica vinculo);
        Task<int> SaveChangesAsync();
    }
}
