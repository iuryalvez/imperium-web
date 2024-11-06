using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Usuario;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(AppDbContext context, AuthService authService, UserManager<Usuario> userManager)
        {
            _context = context;
            _authService = authService;
            _userManager = userManager;
        }
        
        
        // cria um novo usuário com perfil e associação à clínica
        [HttpPost("{clinicaId}/usuarios/novo-usuario-{perfilId}")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CreateUsuario(int clinicaId, int perfilId, [FromBody] UsuarioDto usuarioDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetUsuariosByClinicaId(int clinicaId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }
            
            var usuarios = await _context.UsuariosPerfisClinicas
                .Where(upc => upc.ClinicaId == clinicaId)
                .Select(upc => new GetUsuariosDto
                {
                    Id = upc.Usuario.Id,
                    UserName = upc.Usuario.UserName,
                    Email = upc.Usuario.Email,
                    Nome = upc.Usuario.Nome
                })
                .ToListAsync();

            if (!usuarios.Any())
            {
                return NotFound("Nenhum usuário encontrado na clínica.");
            }

            return Ok(usuarios);
        }

        // Obtém um usuário por ID
        [HttpGet("{clinicaId}/usuarios/{usuarioId}")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetUsuarioById(int clinicaId, int usuarioId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            return Ok(usuario);
        }

        // Atualiza um usuário
        [HttpPut("{clinicaId}/usuarios/{usuarioId}/editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> UpdateUsuario(int clinicaId, int usuarioId, [FromBody] UsuarioDto usuarioDto)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }

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
        [HttpDelete("{clinicaId}/usuarios/{usuarioId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteUsuario(int clinicaId, int usuarioId)
        {
            if (!await _authService.VerificarPermissaoClinica(clinicaId))
            {
                return Forbid("Usuário não tem permissão para acessar essa clínica.");
            }
            
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