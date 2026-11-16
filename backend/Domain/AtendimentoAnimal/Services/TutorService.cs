using backend.Domain.AtendimentoAnimal.Mappers;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IAnimalRepository _animalRepository; // Para validar animais
        private readonly ILogger<TutorService> _logger;
        private readonly AppDbContext _context; // Para transações

        public TutorService(ITutorRepository tutorRepository,
                            IAnimalRepository animalRepository,
                            ILogger<TutorService> logger,
                            AppDbContext context)
        {
            _tutorRepository = tutorRepository;
            _animalRepository = animalRepository;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Obtém a entidade de um tutor específico pelo seu ID, validando o acesso pela clínica.
        /// Este método retorna a entidade de domínio completa.
        /// </summary>
        /// <param name="tutorId">O ID do tutor a ser buscado.</param>
        /// <param name="clinicaId">O ID da clínica para validação de acesso.</param>
        /// <returns>A entidade 'Tutor' ou nulo se não for encontrado ou o acesso for negado.</returns>
        public async Task<Tutor?> ObterPorIdEClinicaIdAsync(int tutorId, int clinicaId)
        {
            _logger.LogInformation("Buscando entidade Tutor ID {TutorId} para a clínica ID {ClinicaId}.", tutorId, clinicaId);
            try
            {
                var tutor = await _tutorRepository.GetByIdAndClinicIdWithIncludesAsync(tutorId, clinicaId);

                if (tutor == null)
                {
                    _logger.LogWarning("Nenhuma entidade Tutor encontrada para o ID {TutorId} na clínica {ClinicaId}.", tutorId, clinicaId);
                }

                return tutor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar a entidade Tutor ID {TutorId}.", tutorId);
                throw; // Relança a exceção para ser tratada por uma camada superior
            }
        }

        /// <summary>
        /// Obtém um DTO de um tutor específico, fazendo uso do método de serviço que busca a entidade.
        /// </summary>
        /// <param name="tutorId">O ID do tutor a ser buscado.</param>
        /// <param name="clinicaId">O ID da clínica para validação de acesso.</param>
        /// <returns>O DTO 'GetTutoresClinicaDto' ou nulo se não for encontrado.</returns>
        public async Task<GetTutoresClinicaDto?> ObterDtoPorIdEClinicaId(int tutorId, int clinicaId)
        {
            _logger.LogInformation("Buscando DTO para o Tutor ID {TutorId} na clínica ID {ClinicaId}.", tutorId, clinicaId);

            // 1. Chama o método de serviço intermediário para obter a entidade
            var tutor = await ObterPorIdEClinicaIdAsync(tutorId, clinicaId);

            // 2. Mapeia a entidade para DTO (nulo quando o tutor não existe na clínica).
            return tutor == null ? null : TutorMapper.ParaDto(tutor);
        }

        /// <summary>
        /// Lista paginada dos tutores da clínica (ativos e inativos), mais recentes primeiro por padrão.
        /// </summary>
        public async Task<PaginaResultado<GetTutoresClinicaDto>> ListarAsync(TutorFiltro filtro, int clinicaId)
        {
            var pagina = await _tutorRepository.ListarDaClinicaAsync(filtro, clinicaId);
            return pagina.Converter(itens => itens.Select(TutorMapper.ParaDto));
        }

        /// <summary>
        /// Opções para selects: até 20 tutores { id, nome, isAtivo }, ativos primeiro.
        /// O CPF entra no nome (e na busca) só para quem pode ver dados do tutor; o TECNICO vê só o nome.
        /// </summary>
        public async Task<List<OpcaoDto>> ListarOpcoesAsync(int clinicaId, string? busca, bool incluirCpf)
        {
            return await _tutorRepository.ListarOpcoesAsync(clinicaId, busca, incluirCpf);
        }

        /// <summary>
        /// Valida se uma lista de IDs de tutores é válida e se todos pertencem à clínica especificada.
        /// </summary>
        /// <param name="tutoresIds">Lista de IDs de tutores a validar. Pode ser nula ou vazia.</param>
        /// <param name="clinicaId">ID da clínica para a verificação.</param>
        /// <returns>ServiceResult indicando sucesso se todos os tutores forem válidos, ou falha caso contrário.</returns>
        public async Task<ServiceResult> ValidarTutoresDaClinicaAsync(List<int> tutoresIds, int clinicaId)
        {
            // 1. Trata o caso de não haver tutores para validar (cenário válido).
            // Se a lista for nula ou não contiver nenhum elemento, a validação passa com sucesso.
            if (tutoresIds == null || !tutoresIds.Any())
            {
                _logger.LogInformation("Nenhum ID de tutor fornecido para validação na clínica {ClinicaId}. Validação bem-sucedida.", clinicaId);
                return new ServiceResult { Succeeded = true };
            }

            _logger.LogInformation("Validando {Count} tutor(es) para a clínica {ClinicaId}.", tutoresIds.Count, clinicaId);

            // 2. Chama o repositório para verificar se todos os tutores da lista existem na clínica.
            var saoTodosValidos = await _tutorRepository.AllTutorsExistInClinicAsync(tutoresIds, clinicaId);

            // 3. Lógica corrigida: Se NÃO forem todos válidos, retorna um erro.
            if (!saoTodosValidos)
            {
                _logger.LogWarning("Validação de tutores falhou. Um ou mais tutores da lista {@TutoresIds} não pertencem à clínica {ClinicaId}.", tutoresIds, clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Um ou mais tutores selecionados não são válidos ou não pertencem a esta clínica." };
            }

            // Se a execução chega aqui, significa que todos os tutores são válidos.
            _logger.LogInformation("Todos os {Count} tutores foram validados com sucesso para a clínica {ClinicaId}.", tutoresIds.Count, clinicaId);
            return new ServiceResult { Succeeded = true, Message = "Todos os tutores pertencem à clínica." };
        }

        public async Task<ServiceResult> CreateAsync(CadastrarTutorClinicaDto dto, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // O tutor pertence à clínica: o CPF só é verificado dentro da clínica atual
                if (await _tutorRepository.CpfExistsInClinicAsync(dto.Cpf, clinicaId))
                {
                    await transaction.RollbackAsync();
                    return ServiceResult.Conflito("CPF já cadastrado nesta clínica.");
                }

                var tutor = TutorMapper.ParaEntidade(dto);
                tutor.ClinicaId = clinicaId;
                tutor.DataCadastro = DateTime.UtcNow;
                tutor.IsAtivo = true;
                await _tutorRepository.AddAsync(tutor);
                await _tutorRepository.SaveChangesAsync(); // Salva para obter o ID

                // Lógica de vínculo com animais (se fornecido)
                // Verifique se o nome da propriedade no seu DTO é AnimalIds (plural) para dar match com o JSON
                if (dto.AnimalIds != null && dto.AnimalIds.Any())
                {
                    var allAnimalsAreValid = await _animalRepository.AllAnimalsExistInClinicAsync(dto.AnimalIds, clinicaId);
                    if (!allAnimalsAreValid)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Message = "Um ou mais animais selecionados não são válidos ou não pertencem à clínica." };
                    }
                    if (!await _animalRepository.AllAnimalsActiveAsync(dto.AnimalIds, clinicaId))
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Message = "Um ou mais animais selecionados estão inativos." };
                    }

                    foreach (var animalId in dto.AnimalIds)
                    {
                        await _tutorRepository.AddAnimalLinkAsync(new TutorAnimal
                        {
                            TutorId = tutor.Id,
                            AnimalId = animalId,
                            DataAssociacao = DateTime.UtcNow
                        });
                    }
                }

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResult { Succeeded = true, Message = "Tutor cadastrado com sucesso.", Data = new { TutorId = tutor.Id } };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao cadastrar tutor na clínica {ClinicaId}.", clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Erro ao cadastrar tutor." };
            }
        }

        public async Task<ServiceResult> UpdateAsync(EditarTutorClinicaDto dto, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tutor = await _tutorRepository.GetByIdAndClinicIdWithIncludesAsync(dto.Id, clinicaId);
                if (tutor == null)
                {
                    await transaction.RollbackAsync();
                    return ServiceResult.NaoEncontrado($"Tutor com ID {dto.Id} não encontrado nesta clínica.");
                }

                if (!string.IsNullOrWhiteSpace(dto.Cpf) && dto.Cpf != tutor.Cpf
                    && await _tutorRepository.CpfExistsInClinicAsync(dto.Cpf, clinicaId, tutor.Id))
                {
                    await transaction.RollbackAsync();
                    return ServiceResult.Conflito("CPF já cadastrado nesta clínica.");
                }

                // 1. MAPEAMENTO: aplica na entidade só o que veio preenchido no DTO (ver TutorMapper)
                TutorMapper.Aplicar(dto, tutor);

                // 2. SINCRONIZAÇÃO DE VÍNCULOS
                if (dto.AnimalIds != null)
                {
                    // 2a. VALIDAÇÃO OTIMIZADA
                    // Valida todos os animais com uma única chamada ao banco.
                    var allAnimalsAreValid = await _animalRepository.AllAnimalsExistInClinicAsync(dto.AnimalIds, clinicaId);
                    if (!allAnimalsAreValid)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Message = "Um ou mais animais selecionados não são válidos ou não pertencem à clínica." };
                    }

                    var idsAnimaisAtuais = tutor.TutoresAnimais.Select(ta => ta.AnimalId).ToList();
                    var idsAnimaisDesejados = dto.AnimalIds;

                    // Animais inativos não podem ganhar vínculo novo (os vínculos já existentes são mantidos)
                    if (!await _animalRepository.AllAnimalsActiveAsync(idsAnimaisDesejados.Except(idsAnimaisAtuais).ToList(), clinicaId))
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Message = "Um ou mais animais selecionados estão inativos." };
                    }

                    // 2b. Remove vínculos
                    var vinculosParaRemover = tutor.TutoresAnimais.Where(ta => !idsAnimaisDesejados.Contains(ta.AnimalId)).ToList();
                    if (vinculosParaRemover.Any())
                    {
                        _tutorRepository.RemoveAnimalLinks(vinculosParaRemover);
                    }

                    // 2c. Adiciona novos vínculos
                    var idsParaAdicionar = idsAnimaisDesejados.Except(idsAnimaisAtuais).ToList();
                    foreach (var animalId in idsParaAdicionar)
                    {
                        await _tutorRepository.AddAnimalLinkAsync(new TutorAnimal { TutorId = tutor.Id, AnimalId = animalId, DataAssociacao = DateTime.UtcNow });
                    }
                }

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceResult { Succeeded = true, Message = "Tutor atualizado com sucesso.", Data = new { TutorId = tutor.Id } };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao atualizar o tutor {TutorId} na clínica {ClinicaId}.", dto.Id, clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro interno ao atualizar o tutor." };
            }
        }

        public async Task<ServiceResult> SetTutorStatusAsync(int tutorId, int clinicaId, bool ativo)
        {
            var tutor = await _tutorRepository.GetByIdAndClinicIdAsync(tutorId, clinicaId);
            if (tutor == null)
            {
                return ServiceResult.NaoEncontrado("Tutor não encontrado nesta clínica.");
            }

            tutor.IsAtivo = ativo;
            tutor.DataDesativacao = ativo ? null : DateTime.UtcNow;

            await _tutorRepository.SaveChangesAsync();

            return new ServiceResult
            {
                Succeeded = true,
                Message = ativo ? "Tutor reativado com sucesso." : "Tutor desativado com sucesso."
            };
        }

        public async Task<ServiceResult> DeleteAsync(int tutorId, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tutor = await _tutorRepository.GetByIdAndClinicIdWithIncludesAsync(tutorId, clinicaId);
                if (tutor == null)
                    return ServiceResult.NaoEncontrado("Tutor não encontrado nesta clínica.");

                // Consultas guardam o histórico do tutor: nesse caso ele deve ser desativado, não excluído
                if (await _tutorRepository.HasConsultasAsync(tutor.Id))
                    return ServiceResult.Conflito("Este tutor possui consultas registradas e não pode ser excluído. Desative-o.");

                // Remove os vínculos Tutor-Animal (todos da mesma clínica) e o tutor
                if (tutor.TutoresAnimais != null && tutor.TutoresAnimais.Any())
                {
                    _tutorRepository.RemoveAnimalLinks(tutor.TutoresAnimais);
                }
                _tutorRepository.Remove(tutor);

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResult { Succeeded = true, Message = "Tutor removido com sucesso." };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao remover o tutor {TutorId} da clínica {ClinicaId}.", tutorId, clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Erro ao remover tutor." };
            }
        }
    }
}
