using backend.Data;
using backend.Dtos.Auth;
using backend.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(AppDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        
        // cria um novo usuário com perfil e associação à clínica
        [HttpPost("novo-usuario/{perfilId}_{clinicaId}")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> CreateUsuario(int perfilId, int clinicaId, [FromBody] UsuarioDto usuarioDto)
        {
            // Verifica se o perfil é válido
            var perfil = await _context.Perfis.FindAsync(perfilId);
            if (perfil == null)
            {
                return BadRequest("Perfil inválido.");
            }

            // Restringe a criação de usuários do tipo ADMIN para apenas administradores
            if (perfil.Name == "ADMIN" && !User.IsInRole("Admin"))
            {
                return Forbid("Apenas administradores podem criar usuários do tipo ADMIN.");
            }

            // Verifica se o e-mail já está em uso
            var existingUser = await _userManager.FindByEmailAsync(usuarioDto.Email);
            if (existingUser != null)
            {
                return BadRequest("O e-mail já está em uso.");
            }

            // Cria o novo usuário usando as convenções do Identity
            var usuario = new Usuario
            {
                UserName = usuarioDto.Email,
                Email = usuarioDto.Email,
                Nome = usuarioDto.Nome,
                DataCad = DateTime.UtcNow,
                EmailConfirmed = true, // Opcional: depende se você deseja que o e-mail seja confirmado por padrão
            };

            // Cria o usuário com a senha fornecida
            var result = await _userManager.CreateAsync(usuario, usuarioDto.Senha);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Adiciona o usuário ao respectivo role/perfil
            var roleResult = await _userManager.AddToRoleAsync(usuario, perfil.Name);
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            // Associa o usuário à clínica
            var usuarioPerfilClinica = new UsuarioPerfilClinica
            {
                UsuarioId = usuario.Id,
                PerfilId = perfilId,
                ClinicaId = clinicaId
            };

            _context.UsuariosPerfisClinicas.Add(usuarioPerfilClinica);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuário criado com sucesso.", UsuarioId = usuario.Id });
        }

        // Obtém usuários por clinicaId
        [HttpGet("{clinicaId}/usuarios")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> GetUsuariosByClinicaId(int clinicaId)
        {
            var usuarios = await _context.UsuariosPerfisClinicas
                .Where(upc => upc.ClinicaId == clinicaId)
                .Select(upc => upc.Usuario)
                .ToListAsync();

            if (usuarios == null || !usuarios.Any())
            {
                return NotFound("Nenhum usuário encontrado na clínica.");
            }

            return Ok(usuarios);
        }

        // Obtém todos os usuários
        [HttpGet]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        // Obtém um usuário por ID
        [HttpGet("{usuarioId}")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> GetUsuarioById(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            return Ok(usuario);
        }

        // Atualiza um usuário
        [HttpPut("{usuarioId}/editar")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> UpdateUsuario(int usuarioId, [FromBody] UsuarioDto usuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Atualiza os campos do usuário
            usuario.UserName = usuarioDto.Email;
            usuario.Email = usuarioDto.Email;
            usuario.Nome = usuarioDto.Nome;

            // Atualiza o usuário no banco de dados
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuário atualizado com sucesso." });
        }

        // Deleta um usuário
        [HttpDelete("{usuarioId}")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> DeleteUsuario(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Remove o usuário do banco de dados
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuário deletado com sucesso." });
        }
    }
}