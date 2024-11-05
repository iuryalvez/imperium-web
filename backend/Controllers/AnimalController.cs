using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Dtos.Animal;
using backend.Models;
using backend.Data;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica/animais")]
    public class AnimalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimalController(AppDbContext context)
        {
            _context = context;
        }

        // Cria um novo animal na clínica
        [HttpPost("{clinicaId}/animais/tutor_{tutorId}-novo-animal")]
        [Authorize("Admin,Veterinario")]
        public async Task<IActionResult> CreateAnimal(int clinicaId, int tutorId, [FromBody] AnimalDto animalDto)
        {
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

            // Cria a associação na tabela TutorAnimalClinica
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
        [Authorize(Roles = "Admin,Gestor,Veterinario,Secretario,Tecnico")]
        public async Task<IActionResult> GetAnimalById(int clinicaId, int animalId)
        {
            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }
            return Ok(animal);
        }

        // Obtém animais por tutorId
        [HttpGet("{clinicaId}/animais/tutor_{tutorId}-animais")]
        [Authorize(Roles = "Admin,Gestor,Veterinario,Secretario,Tecnico")]
        public async Task<IActionResult> GetAnimaisByTutorId(int clinicaId, int tutorId)
        {
            var animais = await _context.TutoresAnimaisClinicas
                .Where(tac => tac.TutorId == tutorId)
                .Where(tac => tac.ClinicaId == clinicaId)
                .Select(tac => tac.Animal)
                .ToListAsync();

            if (animais == null || !animais.Any())
            {
                return NotFound("Nenhum animal encontrado para o tutor especificado.");
            }

            return Ok(animais);
        }

        // Atualiza um animal de uma clínica
        [HttpPut("{clinicaId}/animais/{animalId}/editar")]
        [Authorize(Roles = "Admin,Veterinario")]
        public async Task<IActionResult> UpdateAnimal(int clinicaId, int animalId, [FromBody] AnimalDto animalDto)
        {
            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }

            // Atualiza os campos do animal
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnimal(int clinicaId, int animalId)
        {
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