using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using backend.Services;
using backend.Infra.Data;
using System.Security.Claims;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.GestaoDeAcesso.DTOs;

namespace backend.Domain.GestaoDeAcesso
{
    [ApiController]
    [Route("api/clinica")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(AppDbContext context,
                                 UserManager<Usuario> userManager,
                                 ILogger<UsuarioController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        
        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> GetUsuariosByClinicaId()
        {
            try
            {
                var clinicaId = User.GetClaimValue<int>("clinicaId");

                // Console.WriteLine("clinicaId encontrada na claim: " + clinicaId);

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
                        ClinicaId = upc.ClinicaId,
                        UserName = upc.Usuario.UserName,
                        Email = upc.Usuario.Email,
                        Nome = upc.Usuario.Nome ?? string.Empty, // Garante que Nome nunca seja nulo
                        Perfis = upc.Perfis,
                        DataCadastro = upc.Usuario.DataCad,
                        DataUltimoAcesso = upc.Usuario.DataUltimoAcesso,
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

        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> CadastrarUsuarioClinica([FromBody] CadastrarUsuarioClinicaDto cadastrarUsuarioClinicaDto)
        {
            // Validações iniciais (fora da transação pois são verificações rápidas)
            if (cadastrarUsuarioClinicaDto.Perfis == null || !cadastrarUsuarioClinicaDto.Perfis.Any())
            {
                return BadRequest("Pelo menos um perfil deve ser selecionado.");
            }

            // Inicia a transação atômica
            await using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var clinicaId = User.GetClaimValue<int>("clinicaId");
                Console.WriteLine("clinicaId que o usuário vai ser associado: " + clinicaId);
                // Verificação da clínica (dentro da transação)
                var clinicaExiste = await _context.Clinicas.AnyAsync(c => c.Id == clinicaId);
                if (!clinicaExiste)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("A clínica especificada não existe.");
                }

                // Verificação de e-mail único (dentro da transação)
                var existingUser = await _userManager.FindByEmailAsync(cadastrarUsuarioClinicaDto.Email);
                if (existingUser != null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("O e-mail já está em uso.");
                }

                // Criação do usuário (Identity)
                var usuario = new Usuario
                {
                    UserName = cadastrarUsuarioClinicaDto.UserName ?? cadastrarUsuarioClinicaDto.Email,
                    Email = cadastrarUsuarioClinicaDto.Email,
                    Nome = cadastrarUsuarioClinicaDto.Nome,
                    DataCad = DateTime.UtcNow,
                    DataUltimoAcesso = DateTime.UtcNow,
                    EmailConfirmed = true,
                    IsAdmin = false,
                    IsAtivo = true
                };

                var result = await _userManager.CreateAsync(usuario, cadastrarUsuarioClinicaDto.Senha);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
                }

                // Criação do vínculo (Entity Framework)
                var usuarioPerfilClinica = new UsuarioPerfilClinica
                {
                    UsuarioId = usuario.Id,
                    ClinicaId = clinicaId,
                    Perfis = cadastrarUsuarioClinicaDto.Perfis
                };

                _context.UsuariosPerfisClinicas.Add(usuarioPerfilClinica);
                await _context.SaveChangesAsync();

                // Confirma todas as operações
                await transaction.CommitAsync();

                return Ok(new { Message = "Usuário criado com sucesso.", UsuarioId = usuario.Id });
            }
            catch (Exception ex)
            {
                // Rollback automático em caso de erro não tratado
                await transaction.RollbackAsync();
                
                // Log do erro completo
                _logger.LogError(ex, "Erro durante o cadastro de usuário");
                
                return StatusCode(500, "Ocorreu um erro durante o cadastro. Por favor, tente novamente.");
            }
        }

        [HttpPut("usuarios/editar")]
        [Authorize(Policy = "GerenciaClinica")]
        public async Task<IActionResult> EditarUsuarioClinica([FromBody] EditarUsuarioClinicaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                Console.WriteLine("EditarUsuarioClinicaDto.UpcId: " + dto.UpcId);
                var vinculo = await _context.UsuariosPerfisClinicas
                    .Include(upc => upc.Usuario)
                    .FirstOrDefaultAsync(upc => upc.Id == dto.UpcId);

                if (vinculo == null)
                {
                    return NotFound(new { Success = false, Message = "Vínculo do usuário não encontrado." });
                }

                var usuario = vinculo.Usuario;
                if (usuario == null)
                {
                    return NotFound(new { Success = false, Message = "Usuário associado ao vínculo não encontrado." });
                }

                if (!string.IsNullOrWhiteSpace(dto.Nome))
                {
                    usuario.Nome = dto.Nome;
                }

                // Validação de perfis
                if (dto.Perfis == null || !dto.Perfis.Any())
                {
                    return BadRequest(new { Success = false, Message = "O usuário deve ter pelo menos um perfil ativo." });
                }
                vinculo.Perfis = dto.Perfis;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new {
                    Success = true,
                    Message = "Usuário atualizado com sucesso",
                    Data = new { UserId = usuario.Id, VinculoId = vinculo.Id }
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Conflito de concorrência ao editar usuário {dto.UpcId}");
                return StatusCode(409, new {
                    Success = false,
                    Message = "Os dados foram modificados por outro usuário. Recarregue e tente novamente.",
                    ErrorCode = "CONCURRENCY_ERROR"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Erro ao editar usuário com vínculo ID {dto.UpcId}");
                return StatusCode(500, new {
                    Success = false,
                    Message = "Erro interno ao editar usuário.",
                    ErrorCode = "INTERNAL_ERROR"
                });
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