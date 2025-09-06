using backend.Domain.AtendimentoAnimal;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IAnimalService
    {
        Task<IEnumerable<GetAnimaisClinicaDto>> GetAllByClinicIdAsync(int clinicaId);
        Task<GetAnimaisClinicaDto?> GetDetailsByIdAsync(int animalId, int clinicaId);
        Task<ServiceResult> CreateAsync(CadastrarAnimalClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(EditarAnimalClinicaDto dto, int clinicaId);
        Task<ServiceResult> DesativarVinculoAnimalAsync(int animalId, int clinicaId);
        Task<bool> IsAnimalInClinicAsync(int animalId, int clinicaId);
    }
}
