using backend.Dtos.Animal;
using backend.Models;

namespace backend.Interfaces
{
    public interface IAnimalRepository
    {
        Task<List<Animal>> GetAllAsync();
        Task<Animal?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<Animal> CreateAsync(Animal animalModel);
        Task<Animal?> UpdateAsync(int id, UpdateAnimalRequestDto animalDto);
        Task<Animal?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}