using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Mappers;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.Usuario;

namespace backend.Controllers
{
    [Route("api/usuarios")] // ou api/[controller], mas o "[controller]" é capitalizado
    [ApiController] // bring all this for us (?)
    public class UsuarioController : ControllerBase // ControllerBase: way easier to bring atributes later
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
        public UsuarioController(AppDbContext context, ILogger<UsuarioController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = (await _context.Usuarios.ToListAsync())
                .Select(s => s.ToUsuarioDto());
            
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
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
        public IActionResult Create([FromBody] CreateUsuarioRequestDto usuarioDto)
        {
            try
            {
                var usuarioModel = usuarioDto.ToUsuarioFromCreateDTO();
                _context.Usuarios.Add(usuarioModel);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = usuarioModel.Id}, usuarioModel.ToUsuarioDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar o usuário.");
                return StatusCode(500, "Ocorreu um erro no servidor.");
            }
        }

        [HttpPut]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateUsuarioRequestDto updateDto)
        {
            var usuarioModel = _context.Usuarios.FirstOrDefault(x => x.Id == id);
            if (usuarioModel == null)
            {
                return NotFound();
            }

            usuarioModel.Nome = updateDto.Nome;
            usuarioModel.Email = updateDto.Email;
            usuarioModel.Formacao = updateDto.Formacao;   
            usuarioModel.Ativo = updateDto.Ativo;
            usuarioModel.DataUltimoAcesso = updateDto.DataUltimoAcesso;

            _context.SaveChanges();

            return Ok(usuarioModel.ToUsuarioDto());
        }
    }
}
