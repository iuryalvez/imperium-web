using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.UsuarioPerfilClinica;
using backend.Models;

namespace backend.Interfaces
{
    public interface IUsuarioPerfilClinicaRepository
    {
        Task<List<UsuarioPerfilClinica>> GetAllAsync();
        Task<UsuarioPerfilClinica?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<UsuarioPerfilClinica> CreateAsync(UsuarioPerfilClinica usuarioPerfilClinicaModel);
        Task<UsuarioPerfilClinica?> UpdateAsync(int id, UpdateUsuarioPerfilClinicaRequestDto animalDto);
        Task<UsuarioPerfilClinica?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}