using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRacaService
    {
        Task<List<GetRacaDto>> ListAllAsync(EspecieAnimal? especie);
        Task<GetRacaDto?> GetByIdAsync(int id);
        Task<GetRacaDto> CreateAsync(CadastrarRacaDto criarRacaDto);
        Task<bool> UpdateAsync(UpdateRacaDto atualizarRacaDto);
        Task<bool> DeleteAsync(int id);
    }
}
