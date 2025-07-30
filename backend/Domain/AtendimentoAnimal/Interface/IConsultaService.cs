using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IConsultaService
    {
        Task<ServiceResult> CreateAsync(UpsertConsultaDto dto, int clinicaId, int usuarioId);
        Task<ServiceResult> UpdateAsync(int consultaId, UpsertConsultaDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int consultaId, int clinicaId);
        Task<IEnumerable<GetConsultaDto>> GetAllByClinicIdAsync(int clinicaId);
        Task<DetalhesConsultaDto?> GetDetailsByIdAsync(int consultaId, int clinicaId);
    }
}