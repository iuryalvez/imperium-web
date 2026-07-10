using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface ITutorService
    {
        Task<IEnumerable<Tutor>> ObterTodosPorClinicaIdAsync(int clinicaId);
        Task<Tutor?> ObterPorIdEClinicaIdAsync(int tutorId, int clinicaId);
        Task<GetTutoresClinicaDto?> ObterDtoPorIdEClinicaId(int tutorId, int clinicaId);
        Task<IEnumerable<GetTutoresClinicaDto>> ObterTodosDtosPeloClinicaIdAsync(int clinicaId);
        Task<ServiceResult> ValidarTutoresDaClinicaAsync(List<int> tutoresIds, int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(EditarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int tutorId, int clinicaId);
        Task<ServiceResult> SetTutorStatusAsync(int tutorId, int clinicaId, bool ativo);
    }
}
