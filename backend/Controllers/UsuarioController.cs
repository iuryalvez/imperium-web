using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Models;
using backend.Data;
using backend.Dtos.Usuario;
using backend.Interfaces;
using backend.Dtos.Vinculo;
using System.Security.Claims;
using backend.Dtos;
using System.Text.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(AppDbContext context, IAuthService authService, UserManager<Usuario> userManager, ILogger<UsuarioController> logger)
        {
            _context = context;
            _authService = authService;
            _userManager = userManager;
            _logger = logger;
        }
        
        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioClinicaDto cadastrarUsuarioDto)
        {
            if (cadastrarUsuarioDto.Perfis == null || !cadastrarUsuarioDto.Perfis.Any())
            {
                return BadRequest("Pelo menos um perfil deve ser selecionado.");
            }

            var clinicaExiste = await _context.Clinicas.AnyAsync(c => c.Id == cadastrarUsuarioDto.ClinicaId);
            if (!clinicaExiste)
            {
                return BadRequest("A clínica especificada não existe.");
            }

            var existingUser = await _userManager.FindByEmailAsync(cadastrarUsuarioDto.Email);
            if (existingUser != null)
            {
                return BadRequest("O e-mail já está em uso.");
            }

            var usuario = new Usuario
            {
                UserName = cadastrarUsuarioDto.UserName ?? cadastrarUsuarioDto.Email, // Usa o email se UserName for nulo
                Email = cadastrarUsuarioDto.Email,
                Nome = cadastrarUsuarioDto.Nome,
                DataCad = DateTime.UtcNow,
                DataUltimoAcesso = DateTime.UtcNow,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(usuario, cadastrarUsuarioDto.Senha);
            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            var usuarioPerfilClinica = new UsuarioPerfilClinica
            {
                UsuarioId = usuario.Id,
                ClinicaId = cadastrarUsuarioDto.ClinicaId,
                Perfis = cadastrarUsuarioDto.Perfis
            };

            _context.UsuariosPerfisClinicas.Add(usuarioPerfilClinica);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuário criado com sucesso.", UsuarioId = usuario.Id });
        }


        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetUsuariosByClinicaId()
        {
            try
            {
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Realiza a consulta com as devidas inclusões e mapeia para o DTO
                var usuarios = await _context.UsuariosPerfisClinicas
                    .AsNoTracking() // Melhora o desempenho em consultas somente leitura
                    .Include(upc => upc.Usuario) // Inclui os dados do usuário
                    .Where(upc => upc.ClinicaId == clinicaId)
                    .Where(upc => upc.UsuarioId.ToString() != userId)
                    .Select(upc => new GetUsuariosClinicaDto
                    {
                        Id = upc.Usuario.Id,
                        UpcId = upc.Id,
                        UserName = upc.Usuario.UserName,
                        Email = upc.Usuario.Email,
                        Nome = upc.Usuario.Nome ?? string.Empty, // Garante que Nome nunca seja nulo
                        Perfis = upc.Perfis,
                        DataCadastro = upc.Usuario.DataCad,
                        DataUltimoAcesso = upc.Usuario.DataUltimoAcesso
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários da clínica.");
                return StatusCode(500, "Erro interno ao buscar usuários da clínica.");
            }
        }

        [HttpDelete("usuarios/{usuarioId}")]
        [Authorize(Policy = "GerenciaSistema")]
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

        [HttpGet("debug-claims")]
        [Authorize(Policy = "GerenciaSistema")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        [HttpGet("vinculos")]
        public async Task<IActionResult> GetAllVinculos()
        {
            var vinculos = await _context.UsuariosPerfisClinicas
                .AsNoTracking()
                .ToListAsync();

            if (!vinculos.Any())
            {
                return NotFound(new { Message = "Nenhum vínculo encontrado." });
            }

            return Ok(vinculos);
        }
    }
}