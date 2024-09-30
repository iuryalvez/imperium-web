using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Mappers;
using backend.Dtos.Perfil;
using backend.Interfaces;

namespace backend.Controllers
{
    [Route("api/perfis")] // ou api/[controller], mas o "[controller]" é capitalizado
    [ApiController]
    public class PerfilController : ControllerBase // ControllerBase: traz os atributos de controladores mais fácil dps
    {
        private readonly ILogger<PerfilController> _logger;
        private readonly AppDbContext _context;
        private readonly IPerfilRepository _perfilRepo;
        public PerfilController(AppDbContext context, ILogger<PerfilController> logger, IPerfilRepository perfilRepo)
        {
            _context = context;
            _logger = logger;
            _perfilRepo = perfilRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() // async é um identificador, task é o que envolve a função (wrapper)
        {
            var perfils = await _perfilRepo.GetAllAsync(); // await é o que faz a função ser async
            
            var perfilDto = perfils.Select(u => u.ToPerfilDto());
            
            return Ok(perfils);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var perfil = await _perfilRepo.GetByIdAsync(id);
                if (perfil == null)
                {
                    return NotFound();
                }
                return Ok(perfil);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter o usuário com ID {Id}", id);
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePerfilRequestDto perfilDto)
        {
            try
            {
                var perfilModel = perfilDto.ToPerfilFromCreateDTO();
                await _perfilRepo.CreateAsync(perfilModel);
                return CreatedAtAction(nameof(GetById), new { id = perfilModel.Id}, perfilModel.ToPerfilDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar o usuário.");
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePerfilRequestDto updateDto)
        {
            var perfilModel = await _perfilRepo.UpdateAsync(id, updateDto);
            if (perfilModel == null)
            {
                return NotFound();
            }

            return Ok(perfilModel.ToPerfilDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var perfilModel = await _perfilRepo.DeleteAsync(id);

            if(perfilModel == null) 
            {
                return NotFound();
            }

            return NoContent(); // a success in delete method
        }
    }
}
