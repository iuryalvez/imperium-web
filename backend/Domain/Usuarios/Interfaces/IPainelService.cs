using backend.Domain.Utils;

namespace backend.Domain.Usuarios.Interfaces
{
    public interface IPainelService
    {
        Task<ServiceResult> ObterMeuPainelAsync(int clinicaId, int usuarioId, List<string> perfis);
    }
}
