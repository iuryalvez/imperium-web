using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;
using backend.Domain.SaudeAnimal.DTOs.Racao;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IRacaoService
    {
        // --- Clínica: leitura (para Controllers) ---
        Task<PaginaResultado<GetRacoesDto>> ListarDaClinicaAsync(RacaoFiltro filtro, int clinicaId);
        Task<List<OpcaoDto>> ListarOpcoesDaClinicaAsync(int clinicaId, string? busca, EspecieAnimal? especie);
        Task<ServiceResult> ObterDtoPorIdAsync(int racaoId, int clinicaId);

        // --- Clínica: leitura (para outros Serviços, retorna a entidade) ---
        Task<ServiceResult> ObterPorIdEClinicaIdAsync(int racaoId, int clinicaId);
        Task<ServiceResult> ObterSelecionavelAsync(int racaoId, int clinicaId, int? racaoAtualId = null);

        // --- Clínica: escrita ---
        Task<ServiceResult> CreateAsync(CadastrarRacaoDto dto, int clinicaId);
        Task<ServiceResult> UpdateAsync(AtualizarRacaoDto dto, int clinicaId);
        Task<ServiceResult> DeleteAsync(int racaoId, int clinicaId);
        Task<ServiceResult> SetStatusDaClinicaAsync(int racaoId, int clinicaId, bool ativo);
        Task<ServiceResult> SetOcultaAsync(int racaoId, int clinicaId, bool oculta);

        // --- Sistema (admin) ---
        Task<PaginaResultado<GetRacoesDto>> ListarSistemaAsync(RacaoFiltro filtro);
        Task<PaginaResultado<GetRacoesDto>> ListarCopiasAsync(RacaoFiltro filtro);
        Task<ServiceResult> ObterAsync(int racaoId);
        Task<ServiceResult> CadastrarSistemaAsync(CadastrarRacaoDto dto);
        Task<ServiceResult> EditarAsync(AtualizarRacaoDto dto);
        Task<ServiceResult> ExcluirAsync(int racaoId);
        Task<ServiceResult> SetStatusAsync(int racaoId, bool ativo);
    }
}
