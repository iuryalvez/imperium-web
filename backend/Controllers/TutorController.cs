using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Mappers;
using backend.Dtos.Tutor;
using backend.Interfaces;

namespace backend.Controllers
{
    [Route("api/tutores")] // ou api/[controller], mas o "[controller]" é capitalizado
    [ApiController]
    public class TutorController : ControllerBase // ControllerBase: traz os atributos de controladores mais fácil dps
    {
        private readonly ILogger<TutorController> _logger;
        private readonly AppDbContext _context;
        private readonly ITutorRepository _tutorRepo;
        public TutorController(AppDbContext context, ILogger<TutorController> logger, ITutorRepository tutorRepo)
        {
            _context = context;
            _logger = logger;
            _tutorRepo = tutorRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() // async é um identificador, task é o que envolve a função (wrapper)
        {
            var tutores = await _tutorRepo.GetAllAsync(); // await é o que faz a função ser async
            
            var tutorDto = tutores.Select(u => u.ToTutorDto());
            
            return Ok(tutores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var tutor = await _tutorRepo.GetByIdAsync(id);
                if (tutor == null)
                {
                    return NotFound();
                }
                return Ok(tutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter o usuário com ID {Id}", id);
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTutorRequestDto tutorDto)
        {
            try
            {
                var tutorModel = tutorDto.ToTutorFromCreateDTO();
                await _tutorRepo.CreateAsync(tutorModel);
                return CreatedAtAction(nameof(GetById), new { id = tutorModel.Id}, tutorModel.ToTutorDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar o usuário.");
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTutorRequestDto updateDto)
        {
            var tutorModel = await _tutorRepo.UpdateAsync(id, updateDto);
            if (tutorModel == null)
            {
                return NotFound();
            }

            return Ok(tutorModel.ToTutorDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var tutorModel = await _tutorRepo.DeleteAsync(id);

            if(tutorModel == null) 
            {
                return NotFound();
            }

            return NoContent(); // a success in delete method
        }
    }
}
