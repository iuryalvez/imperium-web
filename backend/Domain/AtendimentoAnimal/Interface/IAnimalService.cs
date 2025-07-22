using backend.Domain.AtendimentoAnimal;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IAnimalService
    {
        Task<IEnumerable<GetAnimaisClinicaDto>> GetAllByClinicIdAsync(int clinicaId);
        Task<ServiceResult> CadastrarAnimalAsync(CadastrarAnimalClinicaDto dto, int clinicaId);
        Task<GetAnimaisClinicaDto?> GetDetailsByIdAsync(int animalId, int clinicaId);
        Task<ServiceResult> UpdateAnimalAsync(EditarAnimalClinicaDto dto, int clinicaId);
    }
}
