using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using backend.Domain.Auth.Extensions;
using backend.Domain.Usuarios.Interfaces;

namespace backend.Domain.Usuarios.Controllers
{
    /// <summary>
    /// Meu Painel: página inicial da clínica para o usuário logado, com uma seção por perfil.
    /// </summary>
    [ApiController]
    [Route("api/clinica/meu-painel")]
    public class PainelController : ControllerBase
    {
        private readonly IPainelService _painelService;

        public PainelController(IPainelService painelService)
        {
            _painelService = painelService;
        }

        [HttpGet]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetMeuPainel()
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var usuarioId = User.GetUsuarioId();
            if (usuarioId == null) return Forbid();

            var perfisClaim = User.FindFirst("Perfis")?.Value;
            var perfis = string.IsNullOrEmpty(perfisClaim)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(perfisClaim) ?? new List<string>();

            var result = await _painelService.ObterMeuPainelAsync(clinicaId, usuarioId.Value, perfis);
            if (!result.Succeeded) return NotFound(result);
            return Ok(result.Data);
        }
    }
}
