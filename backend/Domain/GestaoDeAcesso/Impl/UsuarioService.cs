// Application/Services/UsuarioService.cs
using AutoMapper;
using backend.Domain.GestaoDeAcesso.DTOs;
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context; // Para transações

    public UsuarioService(IUsuarioRepository userRepository,
                          IMapper mapper,
                          AppDbContext context)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ServiceResult> ListarUsuariosSistemaAsync(int currentUserId)
    {
        try
        {
            var vinculos = await _userRepository.GetAllVinculosSistemaAsync(); // existem usuários administradores (não possuem vínculo de fato)

            var usuariosComuns = _mapper.Map<List<GetUsuariosSistemaDto>>(vinculos); // pega todos os usuários (assim não há repetição de usuários)

            var idsUsuariosComuns = usuariosComuns.Select(u => u.Id).Distinct().ToList();
            var adminsNaoVinculados = await _userRepository.GetUnlinkedAdminsAsync(idsUsuariosComuns, currentUserId);

            var usuariosAdministradores = _mapper.Map<List<GetUsuariosSistemaDto>>(adminsNaoVinculados);

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

        var vinculosFiltrados = vinculos
            .Where(upc => upc.UsuarioId.ToString() != currentUserId);

        return _mapper.Map<IEnumerable<GetUsuariosClinicaDto>>(vinculosFiltrados);
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
            var usuario = _mapper.Map<Usuario>(dto);
            usuario.UserName = dto.UserName ?? dto.Email;
            usuario.DataCad = DateTime.UtcNow;
            usuario.DataUltimoAcesso = DateTime.UtcNow;
            usuario.EmailConfirmed = true; // TODO: alterar depois para usar um emailService e validar o email recebido

            var (identityResult, novoUsuario) = await _userRepository.CreateUserAsync(usuario, dto.Senha);
            if (!identityResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Succeeded = false, Errors = identityResult.Errors.Select(e => e.Description).ToList() };
            }

            // Criar vínculos (se não for admin)
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

        _mapper.Map(dto, usuario);

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

        _mapper.Map(dto, usuario);

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

    public async Task<ServiceResult> UpdateUsuarioClinicaAsync(EditarUsuarioClinicaDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var vinculo = await _userRepository.GetUserClinicLinkByIdAsync(dto.UpcId);
            if (vinculo?.Usuario == null)
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Succeeded = false, Message = "Vínculo do usuário não encontrado." };
            }

            var usuario = vinculo.Usuario;

            // A validação de email duplicado deve ser no escopo da clínica
            if (await _userRepository.EmailExistsInClinicForAnotherUserAsync(dto.Email, vinculo.ClinicaId, usuario.Id))
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Succeeded = false, Message = "O e-mail informado já está em uso nesta clínica." };
            }

             _mapper.Map(dto, usuario);

            // Altera a senha, se uma nova foi fornecida
            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                var passwordResult = await _userRepository.UpdateUserPasswordAsync(usuario, dto.Senha);
                if (!passwordResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Errors = passwordResult.Errors.Select(e => e.Description).ToList() };
                }
            }

            await _userRepository.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResult { Succeeded = true, Message = "Usuário atualizado com sucesso." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Logar erro 'ex'
            return new ServiceResult { Succeeded = false, Message = "Erro inesperado ao atualizar usuário." };
        }
    }
}