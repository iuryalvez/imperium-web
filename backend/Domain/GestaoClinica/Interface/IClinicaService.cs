using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils.Pagination;

namespace backend.Domain.GestaoClinica.Interface
{
    public interface IClinicaService
    {
        Task<PagedResponse<GetClinicaDto>> GetAllAsync(int page, int pageSize, string? searchNome);
        Task<GetClinicaDto?> GetByIdAsync(int clinicaId);
        Task<ServiceResult> GetClinicaDashboardAsync(int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarClinicaDto dto);
        Task<ServiceResult> UpdateClinicaAsync(EditarClinicaAdminDto dto);
        Task<ServiceResult> UpdateOwnClinicAsync(int clinicaId, EditarClinicaDto dto);
        Task<ServiceResult> DeleteAsync(int clinicaId);
        public Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        public Task<bool> UsuarioCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        public Task<List<TutorClinica>> ObterVinculosTutoresDaClinicaAsync(int clinicaId);
        public Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaAsync(int clinicaId);
        public Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaComTutoresAsync(int clinicaId);
    }
}
