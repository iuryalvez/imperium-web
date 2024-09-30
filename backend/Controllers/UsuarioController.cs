using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Mappers;
using backend.Dtos.Usuario;
using backend.Interfaces;

namespace backend.Controllers
{
    [Route("api/usuarios")] // ou api/[controller], mas o "[controller]" é capitalizado
    [ApiController]
    public class UsuarioController : ControllerBase // ControllerBase: traz os atributos de controladores mais fácil dps
    {
        /* veio com o controlador 
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
        */
        private readonly ILogger<UsuarioController> _logger;
        private readonly AppDbContext _context;
        private readonly IUsuarioRepository _usuarioRepo;
        public UsuarioController(AppDbContext context, ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepo)
        {
            _context = context;
            _logger = logger;
            _usuarioRepo = usuarioRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() // async é um identificador, task é o que envolve a função (wrapper)
        {
            var usuarios = await _usuarioRepo.GetAllAsync(); // await é o que faz a função ser async
            
            var usuarioDto = usuarios.Select(u => u.ToUsuarioDto());
            
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var usuario = await _usuarioRepo.GetByIdAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter o usuário com ID {Id}", id);
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUsuarioRequestDto usuarioDto)
        {
            try
            {
                var usuarioModel = usuarioDto.ToUsuarioFromCreateDTO();
                await _usuarioRepo.CreateAsync(usuarioModel);
                return CreatedAtAction(nameof(GetById), new { id = usuarioModel.Id}, usuarioModel.ToUsuarioDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar o usuário.");
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUsuarioRequestDto updateDto)
        {
            var usuarioModel = await _usuarioRepo.UpdateAsync(id, updateDto);
            if (usuarioModel == null)
            {
                return NotFound();
            }

            return Ok(usuarioModel.ToUsuarioDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var usuarioModel = await _usuarioRepo.DeleteAsync(id);

            if(usuarioModel == null) 
            {
                return NotFound();
            }

            return NoContent(); // a success in delete method
        }
    }
}
