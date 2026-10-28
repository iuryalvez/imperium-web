using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface ITutorRepository
    {
        Task<bool> CpfExistsInClinicAsync(string cpf, int clinicaId, int? excludeTutorId = null);
        Task<Tutor?> GetByIdAndClinicIdWithIncludesAsync(int tutorId, int clinicaId);

        Task AddAnimalLinkAsync(TutorAnimal link);
        void RemoveAnimalLinks(IEnumerable<TutorAnimal> links);

        Task<bool> AllTutorsExistInClinicAsync(List<int> tutoresIds, int clinicaId);
        Task<bool> AllTutorsActiveAsync(List<int> tutoresIds, int clinicaId);
        Task<IEnumerable<Tutor>> GetAllByClinicIdWithIncludesAsync(int clinicaId);
        Task<Tutor?> GetByIdAndClinicIdAsync(int tutorId, int clinicaId);
        Task<bool> HasConsultasAsync(int tutorId);

        Task AddAsync(Tutor tutor);
        void Remove(Tutor tutor);
        Task<int> SaveChangesAsync();
    }
}
