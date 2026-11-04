using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IRacaService
    {
        // --- Clínica (raças do sistema + próprias) ---
        Task<PaginaResultado<GetRacaDto>> ListarDaClinicaAsync(RacaFiltro filtro, int clinicaId);
        Task<List<OpcaoDto>> ListarOpcoesDaClinicaAsync(int clinicaId, string? busca, EspecieAnimal? especie);
        Task<ServiceResult> ObterDaClinicaAsync(int id, int clinicaId);
        Task<ServiceResult> CadastrarDaClinicaAsync(CadastrarRacaDto dto, int clinicaId);
        Task<ServiceResult> EditarDaClinicaAsync(UpdateRacaDto dto, int clinicaId);
        Task<ServiceResult> ExcluirDaClinicaAsync(int id, int clinicaId);
        Task<ServiceResult> SetStatusDaClinicaAsync(int id, int clinicaId, bool ativo);
        Task<ServiceResult> SetOcultaAsync(int id, int clinicaId, bool oculta);
        Task<ServiceResult> ValidarSelecionavelAsync(int racaId, int clinicaId);

        /// <summary>Confere se a raça (disponível para a clínica) é da espécie informada.</summary>
        Task<ServiceResult> ValidarEspecieAsync(int racaId, int clinicaId, EspecieAnimal especie);

        // --- Sistema (admin) ---
        Task<PaginaResultado<GetRacaDto>> ListarSistemaAsync(RacaFiltro filtro);
        Task<PaginaResultado<GetRacaDto>> ListarCopiasAsync(RacaFiltro filtro);
        Task<ServiceResult> ObterAsync(int id);
        Task<ServiceResult> CadastrarSistemaAsync(CadastrarRacaDto dto);
        Task<ServiceResult> EditarAsync(UpdateRacaDto dto);
        Task<ServiceResult> ExcluirAsync(int id);
        Task<ServiceResult> SetStatusAsync(int id, bool ativo);
    }
}
