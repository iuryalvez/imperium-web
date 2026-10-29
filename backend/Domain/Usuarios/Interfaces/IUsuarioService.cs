using backend.Domain.Utils.Paginacao;
using backend.Domain.Usuarios.DTOs.Usuario;
using backend.Domain.Usuarios.DTOs.Vinculo;
using backend.Domain.Usuarios.DTOs.PerfilPessoal;
using backend.Domain.Utils;
using backend.Domain.Usuarios.DTOs.UsuarioStatus;

namespace backend.Domain.Usuarios.Interfaces
{
    public interface IUsuarioService
    {
        // Usaremos uma classe de resultado para comunicar sucesso ou falha ao controller
        Task<ServiceResult> ListarUsuariosSistemaAsync(int currentUserId);
        Task<ServiceResult> CadastrarUsuarioSistemaAsync(CadastrarUsuarioSistemaDto dto);
        Task<PaginaResultado<GetUsuariosClinicaDto>> ListarUsuariosDaClinicaAsync(UsuarioClinicaFiltro filtro, int clinicaId, int? upcIdAtual);
        Task<ServiceResult> UpdateUsuarioSistemaAsync(EditarUsuarioSistemaDto dto);
        Task<ServiceResult> CadastrarUsuarioClinicaAsync(CadastrarUsuarioClinicaDto dto, int clinicaId);
        Task<ServiceResult> UpdateUsuarioClinicaAsync(EditarUsuarioClinicaDto dto, int clinicaId);
        Task<ServiceResult> UnlinkUserFromClinicAsync(int upcId);
        Task<ServiceResult> SetUserActivationStatusAsync(SetUserStatusDto dto);
        Task<ServiceResult> GetMeuPerfilAsync(int usuarioId);
        Task<ServiceResult> UpdateMeuPerfilAsync(int usuarioId, UpdateMeuPerfilDto dto);
    }
}