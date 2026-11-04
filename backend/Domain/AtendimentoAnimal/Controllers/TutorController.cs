using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Domain.Auth.Extensions;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Utils;

// TODO: Ver possibilidade de refatorar a lógica da persistência dos dados dos tutores:
//  -> manter o tutor cadastrado no banco e no cadastro de tutor ver se já existe um cadastro
//     proveniente de outra clínica para aproveitar dados antigos/existentes de outras clínicas

namespace backend.Domain.AtendimentoAnimal.Controllers
{
    [ApiController]
    [Route("api/clinica/tutores")]
    public class TutorController : ControllerBase
    {
        private readonly ITutorService _tutorService;
        public TutorController(ITutorService tutorService) { _tutorService = tutorService; }

        [HttpGet]
        [Authorize(Policy = "VisualizaAtendimento")]
        public async Task<IActionResult> GetAll([FromQuery] TutorFiltro filtro)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            return Ok(await _tutorService.ListarAsync(filtro, clinicaId));
        }

        [HttpGet("opcoes")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetOpcoes([FromQuery] string? busca)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId <= 0) return Forbid();
            // Usado também no formulário de animal pelo TECNICO, que vê só o nome do tutor
            return Ok(await _tutorService.ListarOpcoesAsync(clinicaId, busca, User.PodeVerDadosDoTutor()));
        }

        [HttpGet("{tutorId}")]
        [Authorize(Policy = "VisualizaAtendimento")]
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
            if (!result.Succeeded) return Falha(result);

            return Ok(result);
        }

        [HttpPut("editar")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> Update([FromBody] EditarTutorClinicaDto dto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _tutorService.UpdateAsync(dto, clinicaId);
            if (!result.Succeeded) return Falha(result);
            return NoContent();
        }

        [HttpPut("{tutorId}/status")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> SetStatus(int tutorId, [FromQuery] bool ativo)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _tutorService.SetTutorStatusAsync(tutorId, clinicaId, ativo);

            if (!result.Succeeded) return Falha(result);
            return Ok(result);
        }

        [HttpDelete("{tutorId}")]
        [Authorize(Policy = "GerenciaTutores")]
        public async Task<IActionResult> Delete(int tutorId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            var result = await _tutorService.DeleteAsync(tutorId, clinicaId);
            if (!result.Succeeded) return Falha(result);
            return NoContent();
        }

        /// <summary>
        /// Converte a falha do service no status HTTP: 404 para tutor inexistente ou de outra clínica,
        /// 409 para CPF já cadastrado na clínica e 400 para as demais regras. O corpo é sempre o ServiceResult.
        /// </summary>
        private IActionResult Falha(ServiceResult result) => result.Falha switch
        {
            TipoFalha.NaoEncontrado => NotFound(result),
            TipoFalha.Conflito => Conflict(result),
            _ => BadRequest(result),
        };
    }
}