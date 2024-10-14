using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Relatorio;
using backend.Models;

namespace backend.Interfaces
{
    public interface IRelatorioRepository
    {
        Task<List<Relatorio>> GetAllAsync();
        Task<Relatorio?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<Relatorio> CreateAsync(Relatorio relatorioModel);
        Task<Relatorio?> UpdateAsync(int id, UpdateRelatorioRequestDto animalDto);
        Task<Relatorio?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}