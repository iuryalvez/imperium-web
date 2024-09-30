using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Perfil;
using backend.Models;

namespace backend.Interfaces
{
    public interface IPerfilRepository
    {
        Task<List<Perfil>> GetAllAsync();
        Task<Perfil?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<Perfil> CreateAsync(Perfil perfilModel);
        Task<Perfil?> UpdateAsync(int id, UpdatePerfilRequestDto perfilDto);
        Task<Perfil?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}