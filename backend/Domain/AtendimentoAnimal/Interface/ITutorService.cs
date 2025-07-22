using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface ITutorService
    {
        Task<ServiceResult> UpdateTutorAsync(EditarTutorClinicaDto dto, int clinicaId);
    }
}
