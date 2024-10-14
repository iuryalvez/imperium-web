using backend.Dtos.Tutor;
using backend.Models;

namespace backend.Interfaces
{
    public interface ITutorRepository
    {
        Task<List<Tutor>> GetAllAsync();
        Task<Tutor?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<Tutor> CreateAsync(Tutor usuarioModel);
        Task<Tutor?> UpdateAsync(int id, UpdateTutorRequestDto usuarioDto);
        Task<Tutor?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}