using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interface
{
    public interface RacaInterface
    {
        Task<List<RacaDto>> ListAllAsync(EspecieAnimal? especie);
        Task<RacaDto?> GetByIdAsync(int id);
        Task<RacaDto> CreateAsync(CriarRacaoDto criarRacaDto);
        Task<bool> UpdateAsync(int id, AtualizarRacaDto atualizarRacaDto);
        Task<bool> DeleteAsync(int id);
    }
}