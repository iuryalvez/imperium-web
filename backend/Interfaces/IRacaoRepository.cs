using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Racao;
using backend.Models;

namespace backend.Interfaces
{
    public interface IRacaoRepository
    {
        Task<List<Racao>> GetAllAsync();
        Task<Racao?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<Racao> CreateAsync(Racao animalModel);
        Task<Racao?> UpdateAsync(int id, UpdateRacaoRequestDto animalDto);
        Task<Racao?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}