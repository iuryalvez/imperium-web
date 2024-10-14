using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.DietaPersonalizada;
using backend.Models;

namespace backend.Interfaces
{
    public interface IDietaPersonalizadaRepository
    {
        Task<List<DietaPersonalizada>> GetAllAsync();
        Task<DietaPersonalizada?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<DietaPersonalizada> CreateAsync(DietaPersonalizada dietaPersonalizadaModel);
        Task<DietaPersonalizada?> UpdateAsync(int id, UpdateDietaPersonalizadaRequestDto animalDto);
        Task<DietaPersonalizada?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}