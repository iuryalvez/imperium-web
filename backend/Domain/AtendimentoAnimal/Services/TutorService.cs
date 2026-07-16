using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.Usuarios.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IAnimalRepository _animalRepository; // Para validar animais
        private readonly IMapper _mapper;
        private readonly ILogger<TutorService> _logger;
        private readonly AppDbContext _context; // Para transações

        public TutorService(ITutorRepository tutorRepository,
                            IAnimalRepository animalRepository,
                            IMapper mapper,
                            ILogger<TutorService> logger,
                            AppDbContext context)
        {
            _tutorRepository = tutorRepository;
            _animalRepository = animalRepository;
            _mapper = mapper;
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
        /// Obtém uma lista de todas as entidades de tutores de uma clínica específica.
        /// Este método retorna as entidades de domínio completas.
        /// </summary>
        /// <param name="clinicaId">O ID da clínica.</param>
        /// <returns>Uma coleção de entidades 'Tutor'.</returns>
        public async Task<IEnumerable<Tutor>> ObterTodosPorClinicaIdAsync(int clinicaId)
        {
            _logger.LogInformation("Buscando todas as entidades Tutor para a clínica ID {ClinicaId}.", clinicaId);
            try
            {
                return await _tutorRepository.GetAllByClinicIdWithIncludesAsync(clinicaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as entidades Tutor da clínica ID {ClinicaId}.", clinicaId);
                throw;
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

            // 2. Mapeia a entidade para DTO. O AutoMapper lida com o caso de 'tutor' ser nulo.
            return _mapper.Map<GetTutoresClinicaDto>(tutor);
        }

        /// <summary>
        /// Obtém uma lista de DTOs de todos os tutores de uma clínica, fazendo uso do método de serviço que busca as entidades.
        /// </summary>
        /// <param name="clinicaId">O ID da clínica.</param>
        /// <returns>Uma coleção de DTOs 'GetTutoresClinicaDto'.</returns>
        public async Task<IEnumerable<GetTutoresClinicaDto>> ObterTodosDtosPeloClinicaIdAsync(int clinicaId)
        {
            _logger.LogInformation("Buscando todos os DTOs de Tutores para a clínica ID {ClinicaId}.", clinicaId);

            // 1. Chama o método de serviço intermediário para obter as entidades
            var tutores = await ObterTodosPorClinicaIdAsync(clinicaId);

            // 2. Mapeia a coleção de entidades para uma coleção de DTOs.
            return _mapper.Map<IEnumerable<GetTutoresClinicaDto>>(tutores);
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
                _logger.LogDebug("Nenhum ID de tutor fornecido para validação na clínica {ClinicaId}. Validação bem-sucedida.", clinicaId);
                return new ServiceResult { Succeeded = true };
            }

            _logger.LogDebug("Validando {Count} tutor(es) para a clínica {ClinicaId}.", tutoresIds.Count, clinicaId);

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
                // Lógica para reutilizar tutor existente ou criar um novo
                var tutor = await _tutorRepository.FindByCpfAsync(dto.Cpf);
                if (tutor == null)
                {
                    // Tutor não existe no sistema, cria um novo
                    tutor = _mapper.Map<Tutor>(dto);
                    tutor.DataCadastro = DateTime.UtcNow;
                    await _tutorRepository.AddAsync(tutor);
                    await _tutorRepository.SaveChangesAsync(); // Salva para obter o ID
                }
                else
                {
                    // Tutor já existe, apenas atualiza seus dados com as novas informações
                    _mapper.Map(dto, tutor);
                }

                // Verifica se este tutor já está vinculado a esta clínica
                if (await _tutorRepository.IsTutorLinkedToClinicAsync(tutor.Id, clinicaId))
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Message = "Este tutor (CPF) já está cadastrado nesta clínica." };
                }

                // Cria o vínculo com a clínica
                await _tutorRepository.AddClinicLinkAsync(new TutorClinica { TutorId = tutor.Id, ClinicaId = clinicaId });

                // Lógica de vínculo com animais (se fornecido)
                if (dto.AnimalId != null && dto.AnimalId.Any())
                {
                    // Validação e vínculo dos animais aqui...
                }

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResult { Succeeded = true, Message = "Tutor cadastrado com sucesso.", Data = new { TutorId = tutor.Id } };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Logar 'ex'
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
                    return new ServiceResult { Succeeded = false, Message = $"Tutor com ID {dto.Id} não encontrado nesta clínica." };
                }

                // 1. MAPEAMENTO AUTOMÁTICO
                // O AutoMapper aplica as alterações do DTO na entidade existente,
                // ignorando propriedades nulas (se o Profile estiver configurado para isso).
                _mapper.Map(dto, tutor);

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
                // Logar o erro 'ex'
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro interno ao atualizar o tutor." };
            }
        }

        public async Task<ServiceResult> SetTutorStatusAsync(int tutorId, int clinicaId, bool ativo)
        {
            var link = await _tutorRepository.GetClinicLinkAsync(tutorId, clinicaId);
            if (link == null)
            {
                return new ServiceResult
                {
                    Succeeded = false,
                    Message = "Tutor não vinculado a esta clínica."
                };
            }

            link.IsAtivo = ativo;
            if (ativo)
            {
                link.DataAssociacao = DateTime.UtcNow;
                link.DataDesativacao = null;
            }
            else link.DataDesativacao = DateTime.UtcNow;

            await _tutorRepository.UpdateClinicLinkAsync(link);

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
                    return new ServiceResult { Succeeded = false, Message = "Tutor não encontrado nesta clínica." };

                // Remove todos os vínculos Tutor-Animal na clínica atual
                var vinculosAnimaisParaRemover = tutor.TutoresAnimais
                    .Where(ta => ta.Animal.AnimaisClinicas.Any(ac => ac.ClinicaId == clinicaId))
                    .ToList();
                if (vinculosAnimaisParaRemover.Any())
                {
                    _tutorRepository.RemoveAnimalLinks(vinculosAnimaisParaRemover);
                }

                // Remove o vínculo Tutor-Clínica
                var vinculoClinica = tutor.TutoresClinicas.First(tc => tc.ClinicaId == clinicaId);
                _tutorRepository.RemoveClinicLink(vinculoClinica);

                // Se o tutor não tiver mais nenhum vínculo com NENHUMA clínica, remove o tutor.
                if (tutor.TutoresClinicas.Count <= 1)
                {
                    _tutorRepository.Remove(tutor);
                }

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResult { Succeeded = true, Message = "Tutor removido da clínica com sucesso." };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Logar 'ex'
                return new ServiceResult { Succeeded = false, Message = "Erro ao remover tutor." };
            }
        }
    }
}
