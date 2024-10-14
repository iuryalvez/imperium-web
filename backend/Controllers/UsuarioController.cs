using backend.Dtos.Usuario;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(UserManager<Usuario> userManager) {
            _userManager = userManager;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroDto registroDto) {
            try {
                if(!ModelState.IsValid) return BadRequest(ModelState);
                var usuario = new Usuario {
                    Nome = registroDto.Nome,
                    Email = registroDto.Email
                };

                var createdUsuario = await _userManager.CreateAsync(usuario, registroDto.Senha);
                
                if (createdUsuario.Succeeded) {
                    var roleResult = await _userManager.AddToRoleAsync(usuario, "Veterinário");
                    if (roleResult.Succeeded) {
                        return Ok("Veterinário criado!");
                    } else {
                        return StatusCode(500, roleResult.Errors);
                    }
                } else {
                    return StatusCode(500, createdUsuario.Errors);
                }
            }
            catch (Exception e) {
                return StatusCode(500, e);
            } 
        }

    }
}