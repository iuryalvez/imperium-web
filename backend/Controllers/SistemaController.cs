using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using backend.Data;
using backend.Models;
using backend.Dtos.Clinica;
using backend.Dtos.Animal;
using backend.Dtos.Usuario;
using backend.Services;
using System.Security.Claims;

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

        [HttpGet("clinicas/buscar")]
        [Authorize(Policy = "AcessoLoginPolicy")]
        public async Task<IActionResult> BuscarClinicasPorNome([FromQuery] string nome)
        {
            try {
                if (string.IsNullOrWhiteSpace(nome))
                    return BadRequest("O nome da clínica não pode ser vazio.");

                var clinicas = await _context.Clinicas
                    .Where(c => EF.Functions.ILike(c.NomeClinica, $"%{nome}%")) // ILike é case-insensitive (PostgreSQL)
                    .Select(c => new
                    {
                        value = c.Id,
                        label = c.NomeClinica
                    })
                    .ToListAsync();

                return Ok(clinicas);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Erro ao buscar clínica por nome.");
                return StatusCode(500, "Erro interno ao buscar clínica por nome.");
            }
        }

        // obtém todas as clínicas cadastradas
        [HttpGet("clinicas")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<IEnumerable<GetClinicaDto>>> GetClinicas()
        {
            try
            {
                var clinicas = await _context.Clinicas
                    .Select(c => new GetClinicaDto
                    {
                        Id = c.Id,
                        NomeClinica = c.NomeClinica,
                        NomeFantasia = c.NomeFantasia,
                        Cnpj = c.Cnpj,
                        Endereco = c.Endereco,
                        EndNumero = c.EndNumero,
                        EndEstado = c.EndEstado,
                        EndCidade = c.EndCidade,
                        Cep = c.Cep,
                        Telefone1 = c.Telefone1,
                        Telefone2 = c.Telefone2,
                        Whatsapp = c.Whatsapp,
                        Email = c.Email,
                        Instagram = c.Instagram,
                        DataCad = c.DataCad,
                        IsAtiva = c.IsAtiva
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
        public async Task<ActionResult<GetClinicaDto>> GetClinica(int clinicaId)
        {
            var clinica = await _context.Clinicas
                .Where(c => c.Id == clinicaId)
                .Select(c => new GetClinicaDto
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
        public async Task<ActionResult<CadastrarClinicaDto>> CadastrarClinica([FromBody] CadastrarClinicaDto cadastrarClinicaDto)
        {
            var novaClinica = new Clinica
            {
                NomeClinica = cadastrarClinicaDto.NomeClinica,
                IsAtiva = cadastrarClinicaDto.IsAtiva,
                DataCad = DateTime.UtcNow
            };

            _context.Clinicas.Add(novaClinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinica), new { clinicaId = novaClinica.Id }, cadastrarClinicaDto);
        }

        [HttpPut("clinicas/editar")]
        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        public async Task<IActionResult> EditarClinicaAdmin([FromBody] EditarClinicaAdminDto editarClinicaAdminDto)
        {
            try {
                // Verifica se o usuário tem permissão de administrador
                var isAdmin = User.Claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
                if (!isAdmin)
                {
                    return Forbid("Apenas administradores podem acessar esse recurso.");
                }

                if(!ModelState.IsValid) {
                    return BadRequest(ModelState);
                }

                var clinica = await _context.Clinicas.FindAsync(editarClinicaAdminDto.Id);
                if (clinica == null)
                {
                    return NotFound("Clínica não encontrada.");
                }

                clinica.NomeClinica = editarClinicaAdminDto.NomeClinica;
                clinica.NomeFantasia = editarClinicaAdminDto.NomeFantasia;
                clinica.Cnpj = editarClinicaAdminDto.Cnpj;
                clinica.Endereco = editarClinicaAdminDto.Endereco;
                clinica.EndNumero = editarClinicaAdminDto.EndNumero;
                clinica.EndEstado = editarClinicaAdminDto.EndEstado;
                clinica.EndCidade = editarClinicaAdminDto.EndCidade;
                clinica.Cep = editarClinicaAdminDto.Cep;
                clinica.Telefone1 = editarClinicaAdminDto.Telefone1;
                clinica.Telefone2 = editarClinicaAdminDto.Telefone2;
                clinica.Whatsapp = editarClinicaAdminDto.Whatsapp;
                clinica.Email = editarClinicaAdminDto.Email;
                clinica.Instagram = editarClinicaAdminDto.Instagram;
                clinica.IsAtiva = editarClinicaAdminDto.IsAtiva;

                // TODO: Validação adicional do CNPJ - 
                // if (!ValidarCnpj(clinica.Cnpj))
                // {
                //     return BadRequest("CNPJ inválido");
                // }

                await _context.SaveChangesAsync();
                return Ok( new {
                    Success = true,
                    Message = "Clínica atualizada com sucesso.",
                    Data = clinica
                });
            } 
            catch (DbUpdateConcurrencyException ex) 
            {
                _logger.LogError(ex, $"Conflito de concorrência ao editar clínica {editarClinicaAdminDto.NomeClinica}");
                return StatusCode(409, $"Os dados foram modificados por outro usuário. Recarregue e tente novamente.");
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Erro ao editar clínica {editarClinicaAdminDto.NomeClinica}");
                return StatusCode(500, "Erro interno ao editar clínica.");
            }
        }


        [HttpDelete("clinicas/{clinicaId}")]
        [Authorize(Policy = "GerenciaSistema")] // Descomente quando implementar a autenticação
        public async Task<IActionResult> DeleteClinica(int clinicaId)
        {
            // Verificação inicial do ID
            if (clinicaId <= 0)
            {
                return BadRequest(new { 
                    Success = false,
                    Message = "ID da clínica inválido.",
                    ErrorCode = "INVALID_ID"
                });
            }

            try
            {
                // Busca a clínica incluindo possíveis relacionamentos que precisam ser tratados
                var clinica = await _context.Clinicas
                    .Include(c => c.UsuariosPerfisClinicas) // Exemplo: incluir relacionamentos que podem impedir a deleção
                    .FirstOrDefaultAsync(c => c.Id == clinicaId);

                if (clinica == null)
                {
                    return NotFound(new {
                        Success = false,
                        Message = "Clínica não encontrada.",
                        ErrorCode = "CLINICA_NOT_FOUND"
                    });
                }

                // Verifica se há dependências que impedem a exclusão
                if (clinica.UsuariosPerfisClinicas?.Any() == true)
                {
                    return Conflict(new {
                        Success = false,
                        Message = "Não é possível excluir a clínica pois existem usuários vinculados.",
                        ErrorCode = "HAS_DEPENDENCIES",
                        Data = new { UsersCount = clinica.UsuariosPerfisClinicas.Count }
                    });
                }

                // Inicia transação para operação atômica
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    _context.Clinicas.Remove(clinica);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Clínica {clinicaId} deletada com sucesso.");
                    
                    return NoContent();
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    
                    _logger.LogError(ex, $"Erro ao deletar clínica {clinicaId}");
                    
                    return StatusCode(StatusCodes.Status500InternalServerError, new {
                        Success = false,
                        Message = "Erro ao deletar clínica no banco de dados.",
                        ErrorCode = "DATABASE_ERROR",
                        Detailed = ex.InnerException?.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro geral ao processar deleção da clínica {clinicaId}");
                
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    Success = false,
                    Message = "Erro interno no servidor.",
                    ErrorCode = "INTERNAL_ERROR",
                    Detailed = ex.Message
                });
            }
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

        [HttpGet("usuarios/buscar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> BuscarUsuariosPorNome([FromQuery] string nome)
        {
            try {
                if (string.IsNullOrWhiteSpace(nome))
                    return BadRequest("O nome do usuário não pode ser vazio.");

                var usuarios = await _context.Usuarios
                    .Where(u => EF.Functions.ILike(u.Nome, $"%{nome}%")) // ILike é case-insensitive (PostgreSQL)
                    .Select(u => new
                    {
                        value = u.Id,
                        label = u.Nome
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Erro ao buscar usuário por nome.");
                return StatusCode(500, "Erro interno ao buscar usuário por nome.");
            }
        }

        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<ActionResult<IEnumerable<GetUsuariosSistemaDto>>> ListarUsuarios()
        {        
            List<GetUsuariosSistemaDto> usuariosComuns = new();
            List<GetUsuariosSistemaDto> usuariosAdministradores = new();

            try
            {
                usuariosComuns = await _context.UsuariosPerfisClinicas
                    .AsNoTracking()
                    .Include(upc => upc.Usuario)
                    .Include(upc => upc.Clinica)
                    .Select(upc => new GetUsuariosSistemaDto
                    {
                        Id = upc.Usuario.Id,
                        UpcId = upc.Id,
                        UserName = upc.Usuario.UserName,
                        NomeClinica = upc.Clinica.NomeClinica,
                        ClinicaId = upc.ClinicaId,
                        Email = upc.Usuario.Email,
                        Nome = upc.Usuario.Nome ?? string.Empty,
                        Perfis = upc.Perfis,
                        DataCadastro = upc.Usuario.DataCad,
                        DataUltimoAcesso = upc.Usuario.DataUltimoAcesso,
                        IsAdmin = upc.Usuario.IsAdmin
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários comuns vinculados");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Erro interno ao buscar usuários comuns.",
                    ErrorCode = "INTERNAL_ERROR"
                });
            }

            // 2. Busca administradores sem vínculo (excluindo o usuário atual)
            try
            {
                // Obtém o ID do usuário atual
                var usuarioAtualId = User.GetClaimValue<int>(ClaimTypes.NameIdentifier);
                var idsUsuariosComuns = usuariosComuns.Select(u => u.Id).Distinct().ToList();
                
                usuariosAdministradores = await _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IsAdmin && 
                            !idsUsuariosComuns.Contains(u.Id) && 
                            u.Id != usuarioAtualId) // Exclui o usuário atual
                    .Select(u => new GetUsuariosSistemaDto
                    {
                        Id = u.Id,
                        UpcId = -1, // sem vínculo
                        UserName = u.UserName,
                        NomeClinica = "N/A",
                        ClinicaId = -1,
                        Email = u.Email,
                        Nome = u.Nome ?? string.Empty,
                        Perfis = new List<string> { "ADMINISTRADOR" },
                        DataCadastro = u.DataCad,
                        DataUltimoAcesso = u.DataUltimoAcesso,
                        IsAdmin = u.IsAdmin
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar administradores não vinculados");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Erro interno ao buscar usuários administradores.",
                    ErrorCode = "INTERNAL_ERROR"
                });
            }

            // 3. Combina os resultados
            var resultadoFinal = usuariosComuns.Concat(usuariosAdministradores);
            
            return Ok(new
            {
                Success = true,
                Message = "Busca de usuários realizada com sucesso.",
                Data = resultadoFinal,
                Count = resultadoFinal.Count()
            });
        }

        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> CadastrarUsuarioSistema([FromBody] CadastrarUsuarioSistemaDto cadastrarUsuarioSistemaDto)
        {
            Console.WriteLine("[DEBUG] Iniciando cadastro de usuário...");

            if (cadastrarUsuarioSistemaDto.Perfis != null)
            {
                Console.WriteLine("[DEBUG] Perfis recebidos no DTO:");
                foreach (var perfil in cadastrarUsuarioSistemaDto.Perfis)
                {
                    Console.WriteLine($"[DEBUG] - {perfil}");
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] Nenhum perfil recebido (null)");
            }

            if (cadastrarUsuarioSistemaDto.Perfis != null && 
                cadastrarUsuarioSistemaDto.Perfis.Contains("ADMIN", StringComparer.OrdinalIgnoreCase))
            {
                cadastrarUsuarioSistemaDto.IsAdmin = true;
                cadastrarUsuarioSistemaDto.Perfis = new List<string>();
                Console.WriteLine("[DEBUG] Usuário identificado como administrador (ADMIN)");
            }

            if (!cadastrarUsuarioSistemaDto.IsAdmin &&
                (cadastrarUsuarioSistemaDto.Perfis == null || !cadastrarUsuarioSistemaDto.Perfis.Any()))
            {
                Console.WriteLine("[DEBUG] Perfis obrigatórios ausentes para usuário não-admin.");
                return BadRequest("Pelo menos um perfil deve ser selecionado.");
            }

            var existingUser = await _userManager.FindByEmailAsync(cadastrarUsuarioSistemaDto.Email);
            if (existingUser != null)
            {
                Console.WriteLine("[DEBUG] E-mail já em uso.");
                return BadRequest("O e-mail já está em uso.");
            }

            var usuario = new Usuario
            {
                UserName = cadastrarUsuarioSistemaDto.UserName ?? cadastrarUsuarioSistemaDto.Email,
                Email = cadastrarUsuarioSistemaDto.Email,
                Nome = cadastrarUsuarioSistemaDto.Nome,
                DataCad = DateTime.UtcNow,
                DataUltimoAcesso = DateTime.UtcNow,
                EmailConfirmed = true,
                IsAdmin = cadastrarUsuarioSistemaDto.IsAdmin
            };

            var result = await _userManager.CreateAsync(usuario, cadastrarUsuarioSistemaDto.Senha);
            if (!result.Succeeded)
            {
                Console.WriteLine("[DEBUG] Erro ao criar usuário:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"[DEBUG] - {error.Description}");
                }
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            Console.WriteLine($"[DEBUG] Usuário criado com sucesso. ID: {usuario.Id}");

            if (!usuario.IsAdmin)
            {
                if (cadastrarUsuarioSistemaDto.ClinicaIds == null || !cadastrarUsuarioSistemaDto.ClinicaIds.Any())
                {
                    return BadRequest("Pelo menos uma clínica deve ser selecionada.");
                }

                foreach (var clinicaId in cadastrarUsuarioSistemaDto.ClinicaIds)
                {
                    var clinicaExiste = await _context.Clinicas.AnyAsync(c => c.Id == clinicaId);
                    Console.WriteLine($"[DEBUG] Clínica ID {clinicaId} existe? {clinicaExiste}");

                    if (!clinicaExiste)
                    {
                        return BadRequest($"A clínica com ID {clinicaId} não existe.");
                    }

                    var usuarioPerfilClinica = new UsuarioPerfilClinica
                    {
                        UsuarioId = usuario.Id,
                        ClinicaId = clinicaId,
                        Perfis = cadastrarUsuarioSistemaDto.Perfis
                    };

                    _context.UsuariosPerfisClinicas.Add(usuarioPerfilClinica);
                    Console.WriteLine($"[DEBUG] Vínculo UsuarioPerfilClinica com clínica {clinicaId} adicionado ao contexto.");
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] Usuário é admin, vínculo com clínica não será criado.");
            }

            // >>> ESSENCIAL: Salvar as alterações no banco de dados <<<
            await _context.SaveChangesAsync();
            // Console.WriteLine("[DEBUG] Alterações salvas no banco de dados.");

            return Ok(new { Message = "Usuário criado com sucesso.", UsuarioId = usuario.Id });
        }

        [HttpPut("usuarios/editar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> EditarUsuarioSistema([FromBody] EditarUsuarioSistemaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.UpcId == -1)
            {
                // ====== Edição de Usuário Administrador ======
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var usuario = await _context.Usuarios
                        .Where(u => u.IsAdmin == true)
                        .FirstOrDefaultAsync(u => u.Id == dto.Id);

                    if (usuario == null)
                    {
                        return NotFound(new { Success = false, Message = "Usuário administrador não encontrado." });
                    }

                    // Verifica email duplicado entre administradores
                    if (!string.IsNullOrWhiteSpace(dto.Email))
                    {
                        var emailExistente = await _context.Usuarios
                            .AnyAsync(u => u.IsAdmin == true && u.Email == dto.Email && u.Id != usuario.Id);

                        if (emailExistente)
                        {
                            return BadRequest(new { Success = false, Message = "O email informado já está em uso por outro administrador." });
                        }

                        usuario.UserName = dto.Email;
                        usuario.Email = dto.Email;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.Nome))
                    {
                        usuario.Nome = dto.Nome;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new {
                        Success = true,
                        Message = "Administrador atualizado com sucesso",
                        Data = new { UserId = usuario.Id }
                    });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Conflito de concorrência ao editar administrador {dto.Id}");
                    return StatusCode(409, new {
                        Success = false,
                        Message = "Os dados foram modificados por outro usuário. Recarregue e tente novamente.",
                        ErrorCode = "CONCURRENCY_ERROR"
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Erro ao editar administrador com ID {dto.Id}");
                    return StatusCode(500, new {
                        Success = false,
                        Message = "Erro interno ao editar administrador.",
                        ErrorCode = "INTERNAL_ERROR"
                    });
                }
            }
            else
            {
                // ====== Edição de Usuário Vinculado a Clínica ======
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
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

                    // Verifica email duplicado entre usuários não-administradores
                    if (!string.IsNullOrWhiteSpace(dto.Email))
                    {
                        var emailExistente = await _context.Usuarios
                            .AnyAsync(u => u.Email == dto.Email && u.Id != usuario.Id);

                        if (emailExistente)
                        {
                            return BadRequest(new { Success = false, Message = "O email informado já está em uso." });
                        }

                        usuario.UserName = dto.Email;
                        usuario.Email = dto.Email;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.Nome))
                    {
                        usuario.Nome = dto.Nome;
                    }

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
        }
    }
}