using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Animal;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class AnimalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public AnimalController(AppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Cria um novo animal na clínica
        [HttpPost("{clinicaId}/animais/tutor_{tutorId}-novo-animal")]
        [Authorize(Policy = "GerenciaAnimal")]
        public async Task<IActionResult> CreateAnimal(int clinicaId, int tutorId, [FromBody] AnimalDto animalDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var animal = new Animal
            {
                Nome = animalDto.Nome,
                Especie = animalDto.Especie,
                Nascimento = animalDto.Nascimento,
                DataCad = DateTime.UtcNow,
                DataCadModificado = DateTime.UtcNow,
                Sexo = animalDto.Sexo,
                Castrado = animalDto.Castrado,
                Peso = animalDto.Peso,
                CondicaoCorporal = animalDto.CondicaoCorporal,
                GrauAtividade = animalDto.GrauAtividade,
                Gestacao = animalDto.Gestacao,
                Lactacao = animalDto.Lactacao,
                SemanaLactacao = animalDto.SemanaLactacao,
                SemanaGestacao = animalDto.SemanaGestacao
            };

            _context.Animais.Add(animal);
            await _context.SaveChangesAsync();

            var tutorAnimalClinica = new TutorAnimalClinica
            {
                TutorId = tutorId,
                AnimalId = animal.Id,
                ClinicaId = clinicaId
            };

            _context.TutoresAnimaisClinicas.Add(tutorAnimalClinica);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Animal criado com sucesso.", AnimalId = animal.Id });
        }

        // Obtém um animal por ID
        [HttpGet("{clinicaId}/animais/{animalId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAnimalById(int clinicaId, int animalId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }

            return Ok(animal);
        }

        // Obtém animais por tutorId
        [HttpGet("{clinicaId}/animais/tutor_{tutorId}/animais")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<ActionResult<IEnumerable<GetAnimaisDto>>> GetAnimaisByTutorId(int clinicaId, int tutorId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var animais = await _context.TutoresAnimaisClinicas
                .Where(tac => tac.TutorId == tutorId && tac.ClinicaId == clinicaId)
                .Select(tac => new GetAnimaisDto
                {
                    Id = tac.Animal.Id,
                    Nome = tac.Animal.Nome,
                    Especie = tac.Animal.Especie,
                    Sexo = tac.Animal.Sexo
                })
                .ToListAsync();

            if (animais == null || !animais.Any())
            {
                return NotFound("Nenhum animal encontrado para o tutor especificado.");
            }

            return Ok(animais);
        }

        // Atualiza um animal de uma clínica
        [HttpPut("{clinicaId}/animais/{animalId}/editar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> UpdateAnimal(int clinicaId, int animalId, [FromBody] AnimalDto animalDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }

            animal.Nome = animalDto.Nome;
            animal.Especie = animalDto.Especie;
            animal.Nascimento = animalDto.Nascimento;
            animal.DataCadModificado = DateTime.UtcNow;
            animal.Sexo = animalDto.Sexo;
            animal.Castrado = animalDto.Castrado;
            animal.Peso = animalDto.Peso;
            animal.CondicaoCorporal = animalDto.CondicaoCorporal;
            animal.GrauAtividade = animalDto.GrauAtividade;
            animal.Gestacao = animalDto.Gestacao;
            animal.Lactacao = animalDto.Lactacao;
            animal.SemanaLactacao = animalDto.SemanaLactacao;
            animal.SemanaGestacao = animalDto.SemanaGestacao;

            _context.Animais.Update(animal);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Animal atualizado com sucesso." });
        }

        // Deleta um animal em uma clínica
        [HttpDelete("{clinicaId}/animais/{animalId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteAnimal(int clinicaId, int animalId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }

            _context.Animais.Remove(animal);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Animal deletado com sucesso." });
        }
    }
}
