using backend.Domain.Auth.DTOs;
using backend.Domain.Usuarios.DTOs.Vinculo;
using backend.Domain.Utils;

namespace backend.Domain.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult> LoginAsync(LoginDto model, string remoteIp);

        /// <summary>Troca o carimbo de segurança do usuário: todas as sessões dele deixam de valer.</summary>
        Task LogoutAsync(int usuarioId);

        Task<ServiceResult> GetVinculosAsync(int usuarioId);
        Task<ServiceResult> SelecionarVinculoAsync(int upcId, int usuarioId);
        Task<ServiceResult> SelecionarVinculoAdminAsync(GetVinculoAdminDto dto);
        Task<ServiceResult> TrocarVinculoAsync(int usuarioId, bool isAcessoLogin);
        Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto model, string frontendUrl, string remoteIp);
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto model);
    }
}
