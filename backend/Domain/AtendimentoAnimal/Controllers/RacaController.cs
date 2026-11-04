using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Auth.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Domain.Utils;

namespace backend.Domain.AtendimentoAnimal.Controllers
{
    /// <summary>
    /// Raças vistas pela clínica (tenant): raças do sistema + raças próprias. O clinicaId vem sempre do token.
    /// </summary>
    [ApiController]
    [Route("api/clinica/racas")]
    public class RacaController : ControllerBase
    {
        private readonly IRacaService _racaService;

        public RacaController(IRacaService racaService)
        {
            _racaService = racaService;
        }

        [HttpGet]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAll([FromQuery] RacaFiltro filtro)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            return Ok(await _racaService.ListarDaClinicaAsync(filtro, clinicaId));
        }

        [HttpGet("opcoes")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetOpcoes([FromQuery] string? busca, [FromQuery] EspecieAnimal? especie)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            return Ok(await _racaService.ListarOpcoesDaClinicaAsync(clinicaId, busca, especie));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetById(int id)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            var result = await _racaService.ObterDaClinicaAsync(id, clinicaId);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result.Data);
        }

        [HttpPost("cadastrar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Create([FromBody] CadastrarRacaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            var result = await _racaService.CadastrarDaClinicaAsync(dto, clinicaId);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Update([FromBody] UpdateRacaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            var result = await _racaService.EditarDaClinicaAsync(dto, clinicaId);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> Delete(int id)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            var result = await _racaService.ExcluirDaClinicaAsync(id, clinicaId);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] bool ativo)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            var result = await _racaService.SetStatusDaClinicaAsync(id, clinicaId, ativo);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }

        [HttpPut("{id}/ocultar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> SetOculta(int id, [FromQuery] bool oculta)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            var result = await _racaService.SetOcultaAsync(id, clinicaId, oculta);
            if (!result.Succeeded) return this.Falha(result);
            return Ok(result);
        }
    }
}
