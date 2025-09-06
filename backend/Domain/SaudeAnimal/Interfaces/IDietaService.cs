using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IDietaService
    {
        Task<DietaPersonalizada> CalcularOuAtualizarDietaAsync(
            Animal animal,
            Relatorio relatorio,
            Racao racao,
            CreateDietaDto dto);
    }
}