using Microsoft.AspNetCore.Mvc;

namespace backend.Domain.Utils
{
    /// <summary>
    /// Converte a falha de um ServiceResult no status HTTP, pelo campo <see cref="ServiceResult.Falha"/>:
    /// NaoEncontrado = 404 (registro inexistente ou de outra clínica), Conflito = 409 (duplicado ou em uso)
    /// e sem tipo = 400 (regra ou validação). O corpo é sempre o próprio ServiceResult.
    /// Uso no controller: <c>if (!result.Succeeded) return this.Falha(result);</c>
    /// </summary>
    public static class ServiceResultHttpExtensions
    {
        public static IActionResult Falha(this ControllerBase controller, ServiceResult result) => result.Falha switch
        {
            TipoFalha.NaoEncontrado => controller.NotFound(result),
            TipoFalha.Conflito => controller.Conflict(result),
            _ => controller.BadRequest(result),
        };
    }
}
