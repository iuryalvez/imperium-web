using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Clinica;
using backend.Models;

namespace backend.Interfaces
{
    public interface IClinicaRepository
    {
        Task<List<Clinica>> GetAllAsync();
        Task<Clinica?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<Clinica> CreateAsync(Clinica clinicaModel);
        Task<Clinica?> UpdateAsync(int id, UpdateClinicaRequestDto clinicaDto);
        Task<Clinica?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}