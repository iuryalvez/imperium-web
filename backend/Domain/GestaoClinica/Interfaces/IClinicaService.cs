using backend.Domain.Utils.Paginacao;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.DTOs.Dashboard;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;

namespace backend.Domain.GestaoClinica.Interfaces
{
    public interface IClinicaService
    {
        Task<PaginaResultado<GetClinicaDto>> ListarAsync(ClinicaFiltro filtro);
        Task<List<OpcaoDto>> ListarOpcoesAsync(string? busca);
        Task<GetClinicaDto?> GetByIdAsync(int clinicaId);
        Task<ServiceResult> GetClinicaDashboardAsync(int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarClinicaDto dto);
        Task<ServiceResult> UpdateClinicaAsync(EditarClinicaAdminDto dto);
        Task<ServiceResult> UpdateOwnClinicAsync(int clinicaId, EditarClinicaDto dto);
        Task<ServiceResult> DeleteAsync(int clinicaId);
        public Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        public Task<bool> UsuarioCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        public Task<List<Tutor>> ObterTutoresDaClinicaAsync(int clinicaId);
        public Task<List<Animal>> ObterAnimaisDaClinicaAsync(int clinicaId);
        public Task<List<Animal>> ObterAnimaisDaClinicaComTutoresAsync(int clinicaId);
    }
}
