using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IRacaRepository
    {
        // Clínica: raças do sistema + próprias (nunca de outra clínica)
        Task<PaginaResultado<Raca>> ListarDaClinicaAsync(RacaFiltro filtro, int clinicaId);
        Task<List<OpcaoDto>> ListarOpcoesDaClinicaAsync(int clinicaId, string? busca, EspecieAnimal? especie);
        Task<Raca?> GetDisponivelParaClinicaAsync(int id, int clinicaId);
        Task<HashSet<int>> GetIdsOcultasAsync(int clinicaId, IEnumerable<int> racaIds);
        Task<RacaOcultaClinica?> GetOcultaAsync(int racaId, int clinicaId);
        Task AddOcultaAsync(RacaOcultaClinica oculta);
        void RemoveOculta(RacaOcultaClinica oculta);

        // Sistema (admin)
        Task<PaginaResultado<Raca>> ListarSistemaAsync(RacaFiltro filtro);
        Task<PaginaResultado<Raca>> ListarCopiasAsync(RacaFiltro filtro);
        Task<Raca?> GetByIdAsync(int id);

        Task<bool> EmUsoAsync(int racaId);
        Task AddAsync(Raca raca);
        void Delete(Raca raca);
        Task<int> SaveChangesAsync();
    }
}
