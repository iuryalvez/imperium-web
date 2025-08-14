using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface ITutorRepository
    {
        Task<Tutor?> FindByCpfAsync(string cpf);
        Task<Tutor?> GetByIdWithIncludesAsync(int tutorId, int clinicaId);

        Task AddAnimalLinkAsync(TutorAnimal link);
        void RemoveAnimalLinks(IEnumerable<TutorAnimal> links);

        Task<bool> AllTutorsExistInClinicAsync(List<int> tutorIds, int clinicaId);
        Task<IEnumerable<Tutor>> GetAllByClinicIdWithIncludesAsync(int clinicaId);
        Task<bool> IsTutorLinkedToClinicAsync(int tutorId, int clinicaId);
        Task<TutorClinica?> GetClinicLinkAsync(int tutorId, int clinicaId);
        Task UpdateClinicLinkAsync(TutorClinica link);
        Task AddClinicLinkAsync(TutorClinica link);
        void RemoveClinicLink(TutorClinica link);

        Task AddAsync(Tutor tutor);
        void Remove(Tutor tutor);
        Task<int> SaveChangesAsync();
    }
}
