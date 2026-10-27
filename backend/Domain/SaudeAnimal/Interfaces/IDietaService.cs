using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;

namespace backend.Domain.SaudeAnimal.Interfaces
{
    public interface IDietaService
    {
        Task<Dieta> CalcularOuAtualizarDietaAsync(
            Animal animal,
            Avaliacao avaliacao,
            Racao racao,
            CreateDietaDto dto);

        Task<GetDietaDto> GetByIdAsync(int dietaId, int clinicaId);
        Task<IEnumerable<GetDietaDto>> GetAllByAcompanhamentoAsync(int acompanhamentoId, int clinicaId);
        Task<ServiceResult> UpdateAsync(int dietaId, UpdateDietaDto dto, int clinicaId, Racao racao);
        Task<ServiceResult> DeleteAsync(int dietaId, int clinicaId);
        Task<ServiceResult> IniciarAsync(int dietaId, int clinicaId);
        Task<ServiceResult> ConcluirAsync(int dietaId, int clinicaId);
        Task<ServiceResult> RegistrarPesoAsync(int dietaId, CreateRegistroPesoDto dto, int clinicaId);
        Task<ServiceResult> RemoverRegistroPesoAsync(int dietaId, int registroId, int clinicaId);
    }
}