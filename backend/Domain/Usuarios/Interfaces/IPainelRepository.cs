using backend.Domain.Usuarios.DTOs.Painel;
using backend.Domain.Usuarios.Entities;

namespace backend.Domain.Usuarios.Interfaces
{
    public interface IPainelRepository
    {
        Task<Usuario?> GetUsuarioAsync(int usuarioId);
        Task<string?> GetNomeClinicaAsync(int clinicaId);
        Task<ResumoConsultasDto> ResumirConsultasMarcadasAsync(int clinicaId, int usuarioId);
        Task<ResumoVeterinarioDto> ResumirVeterinarioAsync(int clinicaId, int usuarioId);
    }
}
