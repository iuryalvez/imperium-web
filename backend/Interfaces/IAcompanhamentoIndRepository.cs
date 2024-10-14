using backend.Dtos.AcompanhamentoInd;
using backend.Models;

namespace backend.Interfaces
{
    public interface IAcompanhamentoIndRepository
    {
        Task<List<AcompanhamentoInd>> GetAllAsync();
        Task<AcompanhamentoInd?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<AcompanhamentoInd> CreateAsync(AcompanhamentoInd acompanhamentoIndModel);
        Task<AcompanhamentoInd?> UpdateAsync(int id, UpdateAcompanhamentoIndRequestDto animalDto);
        Task<AcompanhamentoInd?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}