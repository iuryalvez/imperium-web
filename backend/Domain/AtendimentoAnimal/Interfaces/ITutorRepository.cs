using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Utils.Paginacao;

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
        Task<PaginaResultado<Tutor>> ListarDaClinicaAsync(TutorFiltro filtro, int clinicaId);
        Task<List<OpcaoDto>> ListarOpcoesAsync(int clinicaId, string? busca);
        Task<Tutor?> GetByIdAndClinicIdAsync(int tutorId, int clinicaId);
        Task<bool> HasConsultasAsync(int tutorId);

        Task AddAsync(Tutor tutor);
        void Remove(Tutor tutor);
        Task<int> SaveChangesAsync();
    }
}
