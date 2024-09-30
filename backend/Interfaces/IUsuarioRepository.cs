using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Usuario;
using backend.Models;

namespace backend.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<Usuario> CreateAsync(Usuario usuarioModel);
        Task<Usuario?> UpdateAsync(int id, UpdateUsuarioRequestDto usuarioDto);
        Task<Usuario?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}