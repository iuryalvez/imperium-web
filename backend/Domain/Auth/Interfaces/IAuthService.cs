using backend.Domain.Auth.DTOs;
using backend.Domain.Usuarios.DTOs;
using backend.Domain.Usuarios.Interfaces;

namespace backend.Domain.Auth.Interfaces
{
    public interface IAuthService
    {   
        Task<ServiceResult> LoginAsync(LoginDto model, string remoteIp, string userAgent);
        Task<ServiceResult> GetVinculosAsync(string userIdClaim);
        Task<ServiceResult> SelecionarVinculoAsync(int upcId, string userIdClaim, bool isAcessoLogin, string remoteIp, string userAgent);
        Task<ServiceResult> SelecionarVinculoAdminAsync(GetVinculoAdminDto dto, string userIdClaim, string remoteIp, string userAgent, string usuarioNome);
        Task<ServiceResult> TrocarVinculoAsync(string userIdClaim, bool isAcessoLogin, bool isAdmin, string remoteIp, string userAgent);
    }
}