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
        
        // cria um novo usuário com perfil e associação à clínica
        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioDto cadastrarUsuarioDto)
        {
            // Verifica se o perfil é válido
            var perfil = await _context.Perfis.FindAsync(cadastrarUsuarioDto.PerfilId);
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
            var existingUser = await _userManager.FindByEmailAsync(cadastrarUsuarioDto.Email);
            if (existingUser != null)
            {
                return BadRequest("O e-mail já está em uso.");
            }

            // Cria o novo usuário usando as convenções do Identity
            var usuario = new Usuario
            {
                UserName = cadastrarUsuarioDto.Email,
                Email = cadastrarUsuarioDto.Email,
                Nome = cadastrarUsuarioDto.Nome,
                DataCad = DateTime.UtcNow,
                DataUltimoAcesso = DateTime.UtcNow,
                EmailConfirmed = true, // confirmado por padrão
            };

            // Cria o usuário com a senha fornecida (o Identity tratará do hashing automaticamente)
            var result = await _userManager.CreateAsync(usuario, cadastrarUsuarioDto.Senha);
            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }
            
            // Adiciona o usuário ao respectivo role/perfil
            var roleResult = await _userManager.AddToRoleAsync(usuario, perfil.Name);
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            var clinicaIdClaim = User.Claims.FirstOrDefault(c => c.Type == "clinicaId")?.Value;
            if (string.IsNullOrEmpty(clinicaIdClaim) || !int.TryParse(clinicaIdClaim, out var clinicaId))
            {
                return BadRequest("Não foi possível identificar a clínica do usuário.");
            }

            // Associa o usuário à clínica
            var usuarioPerfilClinica = new UsuarioPerfilClinica
            {
                UsuarioId = usuario.Id,
                PerfilId = perfil.Id,
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
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // realiza a consulta com as devidas inclusões e mapeia para o DTO
                var usuarios = await _context.UsuariosPerfisClinicas
                    .AsNoTracking() // melhora o desempenho em consultas somente leitura
                    .Include(upc => upc.Usuario) // inclui os dados do usuário
                    .Where(upc => upc.ClinicaId == clinicaId)
                    .Where(upc => upc.Perfil.Name != "ADMIN")
                    .Where(u => u.UsuarioId.ToString() != userId)
                    .Select(upc => new GetUsuariosByClinicaDto
                    {
                        Id = upc.Usuario.Id,
                        UpcId = upc.Id,
                        UserName = upc.Usuario.UserName,
                        Email = upc.Usuario.Email,
                        Nome = upc.Usuario.Nome ?? string.Empty, // garante que Nome nunca seja nulo
                        Perfil = upc.Perfil.Name ?? string.Empty,
                        DataCadastro = upc.Usuario.DataCad,
                        DataUltimoAcesso = upc.Usuario.DataUltimoAcesso,
                        Habilitado = upc.Habilitado
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

        [HttpPut("usuarios/{upcId}/perfil")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> AtualizarPerfil(int upcId, [FromBody] AtualizarPerfilDto atualizarPerfilDto)
        {
            // Buscar o vínculo existente
            var vinculo = await _context.UsuariosPerfisClinicas.FirstOrDefaultAsync(v => v.Id == upcId);
            if (vinculo == null)
            {
                return NotFound(new { Message = "Vínculo do usuário não encontrado." });
            }

            // Buscar o perfil com base no nome informado
            var perfil = await _context.Perfis.FirstOrDefaultAsync(p => p.Name == atualizarPerfilDto.NovoPerfil);
            if (perfil == null)
            {
                return BadRequest(new { Message = "Perfil inválido. Selecione um perfil existente." });
            }

            // Remover o vínculo atual
            _context.UsuariosPerfisClinicas.Remove(vinculo);
            await _context.SaveChangesAsync();

            // Criar um novo vínculo com o novo PerfilId e manter os outros dados
            var novoVinculo = new UsuarioPerfilClinica
            {
                UsuarioId = vinculo.UsuarioId,
                PerfilId = perfil.Id,
                ClinicaId = vinculo.ClinicaId,
                Habilitado = vinculo.Habilitado,
                // Se houver outras propriedades, copie-as conforme necessário
            };

            _context.UsuariosPerfisClinicas.Add(novoVinculo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Perfil do vínculo {VinculoId} atualizado para {NovoPerfil}.", upcId, atualizarPerfilDto.NovoPerfil);

            return Ok(new 
            { 
                Message = "Perfil atualizado com sucesso.", 
                VinculoId = novoVinculo.Id,
                NovoPerfil = atualizarPerfilDto.NovoPerfil
            });
        }

        [HttpPut("usuarios/{upcId}/habilitar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> HabilitarVinculo(int upcId)
        {
            var vinculo = await _context.UsuariosPerfisClinicas
                .FirstOrDefaultAsync(v => v.Id == upcId);
            
            if (vinculo == null)
            {
                return NotFound(new { Message = "Vínculo não encontrado." });
            }

            vinculo.Habilitado = true;

            // persistir alteração no banco
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vínculo {VinculoId} habilitado com sucesso.", upcId);

            return Ok(new 
            { 
                Message = "Vínculo habilitado com sucesso.", 
                VinculoId = upcId 
            });
        }

        [HttpPut("usuarios/{upcId}/desabilitar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> DesabilitarVinculo(int upcId)
        {
            var vinculo = await _context.UsuariosPerfisClinicas
                .FirstOrDefaultAsync(v => v.Id == upcId);
            
            if (vinculo == null)
            {
                return NotFound(new { Message = "Vínculo não encontrado." });
            }

            vinculo.Habilitado = false;

            // Persistir alteração no banco
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vínculo {VinculoId} desabilitado com sucesso.", upcId);

            return Ok(new 
            { 
                Message = "Vínculo desabilitado com sucesso.", 
                VinculoId = upcId 
            });
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