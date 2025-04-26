using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using backend.Data;
using backend.Models;
using backend.Dtos.Clinica;
using backend.Dtos.Animal;
using backend.Dtos.Usuario;

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
        public async Task<ActionResult<IEnumerable<ClinicaDto>>> GetClinicas()
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
        public async Task<ActionResult<ClinicaDto>> CadastrarClinica([FromBody] ClinicaDto clinicaDto)
        {
            var clinica = new Clinica
            {
                NomeClinica = clinicaDto.NomeClinica
            };

            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClinica), new { clinicaId = clinica.Id }, clinicaDto);
        }

        [HttpPut("clinicas/editar")]
        [Authorize(Policy = "AcessoLoginOuTodosUsuarios")]
        public async Task<IActionResult> EditarClinicaAdmin([FromBody] EditarClinicaAdminDto editarClinicaAdminDto)
        {
            // Verifica se o usuário tem permissão de administrador
            var isAdmin = User.Claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
            if (!isAdmin)
            {
                return Forbid("Apenas administradores podem acessar esse recurso.");
            }

            if (editarClinicaAdminDto.ClinicaId == 0)
            {
                return BadRequest("Id da clínica não informado.");
            }

            var clinica = await _context.Clinicas.FindAsync(editarClinicaAdminDto.ClinicaId);
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
            clinica.Whatsapp = editarClinicaAdminDto.Whatsapp;
            clinica.Email = editarClinicaAdminDto.Email;

            await _context.SaveChangesAsync();

            return NoContent();
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

        // obtém todos os usuários
        [HttpGet("usuarios")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> ListarUsuarios()
        {
            try
            {
                var usuariosComuns = await _context.UsuariosPerfisClinicas
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
                        DataCadastro = upc.Usuario.DataCad
                    })
                    .ToListAsync();

                // Pega os IDs dos usuários já retornados
                var idsUsuariosComuns = usuariosComuns.Select(u => u.Id).Distinct();

                // Agora busca usuários administradores que não têm vínculo
                var usuariosAdministradores = await _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IsAdmin && !idsUsuariosComuns.Contains(u.Id))
                    .Select(u => new GetUsuariosSistemaDto
                    {
                        Id = u.Id,
                        UpcId = 0, // sem vínculo
                        UserName = u.UserName,
                        NomeClinica = "N/A",
                        ClinicaId = 0,
                        Email = u.Email,
                        Nome = u.Nome ?? string.Empty,
                        Perfis = new List<string> { "ADMINISTRADOR" },
                        DataCadastro = u.DataCad
                    })
                    .ToListAsync();

                // Junta tudo
                var todosUsuarios = usuariosComuns.Concat(usuariosAdministradores).ToList();

                return Ok(todosUsuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários");
                return StatusCode(500, "Erro interno ao buscar usuários.");
            }
        }

        [HttpGet("usuarios/inativos")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> ListarUsuariosInativos()
        {
            try
            {
                var usuariosInativos = await _context.UsuariosPerfisClinicas
                    .AsNoTracking()
                    .Include(upc => upc.Usuario)
                    .Include(upc => upc.Clinica)
                    .Where(upc => upc.Perfis.All(p => p == "Inativo"))
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
                        DataCadastro = upc.Usuario.DataCad
                    })
                    .ToListAsync();

                return Ok(usuariosInativos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários inativos");
                return StatusCode(500, "Erro interno ao buscar usuários inativos.");
            }
        }

        [HttpGet("usuarios/administradores")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> ListarUsuariosAdministradores()
        {
            try
            {
                var admins = await _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IsAdmin)
                    .Select(u => new GetUsuariosSistemaDto
                    {
                        Id = u.Id,
                        UpcId = 0,
                        UserName = u.UserName,
                        NomeClinica = "N/A",
                        ClinicaId = 0,
                        Email = u.Email,
                        Nome = u.Nome ?? string.Empty,
                        Perfis = new List<string> { "Administrador" },
                        DataCadastro = u.DataCad
                    })
                    .ToListAsync();

                return Ok(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar administradores");
                return StatusCode(500, "Erro interno ao buscar administradores.");
            }
        }

        [HttpPost("usuarios/cadastrar")]
        [Authorize(Policy = "GerenciaSistema")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioSistemaDto cadastrarUsuarioDto)
        {
            // Console.WriteLine("[DEBUG] Iniciando cadastro de usuário...");

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

            if (cadastrarUsuarioDto.Perfis != null && 
                cadastrarUsuarioDto.Perfis.Contains("ADMIN", StringComparer.OrdinalIgnoreCase))
            {
                cadastrarUsuarioDto.IsAdmin = true;
                cadastrarUsuarioDto.Perfis = new List<string>();
                Console.WriteLine("[DEBUG] Usuário identificado como administrador (ADMIN)");
            }

            if (!cadastrarUsuarioDto.IsAdmin &&
                (cadastrarUsuarioDto.Perfis == null || !cadastrarUsuarioDto.Perfis.Any()))
            {
                Console.WriteLine("[DEBUG] Perfis obrigatórios ausentes para usuário não-admin.");
                return BadRequest("Pelo menos um perfil deve ser selecionado.");
            }

            var existingUser = await _userManager.FindByEmailAsync(cadastrarUsuarioDto.Email);
            if (existingUser != null)
            {
                Console.WriteLine("[DEBUG] E-mail já em uso.");
                return BadRequest("O e-mail já está em uso.");
            }

            var usuario = new Usuario
            {
                UserName = cadastrarUsuarioDto.UserName ?? cadastrarUsuarioDto.Email,
                Email = cadastrarUsuarioDto.Email,
                Nome = cadastrarUsuarioDto.Nome,
                DataCad = DateTime.UtcNow,
                EmailConfirmed = true,
                IsAdmin = cadastrarUsuarioDto.IsAdmin
            };

            var result = await _userManager.CreateAsync(usuario, cadastrarUsuarioDto.Senha);
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
                if (cadastrarUsuarioDto.ClinicaIds == null || !cadastrarUsuarioDto.ClinicaIds.Any())
                {
                    return BadRequest("Pelo menos uma clínica deve ser selecionada.");
                }

                foreach (var clinicaId in cadastrarUsuarioDto.ClinicaIds)
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
                        Perfis = cadastrarUsuarioDto.Perfis
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
        public async Task<IActionResult> EditarUsuario([FromBody] EditarUsuarioSistemaDto dto)
        {
            try
            {
                // Busca o vínculo e inclui o usuário relacionado
                var vinculo = await _context.UsuariosPerfisClinicas
                    .Include(upc => upc.Usuario)
                    .FirstOrDefaultAsync(upc => upc.Id == dto.Id); // o Id do dto é o upcId

                if (vinculo == null)
                {
                    return NotFound("Vínculo do usuário não encontrado.");
                }

                // Valida se o vínculo corresponde ao usuário e à clínica recebidos
                // if (vinculo.UsuarioId != dto.Id || vinculo.ClinicaId != dto.ClinicaId)
                // {
                //     return BadRequest("Os dados do vínculo não correspondem ao usuário ou à clínica informados.");
                // }

                var usuario = vinculo.Usuario;
                if (usuario == null)
                {
                    return NotFound("Usuário associado ao vínculo não encontrado.");
                }

                // Atualiza os dados do usuário
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    usuario.UserName = dto.Email;
                    usuario.Email = dto.Email;
                }

                if (!string.IsNullOrWhiteSpace(dto.Nome))
                {
                    usuario.Nome = dto.Nome;
                }

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
                _logger.LogError(ex, $"Erro ao editar usuário com vínculo ID {dto.UpcId}");
                return StatusCode(500, "Erro interno ao editar usuário.");
            }
        }
    }
}