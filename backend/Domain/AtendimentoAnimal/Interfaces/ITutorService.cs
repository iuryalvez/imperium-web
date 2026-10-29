using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface ITutorService
    {
        Task<Tutor?> ObterPorIdEClinicaIdAsync(int tutorId, int clinicaId);
        Task<GetTutoresClinicaDto?> ObterDtoPorIdEClinicaId(int tutorId, int clinicaId);
        Task<PaginaResultado<GetTutoresClinicaDto>> ListarAsync(TutorFiltro filtro, int clinicaId);
        Task<List<OpcaoDto>> ListarOpcoesAsync(int clinicaId, string? busca);
        Task<ServiceResult> ValidarTutoresDaClinicaAsync(List<int> tutoresIds, int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(EditarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int tutorId, int clinicaId);
        Task<ServiceResult> SetTutorStatusAsync(int tutorId, int clinicaId, bool ativo);
    }
}
