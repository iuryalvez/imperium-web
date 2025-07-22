using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRacaService
    {
        Task<List<RacaDto>> ListAllAsync(EspecieAnimal? especie);
        Task<RacaDto?> GetByIdAsync(int id);
        Task<RacaDto> CreateAsync(CriarRacaoDto criarRacaDto);
        Task<bool> UpdateAsync(int id, AtualizarRacaDto atualizarRacaDto);
        Task<bool> DeleteAsync(int id);
    }
}
