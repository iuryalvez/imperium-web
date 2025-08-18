using backend.Domain.GestaoDeAcesso.DTOs;

namespace backend.Domain.GestaoDeAcesso.Interfaces
{
    public interface IUsuarioService
    {
        // Usaremos uma classe de resultado para comunicar sucesso ou falha ao controller
        Task<ServiceResult> ListarUsuariosSistemaAsync(int currentUserId);
        Task<ServiceResult> CadastrarUsuarioSistemaAsync(CadastrarUsuarioSistemaDto dto);
        Task<IEnumerable<GetUsuariosClinicaDto>> GetUsuariosByClinicIdAsync(int clinicaId, string currentUserId);
        Task<ServiceResult> UpdateUsuarioSistemaAsync(EditarUsuarioSistemaDto dto);
        Task<ServiceResult> CadastrarUsuarioClinicaAsync(CadastrarUsuarioClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateUsuarioClinicaAsync(EditarUsuarioClinicaDto dto);
        Task<ServiceResult> UnlinkUserFromClinicAsync(int upcId);
        Task<ServiceResult> SetUserActivationStatusAsync(SetUserStatusDto dto);
    }
}