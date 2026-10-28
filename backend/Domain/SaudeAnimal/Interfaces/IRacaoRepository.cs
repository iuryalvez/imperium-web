using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.DTOs.Racao;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IRacaoRepository
    {
        // Clínica: rações do sistema + próprias (nunca de outra clínica)
        Task<PaginaResultado<Racao>> ListarDaClinicaAsync(RacaoFiltro filtro, int clinicaId);
        Task<List<OpcaoDto>> ListarOpcoesDaClinicaAsync(int clinicaId, string? busca, EspecieAnimal? especie);
        Task<Racao?> GetAvailableByIdAndClinicIdAsync(int racaoId, int clinicaId);
        Task<HashSet<int>> GetIdsOcultasAsync(int clinicaId, IEnumerable<int> racaoIds);
        Task<RacaoOcultaClinica?> GetOcultaAsync(int racaoId, int clinicaId);
        Task AddOcultaAsync(RacaoOcultaClinica oculta);
        void RemoveOculta(RacaoOcultaClinica oculta);

        // Sistema (admin)
        Task<PaginaResultado<Racao>> ListarSistemaAsync(RacaoFiltro filtro);
        Task<PaginaResultado<Racao>> ListarCopiasAsync(RacaoFiltro filtro);
        Task<Racao?> GetByIdAsync(int racaoId);

        Task<bool> EmUsoAsync(int racaoId);
        Task AddAsync(Racao racao);
        void Delete(Racao racao);
        Task<int> SaveChangesAsync();
    }
}
