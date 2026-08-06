using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils.Pagination;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IConsultaService
    {
        Task<ServiceResult> ObterPorIdAsync(int consultaId);
        Task<ServiceResult> ObterPorIdComDetalhesAsync(int consultaId);
        Task<ServiceResult> CreateAsync(UpsertConsultaDto dto, int clinicaId, int usuarioId);
        Task<ServiceResult> UpdateAsync(int consultaId, UpsertConsultaDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int consultaId, int clinicaId);
        Task<PagedResponse<GetConsultaDto>> ObterTodasPorClinicaIdAsync(int clinicaId, int page, int pageSize, string? searchNome, DateTime? searchDataInicio, DateTime? searchDataFim);
        Task<DetalhesConsultaDto?> ObterDetalhesPorIdEClinicaIdAsync(int consultaId, int clinicaId);
    }
}