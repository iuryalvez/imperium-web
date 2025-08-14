using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using backend.Services;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.SaudeAnimal
{
    [ApiController]
    [Route("api/clinica/racoes")]
    public class RacaoController : ControllerBase
    {
        private readonly IRacaoService _racaoService;
        public RacaoController(IRacaoService racaoService) { _racaoService = racaoService; }

        [HttpGet("{racaoId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetRacaoById(int racaoId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) return Forbid();

            var racao = await _racaoService.GetByIdAsync(racaoId, clinicaId);

            if (racao == null) return NotFound(new { Message = "Ração não encontrada." });

            return Ok(racao);
        }

        [HttpGet]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAllRacoes([FromQuery] EspecieAnimal? especie) // Recebe o parâmetro
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) return Forbid();

            // Passa o parâmetro para o serviço
            var racoes = await _racaoService.GetAllByClinicIdAsync(clinicaId, especie);
            return Ok(racoes);
        }

        [HttpPost("cadastrar")]
        [Authorize(Policy = "VisualizaTudo")] // Ajuste a política conforme necessário
        public async Task<IActionResult> CreateRacao([FromBody] CadastrarRacaoDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) return BadRequest("Acesso não permitido.");

            var result = await _racaoService.CreateAsync(dto, clinicaId);
            if (!result.Succeeded) return BadRequest(result);

            // Retorna o objeto criado e a rota para acessá-lo
            return CreatedAtAction(nameof(GetRacaoById), new { racaoId = ((GetRacoesDto)result.Data).Id }, result.Data);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> UpdateRacao([FromBody] AtualizarRacaoDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) return Forbid();

            var result = await _racaoService.UpdateAsync(dto, clinicaId);

            if (!result.Succeeded) return NotFound(result); // NotFound é mais apropriado se a ração não foi encontrada

            return NoContent(); // Resposta padrão para um PUT/PATCH bem-sucedido
        }

        [HttpDelete("{racaoId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> DeleteRacao(int racaoId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) return Forbid();

            var result = await _racaoService.DeleteAsync(racaoId, clinicaId);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return NoContent();
        }
    }
}