using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface ITutorService
    {
        Task<GetTutoresClinicaDto?> GetByIdAsync(int tutorId, int clinicaId);
        Task<IEnumerable<GetTutoresClinicaDto>> GetAllByClinicIdAsync(int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(EditarTutorClinicaDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int tutorId, int clinicaId);
        Task<ServiceResult> SetTutorStatusAsync(int tutorId, int clinicaId, bool ativo);
    }
}
