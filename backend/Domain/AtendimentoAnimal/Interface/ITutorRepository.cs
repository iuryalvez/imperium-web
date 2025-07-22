using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface ITutorRepository
    {
        Task<Tutor?> GetByIdWithIncludesAsync(int tutorId, int clinicaId);
        void RemoveAnimalLinks(IEnumerable<TutorAnimal> links);
        Task AddAnimalLinkAsync(TutorAnimal link);
        Task<int> SaveChangesAsync();
    }
}
