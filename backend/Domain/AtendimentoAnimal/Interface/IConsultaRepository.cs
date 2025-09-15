using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IConsultaRepository
    {
        Task<Consulta?> GetByIdAsync(int consultaId);
        Task<Consulta?> GetByIdWithDetailsAsync(int consultaId);
        Task<IEnumerable<Consulta>> GetAllByClinicIdAsync(int clinicaId);
        Task<Consulta?> GetByIdAndClinicIdAsync(int consultaId, int clinicaId);
        Task AddAsync(Consulta consulta);
        void Delete(Consulta consulta);
        Task<int> SaveChangesAsync();
    }
}