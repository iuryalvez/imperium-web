using Microsoft.AspNetCore.Mvc;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using Microsoft.AspNetCore.Authorization;
using backend.Domain.AtendimentoAnimal.Services;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.Auth.Extensions;
using backend.Domain.SaudeAnimal.Interfaces;

namespace backend.Domain.SaudeAnimal.Controllers
{
    [ApiController]
    [Route("api/clinica/dietas")]
    [Authorize]
    public class DietaController : ControllerBase
    {
        private readonly IAcompanhamentoService _acompanhamentoService;
        private readonly IDietaService _dietaService;

        public DietaController(IAcompanhamentoService acompanhamentoService, IDietaService dietaService)
        {
            _acompanhamentoService = acompanhamentoService;
            _dietaService = dietaService;
        }

        /// <summary>
        /// Cria ou atualiza uma dieta para um relatório.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "GerenciaAcompanhamento")]
        [ProducesResponseType(typeof(ServiceResult), 200)]
        [ProducesResponseType(typeof(ServiceResult), 400)]
        public async Task<IActionResult> CriarDieta([FromBody] CreateDietaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _acompanhamentoService.CriarDietaParaAvaliacaoAsync(dto.AcompanhamentoId, dto, clinicaId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            // 400 para dados inválidos; 404 quando a dieta não existe nesta clínica
            return this.Falha(result);
        }

        [HttpPut("{dietaId}")]
        [Authorize(Policy = "GerenciaAcompanhamento")]
        public async Task<IActionResult> UpdateDieta([FromRoute] int dietaId, [FromBody] UpdateDietaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _acompanhamentoService.UpdateDietaAsync(dietaId, dto, clinicaId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return this.Falha(result);
        }

        [HttpDelete("{dietaId}")]
        [Authorize(Policy = "GerenciaAcompanhamento")]
        public async Task<IActionResult> DeleteDieta([FromRoute] int dietaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _acompanhamentoService.DeleteDietaAsync(dietaId, clinicaId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return this.Falha(result);
        }

        [HttpPatch("{id}/iniciar")]
        [Authorize(Policy = "GerenciaAcompanhamento")]
        public async Task<IActionResult> Iniciar([FromRoute] int id)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.IniciarAsync(id, clinicaId);
            if (result.Succeeded) return Ok(result);
            return this.Falha(result);
        }

        [HttpPatch("{id}/concluir")]
        [Authorize(Policy = "GerenciaAcompanhamento")]
        public async Task<IActionResult> Concluir([FromRoute] int id)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.ConcluirAsync(id, clinicaId);
            if (result.Succeeded) return Ok(result);
            return this.Falha(result);
        }

        [HttpPost("{id}/registros-peso")]
        [Authorize(Policy = "GerenciaAcompanhamento")]
        public async Task<IActionResult> RegistrarPeso([FromRoute] int id, [FromBody] CreateRegistroPesoDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.RegistrarPesoAsync(id, dto, clinicaId);
            if (result.Succeeded) return Ok(result);
            return this.Falha(result);
        }

        [HttpDelete("{id}/registros-peso/{registroId}")]
        [Authorize(Policy = "GerenciaAcompanhamento")]
        public async Task<IActionResult> RemoverRegistroPeso([FromRoute] int id, [FromRoute] int registroId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.RemoverRegistroPesoAsync(id, registroId, clinicaId);
            if (result.Succeeded) return Ok(result);
            return this.Falha(result);
        }
    }
}
