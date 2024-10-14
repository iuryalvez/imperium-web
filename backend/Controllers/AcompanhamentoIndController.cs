using backend.Data;
using backend.Dtos.AcompanhamentoInd;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcompanhamentoIndController : ControllerBase
    {
        private readonly ILogger<AcompanhamentoIndController> _logger;
        private readonly AppDbContext _context;
        private readonly IAcompanhamentoIndRepository _acompanhamentoIndRepo;
        public AcompanhamentoIndController(AppDbContext context, ILogger<AcompanhamentoIndController> logger, IAcompanhamentoIndRepository acompanhamentoIndRepo)
        {
            _context = context;
            _logger = logger;
            _acompanhamentoIndRepo = acompanhamentoIndRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() // async é um identificador, task é o que envolve a função (wrapper)
        {
            var acompanhamentoInds = await _acompanhamentoIndRepo.GetAllAsync(); // await é o que faz a função ser async
            
            var acompanhamentoIndDto = acompanhamentoInds.Select(u => u.ToAcompanhamentoIndDto());
            
            return Ok(acompanhamentoInds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var acompanhamentoInd = await _acompanhamentoIndRepo.GetByIdAsync(id);
                if (acompanhamentoInd == null)
                {
                    return NotFound();
                }
                return Ok(acompanhamentoInd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter o usuário com ID {Id}", id);
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAcompanhamentoIndRequestDto acompanhamentoIndDto)
        {
            try
            {
                var acompanhamentoIndModel = acompanhamentoIndDto.ToAcompanhamentoIndFromCreateDTO();
                await _acompanhamentoIndRepo.CreateAsync(acompanhamentoIndModel);
                return CreatedAtAction(nameof(GetById), new { id = acompanhamentoIndModel.Id}, acompanhamentoIndModel.ToAcompanhamentoIndDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar o usuário.");
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAcompanhamentoIndRequestDto updateDto)
        {
            var acompanhamentoIndModel = await _acompanhamentoIndRepo.UpdateAsync(id, updateDto);
            if (acompanhamentoIndModel == null)
            {
                return NotFound();
            }

            return Ok(acompanhamentoIndModel.ToAcompanhamentoIndDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var acompanhamentoIndModel = await _acompanhamentoIndRepo.DeleteAsync(id);

            if(acompanhamentoIndModel == null) 
            {
                return NotFound();
            }

            return NoContent(); // a success in delete method
        }
    }
}