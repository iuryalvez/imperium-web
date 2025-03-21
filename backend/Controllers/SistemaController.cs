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
        [HttpPost("clinicas/nova-clinica")]
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
                    .AsNoTracking() // melhora o desempenho em consultas somente leitura
                    .Include(upc => upc.Usuario) // inclui os dados do usuário
                    .Where(u => u.UsuarioId.ToString() != userId)
                    .Select(upc => new GetUsuariosDto
                    {
                        Id = upc.Usuario.Id,
                        UpcId = upc.Id,
                        UserName = upc.Usuario.UserName,
                        NomeClinica = upc.Clinica.NomeClinica,
                        Email = upc.Usuario.Email,
                        Nome = upc.Usuario.Nome ?? string.Empty, // garante que Nome nunca seja nulo
                        Perfil = upc.Perfil.Name ?? string.Empty,
                        DataCadastro = upc.Usuario.DataCad,
                        DataUltimoAcesso = upc.Usuario.DataUltimoAcesso,
                        Habilitado = upc.Habilitado
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
                    .Include(upc => upc.Usuario) // Inclui os dados do usuário
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

                // Atualiza o estado de habilitação
                vinculo.Habilitado = dto.Habilitado;

                // Busca o perfil pelo nome e atualiza o vínculo
                var perfil = await _context.Perfis.FirstOrDefaultAsync(p => p.Name == dto.Perfil);
                if (perfil == null)
                {
                    return NotFound("Perfil não encontrado.");
                }

                vinculo.PerfilId = perfil.Id;
                _context.UsuariosPerfisClinicas.Update(vinculo);

                // Salva todas as alterações
                await _context.SaveChangesAsync();

                return Ok("Usuário e vínculo atualizados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao editar usuário com vínculo ID {upcId}");
                return StatusCode(500, "Erro interno ao editar usuário.");
            }
        }

        [HttpPut("usuarios/{usuarioId}/desabilitar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> DesabilitarTodosVinculos(int usuarioId)
        {
            try
            {
                // Verifica se há vínculos antes de buscá-los completamente
                bool existeVinculo = await _context.UsuariosPerfisClinicas
                    .AnyAsync(v => v.UsuarioId == usuarioId && v.Habilitado);

                if (!existeVinculo)
                {
                    return NotFound(new { Message = "Nenhum vínculo ativo encontrado para desabilitar." });
                }

                // Busca e desabilita todos os vínculos do usuário
                var vinculos = await _context.UsuariosPerfisClinicas
                    .Where(v => v.UsuarioId == usuarioId)
                    .ToListAsync();

                vinculos.ForEach(v => v.Habilitado = false);

                // Atualiza em lote para melhor performance
                _context.UsuariosPerfisClinicas.UpdateRange(vinculos);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todos os vínculos do usuário {UsuarioId} foram desabilitados com sucesso.", usuarioId);

                return Ok(new { Message = "Todos os vínculos foram desabilitados com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desabilitar vínculos do usuário {UsuarioId}.", usuarioId);
                return StatusCode(500, new { Message = "Erro interno ao desabilitar os vínculos." });
            }
        }

        [HttpPut("usuarios/{usuarioId}/habilitar-todos")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> HabilitarTodosVinculos(int usuarioId)
        {
            try
            {
                // Verifica se há vínculos desabilitados antes de buscá-los completamente
                bool existeVinculo = await _context.UsuariosPerfisClinicas
                    .AnyAsync(v => v.UsuarioId == usuarioId && !v.Habilitado);

                if (!existeVinculo)
                {
                    return NotFound(new { Message = "Nenhum vínculo desabilitado encontrado para habilitar." });
                }

                // Busca e habilita todos os vínculos desabilitados do usuário
                var vinculos = await _context.UsuariosPerfisClinicas
                    .Where(v => v.UsuarioId == usuarioId && !v.Habilitado)
                    .ToListAsync();

                vinculos.ForEach(v => v.Habilitado = true);

                // Atualiza em lote para melhor performance
                _context.UsuariosPerfisClinicas.UpdateRange(vinculos);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todos os vínculos do usuário {UsuarioId} foram habilitados com sucesso.", usuarioId);

                return Ok(new { Message = "Todos os vínculos foram habilitados com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao habilitar vínculos do usuário {UsuarioId}.", usuarioId);
                return StatusCode(500, new { Message = "Erro interno ao habilitar os vínculos." });
            }
        }

        [HttpGet("campos-cadastro/{entidade}")]
        public IActionResult ObterCamposCadastro(string entidade)
        {
            List<CamposCadastroDto> campos;

            switch (entidade.ToLower())
            {
                case "usuarios do sistema":
                    campos = new List<CamposCadastroDto>
                    {
                        new CamposCadastroDto { Name = "nome", Label = "Nome", Type = "text", Placeholder = "Digite seu nome", Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "Nome obrigatório" } } },
                        new CamposCadastroDto { Name = "email", Label = "E-mail", Type = "email", Placeholder = "Digite seu e-mail", Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "E-mail obrigatório" }, new RuleDto { Type = "email", Message = "Formato inválido" } } },
                        new CamposCadastroDto { Name = "senha", Label = "Senha", Type = "password", Placeholder = "Digite sua senha", Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "Senha obrigatória" }, new RuleDto { Type = "minLength", Message = "Mínimo 6 caracteres" } } },
                        new CamposCadastroDto { 
                            Name = "perfil", Label = "Perfil", Type = "select", Placeholder = "Selecione um perfil", 
                            Options = new List<OpcaoDto> { 
                                new OpcaoDto { Label = "Administrador", Value = "ADMIN" }, 
                                new OpcaoDto { Label = "Gestor", Value = "GESTOR" }, 
                                new OpcaoDto { Label = "Técnico", Value = "TECNICO" }, 
                                new OpcaoDto { Label = "Secretário", Value = "SECRETARIO" }, 
                                new OpcaoDto { Label = "Veterinário", Value = "VETERINARIO" } }, 
                                Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "Perfil obrigatório" } } 
                        }
                    };
                    break;
                
                case "usuarios":
                    campos = new List<CamposCadastroDto>
                    {
                        new CamposCadastroDto { Name = "nome", Label = "Nome", Type = "text", Placeholder = "Digite seu nome", Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "Nome obrigatório" } } },
                        new CamposCadastroDto { Name = "email", Label = "E-mail", Type = "email", Placeholder = "Digite seu e-mail", Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "E-mail obrigatório" }, new RuleDto { Type = "email", Message = "Formato inválido" } } },
                        new CamposCadastroDto { Name = "senha", Label = "Senha", Type = "password", Placeholder = "Digite sua senha", Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "Senha obrigatória" }, new RuleDto { Type = "minLength", Message = "Mínimo 6 caracteres" } } },
                        new CamposCadastroDto { 
                            Name = "perfil", Label = "Perfil", Type = "select", Placeholder = "Selecione um perfil", 
                            Options = new List<OpcaoDto> { 
                                new OpcaoDto { Label = "Gestor", Value = "GESTOR" }, 
                                new OpcaoDto { Label = "Técnico", Value = "TECNICO" }, 
                                new OpcaoDto { Label = "Secretário", Value = "SECRETARIO" }, 
                                new OpcaoDto { Label = "Veterinário", Value = "VETERINARIO" } }, 
                                Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "Perfil obrigatório" } } 
                        }
                    };
                    break;

                case "animais":
                    campos = new List<CamposCadastroDto>
                    {
                        new CamposCadastroDto { Name = "nome", Label = "Nome do Animal", Type = "text", Placeholder = "Digite o nome", Rules = new List<RuleDto> { new RuleDto { Type = "required", Message = "Nome obrigatório" } } },
                        new CamposCadastroDto { Name = "especie", Label = "Espécie", Type = "text", Placeholder = "Digite a espécie" },
                        new CamposCadastroDto { Name = "idade", Label = "Idade", Type = "number", Placeholder = "Digite a idade" },
                        new CamposCadastroDto { Name = "dono", Label = "Dono", Type = "select", Placeholder = "Selecione o dono", Options = new List<OpcaoDto> { new OpcaoDto { Label = "João", Value = "1" }, new OpcaoDto { Label = "Maria", Value = "2" } } }
                    };
                    break;

                default:
                    return BadRequest("Entidade não reconhecida.");
            }

            return Ok(campos);
        }

    }
}