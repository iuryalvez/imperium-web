using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Domain.Auth.Extensions;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Interface;

// TODO: Ver possibilidade de refatorar a lógica da persistência dos dados dos tutores:
//  -> manter o tutor cadastrado no banco e no cadastro de tutor ver se já existe um cadastro
//     proveniente de outra clínica para aproveitar dados antigos/existentes de outras clínicas

namespace backend.Domain.AtendimentoAnimal
{
    [ApiController]
    [Route("api/clinica/tutores")]
    public class TutorController : ControllerBase
    {
        private readonly ITutorService _tutorService;
        public TutorController(ITutorService tutorService) { _tutorService = tutorService; }

        [HttpGet]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchNome = null)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var tutores = await _tutorService.ObterTodosDtosPeloClinicaIdAsync(clinicaId, page, pageSize, searchNome);
            return Ok(tutores);
        }

        [HttpGet("{tutorId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetById(int tutorId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var tutor = await _tutorService.ObterDtoPorIdEClinicaId(tutorId, clinicaId);
            if (tutor == null) return NotFound();
            return Ok(tutor);
        }

        [HttpPost("cadastrar")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> Create([FromBody] CadastrarTutorClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _tutorService.CreateAsync(dto, clinicaId);
            if (!result.Succeeded) return Conflict(result); // Conflict (409) é bom para "CPF já existe"

            return Ok(result);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> Update([FromBody] EditarTutorClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _tutorService.UpdateAsync(dto, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return NoContent();
        }

        [HttpPut("{tutorId}/status")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> SetStatus(int tutorId, [FromQuery] bool ativo)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _tutorService.SetTutorStatusAsync(tutorId, clinicaId, ativo);

            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{tutorId}")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> Delete(int tutorId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _tutorService.DeleteAsync(tutorId, clinicaId);
            if (!result.Succeeded) return BadRequest(result);
            return NoContent();
        }
    }
}