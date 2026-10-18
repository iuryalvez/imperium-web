using Microsoft.AspNetCore.Mvc;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using Microsoft.AspNetCore.Authorization;
using backend.Domain.AtendimentoAnimal.Services;
using backend.Domain.Usuarios.Interfaces;
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
        [Authorize(Policy = "GerenciaAnimais")]
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

            // Se a criação falhar por dados inválidos, retorna 400.
            return BadRequest(result);
        }

        [HttpPut("{dietaId}")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> UpdateDieta([FromRoute] int dietaId, [FromBody] UpdateDietaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _acompanhamentoService.UpdateDietaAsync(dietaId, dto, clinicaId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{dietaId}")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> DeleteDieta([FromRoute] int dietaId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _acompanhamentoService.DeleteDietaAsync(dietaId, clinicaId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("{id}/iniciar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Iniciar([FromRoute] int id)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.IniciarAsync(id, clinicaId);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }

        [HttpPatch("{id}/concluir")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Concluir([FromRoute] int id)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.ConcluirAsync(id, clinicaId);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("{id}/registros-peso")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> RegistrarPeso([FromRoute] int id, [FromBody] CreateRegistroPesoDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.RegistrarPesoAsync(id, dto, clinicaId);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}/registros-peso/{registroId}")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> RemoverRegistroPeso([FromRoute] int id, [FromRoute] int registroId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _dietaService.RemoverRegistroPesoAsync(id, registroId, clinicaId);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
    }
}
