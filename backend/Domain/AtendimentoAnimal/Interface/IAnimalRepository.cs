using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IAnimalRepository
    {
        Task<IEnumerable<Animal>> GetAnimaisByClinicIdWithIncludesAsync(int clinicaId);
        Task<Animal?> GetAnimalDetailsAsync(int animalId, int clinicaId);
        Task<bool> AllAnimalsExistInClinicAsync(List<int> animalIds, int clinicaId);
        Task AddAsync(Animal animal);
        Task AddVinculoClinicaAsync(AnimalClinica vinculo);
        Task<Animal?> GetByIdWithIncludesAsync(int animalId, int clinicaId);
        void RemoveTutorLinks(IEnumerable<TutorAnimal> links);
        Task AddTutorLinkAsync(TutorAnimal link);
        Task<int> SaveChangesAsync();
    }
}
