using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IConsultaRepository
    {
        Task<IEnumerable<Consulta>> GetAllByClinicIdAsync(int clinicaId);
        Task<Consulta?> GetByIdAsync(int consultaId, int clinicaId);
        Task AddAsync(Consulta consulta);
        void Delete(Consulta consulta);
        Task<int> SaveChangesAsync();
    }
}