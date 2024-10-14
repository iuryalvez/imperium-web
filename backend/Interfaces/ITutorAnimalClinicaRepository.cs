using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.TutorAnimalClinica;
using backend.Migrations;

namespace backend.Interfaces
{
    public interface ITutorTutorAnimalClinicaClinicaRepository
    {
        Task<List<TutorAnimalClinica>> GetAllAsync();
        Task<TutorAnimalClinica?> GetByIdAsync(int id); // FirstOrDefault pode ser nulo
        Task<TutorAnimalClinica> CreateAsync(TutorAnimalClinica tutorAnimalClinicaModel);
        Task<TutorAnimalClinica?> UpdateAsync(int id, UpdateTutorAnimalClinicaRequestDto animalDto);
        Task<TutorAnimalClinica?> DeleteAsync(int id); // FirstOrDefault pode ser nulo
    }
}