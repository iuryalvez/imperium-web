using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRacaService
    {
        Task<List<GetRacaDto>> GetAllSystemByEspecieAsync(EspecieAnimal? especie);
        Task<List<GetRacaDto>> ListByClinicaIdAndEspecieAsync(int clinicaId, EspecieAnimal? especie);
        Task<GetRacaDto?> GetByIdAsync(int id);
        Task<GetRacaDto> CreateAsync(CadastrarRacaDto criarRacaDto, int clinicaId);
        Task<bool> CreateOrUpdateAsync(UpdateRacaDto atualizarRacaDto, int clinicaId);
        Task<bool> DeleteAsync(int id);
    }
}
