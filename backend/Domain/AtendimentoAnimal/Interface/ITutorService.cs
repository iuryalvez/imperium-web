using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface ITutorService
    {
        Task<IEnumerable<GetTutoresClinicaDto>> GetAllByClinicIdAsync(int clinicaId);
        Task<ServiceResult> UpdateTutorAsync(EditarTutorClinicaDto dto, int clinicaId);
    }
}
