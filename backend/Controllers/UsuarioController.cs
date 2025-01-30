using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Usuario;
using backend.Interfaces;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(AppDbContext context, IAuthService authService, UserManager<Usuario> userManager)
        {
            _context = context;
            _authService = authService;
            _userManager = userManager;
        }
        
        // cria um novo usuário com perfil e associação à clínica
        [HttpPost("usuarios/cadastrar+{perfilId}")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CreateUsuario(int perfilId, [FromBody] UsuarioDto usuarioDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
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
                EmailConfirmed = true, // confirmado por padrão
            };

            // Cria o usuário com a senha fornecida (o Identity tratará do hashing automaticamente)
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


        // obtém usuários por clínica
        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetUsuariosByClinicaId()
        {
            try
            {
                // obtém o ID da clínica a partir do token do usuário
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                if (clinicaId <= 0)
                {
                    return BadRequest("Acesso não permitido ou clínica inválida.");
                }

                // realiza a consulta com as devidas inclusões e mapeia para o DTO
                var usuarios = await _context.UsuariosPerfisClinicas
                    .AsNoTracking() // melhora o desempenho em consultas somente leitura
                    .Include(upc => upc.Usuario) // inclui os dados do usuário
                    .Where(upc => upc.ClinicaId == clinicaId)
                    .Select(upc => new GetUsuariosDto
                    {
                        Id = upc.Usuario.Id,
                        UserName = upc.Usuario.UserName,
                        Email = upc.Usuario.Email,
                        Nome = upc.Usuario.Nome ?? string.Empty, // garante que Nome nunca seja nulo
                        Perfil = upc.Perfil.Name ?? string.Empty,
                        DataCadastro = upc.Usuario.DataCad,
                        DataUltimoAcesso = upc.Usuario.DataUltimoAcesso,
                        Habilitado = true
                    })
                    .ToListAsync();

                // retorna a lista de usuários (pode ser vazia)
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                // Retorna um erro genérico com a mensagem da exceção para diagnóstico
                return StatusCode(500, $"Erro ao buscar usuários da clínica: {ex.Message}");
            }
        }

        [HttpGet("usuarios/{usuarioId}")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetUsuarioById(int usuarioId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            return Ok(usuario);
        }

        [HttpPut("usuarios/{usuarioId}/editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> UpdateUsuario(int usuarioId, [FromBody] UsuarioDto usuarioDto)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
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

        [HttpDelete("usuarios/{usuarioId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteUsuario(int usuarioId)
        {
            var clinicaId = User.GetClaimValue<int>("clinicaId");
            if (clinicaId == 0) {
                return BadRequest("Acesso não permitido a esta clínica.");
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

        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }
}