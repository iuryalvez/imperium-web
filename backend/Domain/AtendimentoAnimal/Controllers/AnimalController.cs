using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using backend.Domain.Auth.Extensions;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.GestaoClinica.Interfaces;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Utils;

// TODO: Refatorar e usar lógica de Interface + Service (impl) + Repository se houver tempo

namespace backend.Domain.AtendimentoAnimal.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class AnimalController : ControllerBase
    {
        private readonly ILogger<AnimalController> _logger;
        private readonly IClinicaService _clinicaService;
        private readonly IAnimalService _animalService;
        public AnimalController(ILogger<AnimalController> logger,
                                IClinicaService clinicaInterface,
                                IAnimalService animalService)
        {
            _logger = logger;
            _clinicaService = clinicaInterface;
            _animalService = animalService;
        }

        [HttpGet("animais")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetByClinicaId([FromQuery] AnimalFiltro filtro)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            return Ok(await _animalService.ListarAsync(filtro, clinicaId));
        }

        [HttpGet("animais/opcoes")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetOpcoes([FromQuery] string? busca)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            return Ok(await _animalService.ListarOpcoesAsync(clinicaId, busca));
        }

        [HttpGet("animais/{animalId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetByIdWithDetails(int animalId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _animalService.GetDtoDetailsByIdAsync(animalId, clinicaId);

            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpPost("animais/cadastrar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Create([FromBody] CadastrarAnimalClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _animalService.CreateAsync(dto, clinicaId);

            if (!result.Succeeded) return this.Falha(result);

            return Ok(result);
        }

        [HttpPut("animais/editar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Update([FromBody] EditarAnimalClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _animalService.UpdateAsync(dto, clinicaId);

            if (!result.Succeeded) return this.Falha(result);

            return Ok(result);
        }

        [HttpPut("animais/{animalId}/status")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> SetStatus(int animalId, [FromQuery] bool ativo)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();

            var result = await _animalService.SetStatusAsync(animalId, clinicaId, ativo);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpDelete("animais/{animalId}")] // Rota ajustada para clareza
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> DeleteLink(int animalId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0)
            {
                _logger.LogWarning("Usuário tentou excluir animal sem uma claim 'clinicaId' válida. Claims: {@Claims}", User.Claims.Select(c => new { c.Type, c.Value }));
                return Forbid();
            }

            var result = await _animalService.DeleteAsync(animalId, clinicaId);
            if (!result.Succeeded) return this.Falha(result);

            return Ok(new { Message = result.Message });
        }
    }
}
