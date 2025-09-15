using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IConsultaService
    {
        Task<ServiceResult> ObterPorIdAsync(int consultaId);
        Task<ServiceResult> ObterPorIdComDetalhesAsync(int consultaId);
        Task<ServiceResult> CreateAsync(UpsertConsultaDto dto, int clinicaId, int usuarioId);
        Task<ServiceResult> UpdateAsync(int consultaId, UpsertConsultaDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int consultaId, int clinicaId);
        Task<IEnumerable<GetConsultaDto>> ObterTodasPorClinicaIdAsync(int clinicaId);
        Task<DetalhesConsultaDto?> ObterDetalhesPorIdEClinicaIdAsync(int consultaId, int clinicaId);
    }
}