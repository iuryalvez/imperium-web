using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using backend.Data;
using backend.Models;
using backend.Dtos.Clinica;
using backend.Dtos.Animal;
using backend.Dtos.Usuario;
using System.Security.Claims;
using backend.Dtos;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/sistema")]
    public class SistemaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<SistemaController> _logger;

        public SistemaController(AppDbContext context, UserManager<Usuario> userManager, ILogger<SistemaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // obtém todas as clínicas cadastradas
        [HttpGet("clinicas")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<IEnumerable<ClinicaDto>>> GetAllClinicas()
        {
            try
            {
                var clinicas = await _context.Clinicas
                    .Select(c => new ClinicaDto
                    {
                        Id = c.Id,
                        NomeClinica = c.NomeClinica
                    })
                    .ToListAsync();

                return Ok(clinicas); // retorna lista vazia (200) se não houver clínicas
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clínicas");
                return StatusCode(500, "Erro interno ao buscar clínicas.");
            }
        }

        // get clinica by clinicaId
        [HttpGet("clinicas/{clinicaId}")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<ClinicaDto>> GetClinica(int clinicaId)
        {
            var clinica = await _context.Clinicas
                .Where(c => c.Id == clinicaId)
                .Select(c => new ClinicaDto
                {
                    Id = c.Id,
                    NomeClinica = c.NomeClinica
                })
                .FirstOrDefaultAsync();

            if (clinica == null)
            {
                return NotFound();
            }

            return Ok(clinica);
        }

        // criar clinica pelo sistema
        [HttpPost("clinicas/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<ClinicaDto>> CreateClinica([FromBody] ClinicaDto clinicaDto)
        {
            var clinica = new Clinica
            {
                NomeClinica = clinicaDto.NomeClinica
            };

            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinica), new { clinicaId = clinica.Id }, clinicaDto);
        }

        // deleta clínica
        [HttpDelete("clinicas/{clinicaId}")]
        // [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DeleteClinica(int clinicaId)
        {
            var clinica = await _context.Clinicas.FindAsync(clinicaId);
            if (clinica == null)
            {
                return NotFound();
            }

            _context.Clinicas.Remove(clinica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // obtém todos os animais cadastrados
        [HttpGet("animais")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<IEnumerable<GetAnimaisDto>>> GetAllAnimais()
        {
            try
            {
                var animais = await _context.Animais
                .Select(a => new GetAnimaisDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Especie = a.Especie,
                    Sexo = a.Sexo
                })
                .ToListAsync();

                return Ok(animais);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar animais");
                return StatusCode(500, "Erro interno ao buscar animais.");
            }
        }

        // obtém todos os usuários
        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> GetAllUsuarios()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var usuarios = await _context.UsuariosPerfisClinicas
                    .AsNoTracking()
                    .Include(upc => upc.Usuario)
                    .Include(upc => upc.Clinica)
                    .Select(upc => new GetUsuariosDto
                    {
                        Id = upc.Usuario.Id,
                        UserName = upc.Usuario.UserName,
                        NomeClinica = upc.Clinica.NomeClinica,
                        ClinicaId = upc.ClinicaId,
                        Email = upc.Usuario.Email,
                        Nome = upc.Usuario.Nome ?? string.Empty,
                        Perfis = upc.Perfis,
                        DataCadastro = upc.Usuario.DataCad,
                        DataUltimoAcesso = upc.Usuario.DataUltimoAcesso
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários");
                return StatusCode(500, "Erro interno ao buscar usuários.");
            }
        }

        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioDto cadastrarUsuarioDto)
        {
            // Imprime lista de perfis recebida
            if (cadastrarUsuarioDto.Perfis != null)
            {
                Console.WriteLine("[DEBUG] Perfis recebidos no DTO:");
                foreach (var perfil in cadastrarUsuarioDto.Perfis)
                {
                    Console.WriteLine($"[DEBUG] - {perfil}");
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] Nenhum perfil recebido (null)");
            }

            // Se vier com "ADMIN" na lista de perfis, trata como isAdmin
            if (cadastrarUsuarioDto.Perfis != null && 
                cadastrarUsuarioDto.Perfis.Contains("ADMIN", StringComparer.OrdinalIgnoreCase))
            {
                cadastrarUsuarioDto.IsAdmin = true;
                cadastrarUsuarioDto.Perfis = new List<string>(); // ou null, se preferir
            }

            // Para usuários não-admins, precisa validar os perfis
            if (!cadastrarUsuarioDto.IsAdmin &&
                (cadastrarUsuarioDto.Perfis == null || !cadastrarUsuarioDto.Perfis.Any()))
            {
                return BadRequest("Pelo menos um perfil deve ser selecionado.");
            }

            // Para usuários não-admins, clinica é obrigatória
            if (!cadastrarUsuarioDto.IsAdmin)
            {
                var clinicaExiste = await _context.Clinicas.AnyAsync(c => c.Id == cadastrarUsuarioDto.ClinicaId);
                if (!clinicaExiste)
                {
                    return BadRequest("A clínica especificada não existe.");
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(cadastrarUsuarioDto.Email);
            if (existingUser != null)
            {
                return BadRequest("O e-mail já está em uso.");
            }

            var usuario = new Usuario
            {
                UserName = cadastrarUsuarioDto.UserName ?? cadastrarUsuarioDto.Email,
                Email = cadastrarUsuarioDto.Email,
                Nome = cadastrarUsuarioDto.Nome,
                DataCad = DateTime.UtcNow,
                DataUltimoAcesso = DateTime.UtcNow,
                EmailConfirmed = true,
                IsAdmin = cadastrarUsuarioDto.IsAdmin
            };

            var result = await _userManager.CreateAsync(usuario, cadastrarUsuarioDto.Senha);
            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            // Apenas cria vínculo com clínica se não for admin
            if (!usuario.IsAdmin)
            {
                var usuarioPerfilClinica = new UsuarioPerfilClinica
                {
                    UsuarioId = usuario.Id,
                    ClinicaId = cadastrarUsuarioDto.ClinicaId,
                    Perfis = cadastrarUsuarioDto.Perfis
                };

                _context.UsuariosPerfisClinicas.Add(usuarioPerfilClinica);
            }

            return Ok(new { Message = "Usuário criado com sucesso.", UsuarioId = usuario.Id });
        }

        [HttpPut("usuarios/{upcId}/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> EditarUsuario(int upcId, [FromBody] EditarUsuarioDto dto)
        {
            try
            {
                if (dto == null || upcId != dto.UpcId)
                {
                    return BadRequest("Dados inválidos para edição.");
                }

                // Busca o vínculo do usuário na tabela UsuariosPerfisClinicas
                var vinculo = await _context.UsuariosPerfisClinicas
                    .Include(upc => upc.Usuario)
                    .FirstOrDefaultAsync(upc => upc.Id == upcId);

                if (vinculo == null)
                {
                    return NotFound("Vínculo do usuário não encontrado.");
                }

                var usuario = vinculo.Usuario;
                if (usuario == null)
                {
                    return NotFound("Usuário associado ao vínculo não encontrado.");
                }

                // Atualiza os dados do usuário
                usuario.UserName = dto.UserName;
                usuario.Email = dto.Email;
                usuario.Nome = dto.Nome;
                _context.Usuarios.Update(usuario);

                // Atualiza os perfis no vínculo
                if (dto.Perfis != null && dto.Perfis.Any())
                {
                    vinculo.Perfis = dto.Perfis;
                }
                else
                {
                    return BadRequest("O usuário deve ter pelo menos um perfil ativo.");
                }

                _context.UsuariosPerfisClinicas.Update(vinculo);
                await _context.SaveChangesAsync();

                return Ok("Usuário e vínculo atualizados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao editar usuário com vínculo ID {upcId}");
                return StatusCode(500, "Erro interno ao editar usuário.");
            }
        }
    }
}