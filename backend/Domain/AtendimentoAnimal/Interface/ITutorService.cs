using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils.Pagination;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface ITutorService
    {
        Task<PagedResponse<Tutor>> ObterTodosPorClinicaIdAsync(int clinicaId, int page, int pageSize, string? searchNome);
        Task<Tutor?> ObterPorIdEClinicaIdAsync(int tutorId, int clinicaId);
        Task<GetTutoresClinicaDto?> ObterDtoPorIdEClinicaId(int tutorId, int clinicaId);
        Task<PagedResponse<GetTutoresClinicaDto>> ObterTodosDtosPeloClinicaIdAsync(int clinicaId, int page, int pageSize, string? searchNome);
        Task<ServiceResult> ValidarTutoresDaClinicaAsync(List<int> tutoresIds, int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(EditarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int tutorId, int clinicaId);
        Task<ServiceResult> SetTutorStatusAsync(int tutorId, int clinicaId, bool ativo);
    }
}
