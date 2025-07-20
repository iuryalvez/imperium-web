// Application/Services/UsuarioService.cs
using backend.Domain.GestaoDeAcesso.DTOs;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _userRepository;
    private readonly AppDbContext _context; // Para transações

    public UsuarioService(IUsuarioRepository userRepository, AppDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<ServiceResult> ListarUsuariosSistemaAsync(int currentUserId)
    {
        try
        {
            // 1. Busca usuários comuns vinculados
            var vinculos = await _userRepository.GetAllVinculosSistemaAsync();
            var usuariosComuns = vinculos.Select(upc => new GetUsuariosSistemaDto
            {
                // O CompositeKey para usuários vinculados usa o ID do VÍNCULO (upc.Id)
                CompositeKey = $"link-{upc.Id}",
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
                IsAdmin = upc.Usuario.IsAdmin,
                IsAtivo = upc.Usuario.IsAtivo,
                Cpf = upc.Usuario.Cpf,
                Formacao = upc.Usuario.Formacao,
                Telefone1 = upc.Usuario.Telefone1,
                Whatsapp = upc.Usuario.Whatsapp
            }).ToList();

            // 2. Busca administradores sem vínculo
            var idsUsuariosComuns = usuariosComuns.Select(u => u.Id).Distinct().ToList();
            var adminsNaoVinculados = await _userRepository.GetUnlinkedAdminsAsync(idsUsuariosComuns, currentUserId);
            var usuariosAdministradores = adminsNaoVinculados.Select(u => new GetUsuariosSistemaDto
            {
                // O CompositeKey para admins usa o ID do USUÁRIO (u.Id)
                CompositeKey = $"admin-{u.Id}",
                Id = u.Id,
                UpcId = -1, // sem vínculo
                UserName = u.UserName,
                NomeClinica = "N/A",
                ClinicaId = -1,
                Email = u.Email,
                Nome = u.Nome ?? string.Empty,
                Perfis = u.IsAtivo ? new List<string> { "ADMIN" } : new List<string> { "ADMIN", "INATIVO" },
                DataCadastro = u.DataCad,
                DataUltimoAcesso = u.DataUltimoAcesso,
                IsAdmin = u.IsAdmin,
                IsAtivo = u.IsAtivo,
                Cpf = u.Cpf,
                Formacao = u.Formacao,
                Telefone1 = u.Telefone1,
                Whatsapp = u.Whatsapp
            }).ToList();

            // 3. Combina os resultados
            var resultadoFinal = usuariosComuns.Concat(usuariosAdministradores);

            return new ServiceResult
            {
                Succeeded = true,
                Message = "Busca de usuários realizada com sucesso.",
                Data = resultadoFinal
            };
        }
        catch (Exception ex)
        {
            // Logar o erro 'ex' aqui
            return new ServiceResult { Succeeded = false, Message = "Erro interno ao buscar usuários." };
        }
    }

    public async Task<IEnumerable<GetUsuariosClinicaDto>> GetUsuariosByClinicIdAsync(int clinicaId, string currentUserId)
    {
        var vinculos = await _userRepository.GetVinculosByClinicIdAsync(clinicaId);

        return vinculos
            .Where(upc => upc.UsuarioId.ToString() != currentUserId) // Lógica de negócio aqui
            .Select(upc => new GetUsuariosClinicaDto
            {
                Id = upc.Usuario.Id,
                UpcId = upc.Id,
                ClinicaId = upc.ClinicaId,
                UserName = upc.Usuario.UserName,
                Email = upc.Usuario.Email,
                Nome = upc.Usuario.Nome ?? string.Empty,
                Perfis = upc.Perfis,
                IsAtivoNaClinica = !(upc.Perfis?.Contains("INATIVO") ?? false),
                DataCadastro = upc.Usuario.DataCad,
                DataUltimoAcesso = upc.Usuario.DataUltimoAcesso,
                Cpf = upc.Usuario.Cpf,
                Formacao = upc.Usuario.Formacao,
                Telefone1 = upc.Usuario.Telefone1,
                Whatsapp = upc.Usuario.Whatsapp
            });
    }

    public async Task<ServiceResult> CadastrarUsuarioSistemaAsync(CadastrarUsuarioSistemaDto dto)
    {
        // 1. Processar e validar DTO
        dto.IsAdmin = dto.Perfis?.Contains("ADMIN", StringComparer.OrdinalIgnoreCase) ?? false;
        dto.IsAtivo = !(dto.Perfis?.Contains("INATIVO", StringComparer.OrdinalIgnoreCase) ?? false);

        if (!dto.IsAdmin && (dto.Perfis == null || !dto.Perfis.Any()))
        {
            return new ServiceResult { Succeeded = false, Message = "Pelo menos um perfil deve ser selecionado." };
        }

        if (dto.IsAdmin)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                return new ServiceResult { Succeeded = false, Message = "O e-mail já está em uso." };
            }
        }

        // Inicia a transação
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 2. Criar o usuário
            var usuario = new Usuario
            {
                UserName = dto.UserName ?? dto.Email,
                Email = dto.Email,
                Nome = dto.Nome,
                DataCad = DateTime.UtcNow,
                DataUltimoAcesso = DateTime.UtcNow,
                EmailConfirmed = true,
                IsAdmin = dto.IsAdmin,
                IsAtivo = dto.IsAtivo
            };

            var (identityResult, novoUsuario) = await _userRepository.CreateUserAsync(usuario, dto.Senha);
            if (!identityResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Succeeded = false, Errors = identityResult.Errors.Select(e => e.Description).ToList() };
            }

            // 3. Criar vínculos (se não for admin)
            if (!novoUsuario.IsAdmin)
            {
                if (dto.ClinicaIds == null || !dto.ClinicaIds.Any())
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Message = "Pelo menos uma clínica deve ser selecionada." };
                }

                foreach (var clinicaId in dto.ClinicaIds)
                {
                    if (!await _userRepository.ClinicaExistsAsync(clinicaId))
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Message = $"A clínica com ID {clinicaId} não existe." };
                    }

                    if (await _userRepository.EmailExistsInClinicAsync(dto.Email, clinicaId))
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Message = "O email informado já está em uso nesta clínica." };
                    }

                    await _userRepository.AddUserClinicLinkAsync(new UsuarioPerfilClinica
                    {
                        UsuarioId = novoUsuario.Id,
                        ClinicaId = clinicaId,
                        Perfis = dto.Perfis
                    });
                }
            }

            await _userRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResult { Succeeded = true, Message = "Usuário criado com sucesso.", Data = new { UsuarioId = novoUsuario.Id } };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Logar o erro 'ex' aqui
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro inesperado no servidor." };
        }
    }

    public async Task<ServiceResult> SetUserActivationStatusAsync(SetUserStatusDto dto)
    {
        if (dto.IsAdmin)
        {
            // --- LÓGICA PARA ADMINISTRADORES ---
            var usuario = await _userRepository.GetUserByIdAsync(dto.Id);
            if (usuario == null || !usuario.IsAdmin)
                return new ServiceResult { Succeeded = false, Message = "Usuário administrador não encontrado." };

            // Define o status diretamente na entidade principal do usuário
            usuario.IsAtivo = dto.Activate;

            await _userRepository.SaveChangesAsync();
            string acao = dto.Activate ? "ativado" : "inativado";
            return new ServiceResult { Succeeded = true, Message = $"Administrador {acao} com sucesso." };
        }
        else
        {
            // --- LÓGICA PARA USUÁRIOS DE CLÍNICA (BASEADA NO VÍNCULO) ---
            var vinculo = await _userRepository.GetUserClinicLinkByIdAsync(dto.Id); // Aqui, dto.Id é o upcId
            if (vinculo?.Usuario == null)
                return new ServiceResult { Succeeded = false, Message = "Vínculo do usuário não encontrado." };

            if (dto.Activate)
            {
                // ATIVAR O VÍNCULO
                vinculo.Perfis?.Remove("INATIVO");
                // Se pelo menos um vínculo está ativo, o usuário principal também deve estar ativo
                vinculo.Usuario.IsAtivo = true;
            }
            else
            {
                // INATIVAR O VÍNCULO
                if (vinculo.Perfis == null) vinculo.Perfis = new List<string>();
                if (!vinculo.Perfis.Contains("INATIVO"))
                {
                    vinculo.Perfis.Add("INATIVO");
                }

                // REGRA DE NEGÓCIO: Verifica se TODOS os vínculos do usuário estão inativos
                var todosOsVinculos = await _userRepository.GetAllVinculosByUserIdAsync(vinculo.UsuarioId);
                bool todosInativos = todosOsVinculos.All(v => v.Perfis?.Contains("INATIVO") ?? false);

                if (todosInativos)
                {
                    vinculo.Usuario.IsAtivo = false; // Inativa o usuário principal
                }
            }

            await _userRepository.SaveChangesAsync();
            string acaoVinculo = dto.Activate ? "ativado" : "inativado";
            return new ServiceResult { Succeeded = true, Message = $"Vínculo do usuário {acaoVinculo} com sucesso." };
        }
    }

    public async Task<ServiceResult> UpdateUsuarioSistemaAsync(EditarUsuarioSistemaDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var result = dto.IsAdmin
                ? await UpdateAdminUserAsync(dto)
                : await UpdateClinicUserAsync(dto);

            if (result.Succeeded)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Logar o erro 'ex'
            return new ServiceResult { Succeeded = false, Message = "Erro inesperado ao atualizar usuário." };
        }
    }

    public async Task<ServiceResult> UnlinkUserFromClinicAsync(int upcId)
    {
        var vinculo = await _userRepository.GetVinculoByIdAsync(upcId);
        if (vinculo == null)
        {
            return new ServiceResult { Succeeded = false, Message = "Vínculo não encontrado." };
        }

        // REGRA DE NEGÓCIO: Verifica se este é o último vínculo do usuário
        var totalVinculos = await _context.UsuariosPerfisClinicas
                                    .CountAsync(v => v.UsuarioId == vinculo.UsuarioId);

        if (totalVinculos <= 1)
        {
            return new ServiceResult { Succeeded = false, Message = "Não é possível remover o último vínculo do usuário com uma clínica. Para remover o acesso completamente, desative o usuário." };
        }

        await _userRepository.RemoveVinculoAsync(vinculo);
        await _userRepository.SaveChangesAsync();

        return new ServiceResult { Succeeded = true, Message = "Usuário desvinculado da clínica com sucesso." };
    }

    // Métodos privados para organizar a lógica de cada tipo de usuário
    private async Task<ServiceResult> UpdateAdminUserAsync(EditarUsuarioSistemaDto dto)
    {
        var usuario = await _userRepository.GetAdminUserByIdAsync(dto.Id);
        if (usuario == null) return new ServiceResult { Succeeded = false, Message = "Usuário administrador não encontrado." };

        // Validação de e-mail duplicado
        if (await _userRepository.EmailExistsForAnotherUserAsync(dto.Email, dto.Id))
        {
            return new ServiceResult { Succeeded = false, Message = "O e-mail informado já está em uso." };
        }

        // Atualiza os campos
        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.UserName = dto.Email;
        usuario.IsAtivo = dto.IsAtivo;
        // Atualiza campos opcionais
        if (!string.IsNullOrWhiteSpace(dto.Cpf)) usuario.Cpf = dto.Cpf;
        if (!string.IsNullOrWhiteSpace(dto.Formacao)) usuario.Formacao = dto.Formacao;
        if (!string.IsNullOrWhiteSpace(dto.Telefone1)) usuario.Telefone1 = dto.Telefone1;
        if (!string.IsNullOrWhiteSpace(dto.Whatsapp)) usuario.Whatsapp = dto.Whatsapp;

        // Altera a senha, se fornecida
        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            var passwordResult = await _userRepository.UpdateUserPasswordAsync(usuario, dto.Senha);
            if (!passwordResult.Succeeded)
            {
                return new ServiceResult { Succeeded = false, Errors = passwordResult.Errors.Select(e => e.Description).ToList() };
            }
        }

        await _userRepository.SaveChangesAsync();
        return new ServiceResult { Succeeded = true, Message = "Administrador atualizado com sucesso." };
    }

    private async Task<ServiceResult> UpdateClinicUserAsync(EditarUsuarioSistemaDto dto)
    {
        var vinculo = await _userRepository.GetUserClinicLinkByIdAsync(dto.UpcId);
        if (vinculo?.Usuario == null) return new ServiceResult { Succeeded = false, Message = "Vínculo do usuário não encontrado." };

        var usuario = vinculo.Usuario;
        // A validação de email aqui pode ser refinada para ser específica da clínica se necessário
        if (await _userRepository.EmailExistsForAnotherUserAsync(dto.Email, usuario.Id))
        {
            return new ServiceResult { Succeeded = false, Message = "O e-mail informado já está em uso." };
        }

        // Atualiza os campos do usuário e do vínculo
        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.UserName = dto.Email;
        vinculo.Perfis = dto.Perfis ?? vinculo.Perfis;
        // Atualiza campos opcionais
        if (!string.IsNullOrWhiteSpace(dto.Cpf)) usuario.Cpf = dto.Cpf;
        if (!string.IsNullOrWhiteSpace(dto.Formacao)) usuario.Formacao = dto.Formacao;
        if (!string.IsNullOrWhiteSpace(dto.Telefone1)) usuario.Telefone1 = dto.Telefone1;
        if (!string.IsNullOrWhiteSpace(dto.Whatsapp)) usuario.Whatsapp = dto.Whatsapp;

        // Altera a senha, se fornecida
        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            var passwordResult = await _userRepository.UpdateUserPasswordAsync(usuario, dto.Senha);
            if (!passwordResult.Succeeded)
            {
                return new ServiceResult { Succeeded = false, Errors = passwordResult.Errors.Select(e => e.Description).ToList() };
            }
        }

        await _userRepository.SaveChangesAsync();
        return new ServiceResult { Succeeded = true, Message = "Usuário atualizado com sucesso." };
    }
}