using AutoMapper;
using backend.Domain.Usuarios.DTOs.Usuario;
using backend.Domain.Usuarios.DTOs.Vinculo;
using backend.Domain.Usuarios.DTOs.PerfilPessoal;
using backend.Domain.Usuarios.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;
using backend.Domain.Usuarios.DTOs.UsuarioStatus;

namespace backend.Domain.Usuarios.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ILogger<UsuarioService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context; // Para transações

        public UsuarioService(IUsuarioRepository usuarioRepository,
                              ILogger<UsuarioService> logger,
                              IMapper mapper,
                              AppDbContext context)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResult> ListarUsuariosSistemaAsync(int currentUserId)
        {
            try
            {
                var vinculos = await _usuarioRepository.GetAllVinculosSistemaAsync(); // existem usuários administradores (não possuem vínculo de fato)

                var usuariosComuns = _mapper.Map<List<GetUsuariosSistemaDto>>(vinculos); // pega todos os usuários (assim não há repetição de usuários)

                var idsUsuariosComuns = usuariosComuns.Select(u => u.Id).Distinct().ToList();
                var adminsNaoVinculados = await _usuarioRepository.GetUnlinkedAdminsAsync(idsUsuariosComuns, currentUserId);

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
            var vinculos = await _usuarioRepository.GetVinculosByClinicIdAsync(clinicaId);

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
                if (await _usuarioRepository.EmailExistsAsync(dto.Email))
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
                usuario.DataCadastro = DateTime.UtcNow;
                usuario.DataUltimoAcesso = DateTime.UtcNow;
                usuario.EmailConfirmed = true; // TODO: alterar depois para usar um emailService e validar o email recebido

                var (identityResult, novoUsuario) = await _usuarioRepository.CreateUserAsync(usuario, dto.Senha);
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
                        if (!await _usuarioRepository.ClinicaExistsAsync(clinicaId))
                        {
                            await transaction.RollbackAsync();
                            return new ServiceResult { Succeeded = false, Message = $"A clínica com ID {clinicaId} não existe." };
                        }

                        if (await _usuarioRepository.EmailExistsInClinicAsync(dto.Email, clinicaId))
                        {
                            await transaction.RollbackAsync();
                            return new ServiceResult { Succeeded = false, Message = "O email informado já está em uso nesta clínica." };
                        }

                        await _usuarioRepository.AddUserClinicLinkAsync(new UsuarioPerfilClinica
                        {
                            UsuarioId = novoUsuario.Id,
                            ClinicaId = clinicaId,
                            Perfis = dto.Perfis
                        });
                    }
                }

                await _usuarioRepository.SaveChangesAsync();
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

        public async Task<ServiceResult> CadastrarUsuarioClinicaAsync(CadastrarUsuarioClinicaDto dto, int clinicaId)
        {
            // Validações de Negócio Iniciais
            if (dto.Perfis == null || !dto.Perfis.Any())
            {
                return new ServiceResult { Succeeded = false, Message = "Pelo menos um perfil deve ser selecionado." };
            }

            // O repositório agora centraliza as interações com o banco
            if (!await _usuarioRepository.ClinicaExistsAsync(clinicaId))
            {
                return new ServiceResult { Succeeded = false, Message = "A clínica especificada não existe." };
            }

            // O usuário é global (e-mail único no sistema). Se o e-mail já existe, apenas vincula o usuário
            // existente a esta clínica com os perfis escolhidos; os demais campos do DTO são ignorados
            // (nome, CPF, telefone e senha só são alterados pelo próprio usuário ou pelo admin do sistema).
            var usuarioExistente = await _usuarioRepository.GetUserByEmailAsync(dto.Email);
            if (usuarioExistente != null)
            {
                return await VincularUsuarioExistenteAsync(usuarioExistente, dto.Perfis, clinicaId);
            }

            // Inicia a transação a partir do repositório/contexto
            await using var transaction = await _context.Database.BeginTransactionAsync(); // Ou uma abstração no repo

            try
            {
                // Usa o AutoMapper para criar a entidade Usuario a partir do DTO
                var usuario = _mapper.Map<Usuario>(dto);
                // Atribui valores padrão que não vêm do DTO
                usuario.UserName = dto.UserName ?? dto.Email;
                usuario.DataCadastro = DateTime.UtcNow;
                usuario.DataUltimoAcesso = DateTime.UtcNow;
                usuario.EmailConfirmed = true;
                usuario.IsAdmin = false; // Usuário de clínica nunca é admin do sistema
                usuario.IsAtivo = true;

                // Chama o repositório para criar o usuário no Identity
                var (identityResult, novoUsuario) = await _usuarioRepository.CreateUserAsync(usuario, dto.Senha);

                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult
                    {
                        Succeeded = false,
                        Errors = identityResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                // Cria a entidade de vínculo
                var usuarioPerfilClinica = new UsuarioPerfilClinica
                {
                    UsuarioId = novoUsuario.Id,
                    ClinicaId = clinicaId,
                    Perfis = dto.Perfis
                };

                // Chama o repositório para adicionar o vínculo
                await _usuarioRepository.AddUserClinicLinkAsync(usuarioPerfilClinica);
                await _usuarioRepository.SaveChangesAsync();

                // Confirma a transação
                await transaction.CommitAsync();

                return new ServiceResult
                {
                    Succeeded = true,
                    Message = "Usuário criado com sucesso.",
                    Data = new { UsuarioId = novoUsuario.Id } // Retorna dados úteis
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro inesperado ao cadastrar usuário na clínica.");
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro no servidor." };
            }
        }

        /// <summary>
        /// Vincula um usuário que já existe no sistema à clínica, criando apenas o UsuarioPerfilClinica.
        /// </summary>
        private async Task<ServiceResult> VincularUsuarioExistenteAsync(Usuario usuario, List<string> perfis, int clinicaId)
        {
            if (usuario.IsAdmin)
            {
                return new ServiceResult { Succeeded = false, Message = "Este e-mail pertence a um administrador do sistema e não pode ser vinculado à clínica." };
            }

            if (await _usuarioRepository.IsUserLinkedToClinicAsync(usuario.Id, clinicaId))
            {
                return new ServiceResult { Succeeded = false, Message = "Este usuário já está vinculado a esta clínica." };
            }

            await _usuarioRepository.AddUserClinicLinkAsync(new UsuarioPerfilClinica
            {
                UsuarioId = usuario.Id,
                ClinicaId = clinicaId,
                Perfis = perfis
            });
            await _usuarioRepository.SaveChangesAsync();

            _logger.LogInformation("Usuário existente {UsuarioId} vinculado à clínica {ClinicaId}.", usuario.Id, clinicaId);
            return new ServiceResult
            {
                Succeeded = true,
                Message = "Este e-mail já tinha cadastro no sistema. O usuário foi vinculado à clínica com os perfis escolhidos; os dados pessoais e a senha dele não foram alterados.",
                Data = new { UsuarioId = usuario.Id, Vinculado = true }
            };
        }

        public async Task<ServiceResult> SetUserActivationStatusAsync(SetUserStatusDto dto)
        {
            if (dto.IsAdmin)
            {
                // --- LÓGICA PARA ADMINISTRADORES ---
                var usuario = await _usuarioRepository.GetUserByIdAsync(dto.Id);
                if (usuario == null || !usuario.IsAdmin)
                    return new ServiceResult { Succeeded = false, Message = "Usuário administrador não encontrado." };

                // Define o status diretamente na entidade principal do usuário
                usuario.IsAtivo = dto.Activate;

                await _usuarioRepository.SaveChangesAsync();
                string acao = dto.Activate ? "ativado" : "inativado";
                return new ServiceResult { Succeeded = true, Message = $"Administrador {acao} com sucesso." };
            }
            else
            {
                // --- LÓGICA PARA USUÁRIOS DE CLÍNICA (BASEADA NO VÍNCULO) ---
                var vinculo = await _usuarioRepository.GetUserClinicLinkByIdAsync(dto.Id); // Aqui, dto.Id é o upcId
                if (vinculo?.Usuario == null)
                    return new ServiceResult { Succeeded = false, Message = "Vínculo do usuário não encontrado." };

                // Rotas de clínica informam a clínica do token: o vínculo precisa pertencer a ela
                if (dto.ClinicaId.HasValue && vinculo.ClinicaId != dto.ClinicaId.Value)
                    return new ServiceResult { Succeeded = false, Message = "Vínculo do usuário não encontrado nesta clínica." };

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
                    var todosOsVinculos = await _usuarioRepository.GetAllVinculosByUserIdAsync(vinculo.UsuarioId);
                    bool todosInativos = todosOsVinculos.All(v => v.Perfis?.Contains("INATIVO") ?? false);

                    if (todosInativos)
                    {
                        vinculo.Usuario.IsAtivo = false; // Inativa o usuário principal
                    }
                }

                await _usuarioRepository.SaveChangesAsync();
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
            var vinculo = await _usuarioRepository.GetVinculoByIdAsync(upcId);
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

            await _usuarioRepository.RemoveVinculoAsync(vinculo);
            await _usuarioRepository.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = "Usuário desvinculado da clínica com sucesso." };
        }

        // Métodos privados para organizar a lógica de cada tipo de usuário
        private async Task<ServiceResult> UpdateAdminUserAsync(EditarUsuarioSistemaDto dto)
        {
            var usuario = await _usuarioRepository.GetAdminUserByIdAsync(dto.Id);
            if (usuario == null) return new ServiceResult { Succeeded = false, Message = "Usuário administrador não encontrado." };

            // Validação de e-mail duplicado
            if (await _usuarioRepository.EmailExistsForAnotherUserAsync(dto.Email, dto.Id))
            {
                return new ServiceResult { Succeeded = false, Message = "O e-mail informado já está em uso." };
            }

            _mapper.Map(dto, usuario);

            // Altera a senha, se fornecida
            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                var passwordResult = await _usuarioRepository.UpdateUserPasswordAsync(usuario, dto.Senha);
                if (!passwordResult.Succeeded)
                {
                    return new ServiceResult { Succeeded = false, Errors = passwordResult.Errors.Select(e => e.Description).ToList() };
                }
            }

            await _usuarioRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Administrador atualizado com sucesso." };
        }

        private async Task<ServiceResult> UpdateClinicUserAsync(EditarUsuarioSistemaDto dto)
        {
            var vinculo = await _usuarioRepository.GetUserClinicLinkByIdAsync(dto.UpcId);
            if (vinculo?.Usuario == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Vínculo do usuário não encontrado." };
            }

            var usuario = vinculo.Usuario;
            if (await _usuarioRepository.EmailExistsForAnotherUserAsync(dto.Email, usuario.Id))
            {
                return new ServiceResult { Succeeded = false, Message = "O e-mail informado já está em uso." };
            }

            // 1. Mapeia os campos que pertencem à entidade Usuario (Nome, Email, Cpf, etc.)
            _mapper.Map(dto, usuario);

            // 2. ATUALIZAÇÃO EXPLÍCITA: Mapeia manualmente o campo que pertence à entidade de vínculo.
            //    Verificamos se o DTO contém a lista de perfis para não apagar perfis existentes
            //    caso o frontend não envie a lista (embora devesse enviar uma lista vazia para limpar).
            if (dto.Perfis != null)
            {
                vinculo.Perfis = dto.Perfis;
            }

            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                var passwordResult = await _usuarioRepository.UpdateUserPasswordAsync(usuario, dto.Senha);
                if (!passwordResult.Succeeded)
                {
                    return new ServiceResult { Succeeded = false, Errors = passwordResult.Errors.Select(e => e.Description).ToList() };
                }
            }

            // 3. Salva as alterações. 
            await _usuarioRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Usuário atualizado com sucesso." };
        }

        /// <summary>
        /// Edição feita pela clínica: altera apenas o vínculo (perfis). Dados pessoais e senha do usuário
        /// só podem ser alterados pelo próprio usuário (Meu Perfil) ou pelo admin do sistema.
        /// </summary>
        public async Task<ServiceResult> UpdateUsuarioClinicaAsync(EditarUsuarioClinicaDto dto, int clinicaId)
        {
            try
            {
                var vinculo = await _usuarioRepository.GetUserClinicLinkByIdAsync(dto.UpcId);
                if (vinculo?.Usuario == null || vinculo.ClinicaId != clinicaId)
                {
                    return new ServiceResult { Succeeded = false, Message = "Vínculo do usuário não encontrado nesta clínica." };
                }

                if (dto.Perfis == null || !dto.Perfis.Any())
                {
                    return new ServiceResult { Succeeded = false, Message = "Pelo menos um perfil deve ser selecionado." };
                }

                vinculo.Perfis = dto.Perfis;

                await _usuarioRepository.SaveChangesAsync();
                return new ServiceResult { Succeeded = true, Message = "Perfis do usuário atualizados com sucesso." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o vínculo {UpcId} da clínica {ClinicaId}.", dto.UpcId, clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Erro inesperado ao atualizar usuário." };
            }
        }

        public async Task<ServiceResult> GetMeuPerfilAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.GetUserWithTotalClinicsById(usuarioId);
            if (usuario == null)
                return new ServiceResult { Succeeded = false, Message = "Usuário não encontrado." };

            var data = _mapper.Map<GetMeuPerfilDto>(usuario);
            return new ServiceResult { Succeeded = true, Data = data };
        }

        public async Task<ServiceResult> UpdateMeuPerfilAsync(int usuarioId, UpdateMeuPerfilDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = await _usuarioRepository.GetUserByIdAsync(usuarioId);
                if (usuario == null)
                    return new ServiceResult { Succeeded = false, Message = "Usuário não encontrado." };

                if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != usuario.Email)
                {
                    if (await _usuarioRepository.EmailExistsForAnotherUserAsync(dto.Email, usuarioId))
                        return new ServiceResult { Succeeded = false, Message = "E-mail já está em uso." };
                }

                _mapper.Map(dto, usuario);

                if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
                {
                    if (string.IsNullOrWhiteSpace(dto.SenhaAtual))
                        return new ServiceResult { Succeeded = false, Message = "A senha atual é obrigatória para redefinição." };

                    // Validates the current credentials via Identity before applying the new password
                    var passwordResult = await _usuarioRepository.ChangeUserPasswordAsync(usuario, dto.SenhaAtual, dto.NovaSenha);
                    if (!passwordResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Errors = passwordResult.Errors.Select(e => e.Description).ToList() };
                    }
                }

                await _usuarioRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceResult { Succeeded = true, Message = "Perfil atualizado com sucesso." };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao atualizar perfil do usuário {UsuarioId}", usuarioId);
                return new ServiceResult { Succeeded = false, Message = "Erro interno ao atualizar o perfil." };
            }
        }
    }
}