using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface IRacaRepository
    {
        Task<Raca?> GetByIdAsync(int id);
        Task<List<Raca>> ListAllAsync();
        Task<List<Raca>> ListByEspecieAsync(EspecieAnimal especie);
        Task AddAsync(Raca raca);
        void Update(Raca raca); // Update is often synchronous at the model level
        void Delete(Raca raca);
        Task<int> SaveChangesAsync(); // To commit the changes
    }
}