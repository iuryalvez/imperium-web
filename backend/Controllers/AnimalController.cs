using backend.Data;
using backend.Dtos.Animal;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController : ControllerBase
    {
        private readonly ILogger<AnimalController> _logger;
        private readonly AppDbContext _context;
        private readonly IAnimalRepository _animalRepo;
        public AnimalController(AppDbContext context, ILogger<AnimalController> logger, IAnimalRepository animalRepo)
        {
            _context = context;
            _logger = logger;
            _animalRepo = animalRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() // async é um identificador, task é o que envolve a função (wrapper)
        {
            var animais = await _animalRepo.GetAllAsync(); // await é o que faz a função ser async
            
            var animalDto = animais.Select(animal => animal.ToAnimalDto());
            
            return Ok(animais);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var animal = await _animalRepo.GetByIdAsync(id);
                if (animal == null)
                {
                    return NotFound();
                }
                return Ok(animal.ToAnimalDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter o usuário com ID {Id}", id);
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnimalRequestDto animalDto)
        {
            try
            {
                var animalModel = animalDto.ToAnimalFromCreateDTO();
                await _animalRepo.CreateAsync(animalModel);
                return CreatedAtAction(nameof(GetById), new { id = animalModel.Id}, animalModel.ToAnimalDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar o usuário.");
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAnimalRequestDto updateDto)
        {
            var animalModel = await _animalRepo.UpdateAsync(id, updateDto);
            if (animalModel == null)
            {
                return NotFound();
            }

            return Ok(animalModel.ToAnimalDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var animalModel = await _animalRepo.DeleteAsync(id);

            if(animalModel == null) 
            {
                return NotFound();
            }

            return NoContent(); // a success in delete method
        }
    }
}