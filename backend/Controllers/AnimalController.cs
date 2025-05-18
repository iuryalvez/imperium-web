using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Animal;
using backend.Interfaces;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class AnimalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnimalController> _logger;
        public AnimalController(AppDbContext context,
                                ILogger<AnimalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtém um animal por ID
        [HttpGet("animais/{animalId}")]
        [Authorize(Policy = "VisualizaTudo")]
        public async Task<IActionResult> GetAnimalById(int animalId)
        {
            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }

            return Ok(animal);
        }

        [HttpPost("animais/cadastrar")]
        [Authorize(Policy = "GerenciaAnimal")]
        public async Task<IActionResult> CadastrarAnimal([FromBody] CadastrarAnimalClinicaDto cadastrarAnimalClinicaDto)
        {
            var animal = new Animal
            {
                Nome = cadastrarAnimalClinicaDto.Nome,
                Especie = cadastrarAnimalClinicaDto.Especie,
                Nascimento = cadastrarAnimalClinicaDto.Nascimento,
                DataCad = DateTime.UtcNow,
                DataModificado = DateTime.UtcNow,
                Sexo = cadastrarAnimalClinicaDto.Sexo,
                Castrado = cadastrarAnimalClinicaDto.Castrado,
                Peso = cadastrarAnimalClinicaDto.Peso,
                CondicaoCorporal = cadastrarAnimalClinicaDto.CondicaoCorporal,
                GrauAtividade = cadastrarAnimalClinicaDto.GrauAtividade,
                Gestacao = cadastrarAnimalClinicaDto.Gestacao,
                Lactacao = cadastrarAnimalClinicaDto.Lactacao,
                SemanaLactacao = cadastrarAnimalClinicaDto.SemanaLactacao,
                SemanaGestacao = cadastrarAnimalClinicaDto.SemanaGestacao
            };

            _context.Animais.Add(animal);
            await _context.SaveChangesAsync();

            // Obter o valor da claim como inteiro
            var clinicaId = User.GetClaimValue<int>("clinicaId");

            if (clinicaId == 0) // Considerando 0 como default para int
            {
                return BadRequest("Claim 'clinicaId' não encontrada ou inválida.");
            }

            Console.WriteLine($"Acesso permitido à clínica {clinicaId}");

            var vinculoAnimalClinica = new AnimalClinica
            {
                AnimalId = animal.Id,
                ClinicaId = clinicaId
            };

            _context.AnimaisClinicas.Add(vinculoAnimalClinica);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Animal criado com sucesso.", AnimalId = animal.Id });
        }

        // Atualiza um animal de uma clínica
        [HttpPut("animais/{animalId}/editar")]
        [Authorize(Policy = "GerenciaAnimais")]
        public async Task<IActionResult> UpdateAnimal(int animalId, [FromBody] EditarAnimalClinicaDto editarAnimalClinicaDto)
        {
            var animal = await _context.Animais.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal não encontrado.");
            }

            animal.Nome = editarAnimalClinicaDto.Nome;
            animal.Especie = editarAnimalClinicaDto.Especie;
            animal.Nascimento = editarAnimalClinicaDto.Nascimento;
            animal.DataModificado = DateTime.UtcNow;
            animal.Sexo = editarAnimalClinicaDto.Sexo;
            animal.Castrado = editarAnimalClinicaDto.Castrado;
            animal.Peso = editarAnimalClinicaDto.Peso;
            animal.CondicaoCorporal = editarAnimalClinicaDto.CondicaoCorporal;
            animal.GrauAtividade = editarAnimalClinicaDto.GrauAtividade;
            animal.Gestacao = editarAnimalClinicaDto.Gestacao;
            animal.Lactacao = editarAnimalClinicaDto.Lactacao;
            animal.SemanaLactacao = editarAnimalClinicaDto.SemanaLactacao;
            animal.SemanaGestacao = editarAnimalClinicaDto.SemanaGestacao;

            _context.Animais.Update(animal);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Animal atualizado com sucesso." });
        }

        // Deleta um animal em uma clínica
        [HttpDelete("animais/{animalId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteAnimal(int animalId)
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
