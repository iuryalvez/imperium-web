using System.Security.Claims;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Infra.Data;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly IConsultaService _consultaService;
        private readonly IAnimalRepository _animalRepository; // Necessário para atualizar o animal
        private readonly AppDbContext _context; // Mantido apenas para controle de transação
        private readonly ILogger<AvaliacaoService> _logger;
        private readonly IMapper _mapper;

        public AvaliacaoService(
            IAvaliacaoRepository avaliacaoRepository,
            IConsultaService consultaService,
            IAnimalRepository animalRepository,
            IMapper mapper,
            ILogger<AvaliacaoService> logger,
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _avaliacaoRepository = avaliacaoRepository;
            _consultaService = consultaService;
            _animalRepository = animalRepository;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // --- MÉTODOS DE LEITURA (PARA USO INTERNO E EXTERNO) ---

        public async Task<ServiceResult> ObterPorIdEIdAcompanhamentoAsync(int avaliacaoId, int acompanhamentoId, int clinicaId)
        {
            var avaliacao = await _avaliacaoRepository.GetByIdAndAcompanhamentoIdAsync(avaliacaoId, acompanhamentoId);
            if (avaliacao == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Avaliação não encontrada ou não pertence a este acompanhamento." };
            }

            var consultaResult = await _consultaService.ObterPorIdAsync(avaliacao.ConsultaId);
            var consulta = (Consulta)consultaResult.Data;
            if (consultaResult == null || consulta.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Acesso negado à avaliação." };
            }

            return new ServiceResult { Succeeded = true, Data = avaliacao };
        }

        public async Task<ServiceResult> ObterPorIdAsync(int avaliacaoId, int clinicaId)
        {
            var avaliacao = await _avaliacaoRepository.GetByIdWithConsultaAsync(avaliacaoId);
            if (avaliacao == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Avaliação não encontrada." };
            }

            if (avaliacao.Consulta == null || avaliacao.Consulta.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Acesso negado à avaliação." };
            }

            return new ServiceResult { Succeeded = true, Data = avaliacao };
        }

        public async Task<ServiceResult> ObterPorConsultaIdAsync(int consultaId, int clinicaId)
        {
            var consultaResult = await _consultaService.ObterPorIdAsync(consultaId);
            var consulta = (Consulta)consultaResult.Data;
            if (consulta == null || consulta.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada nesta clínica." };
            }

            var avaliacao = await _avaliacaoRepository.GetByConsultaIdAsync(consultaId);
            if (avaliacao == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Nenhum relatório encontrado para esta consulta." };
            }

            return new ServiceResult { Succeeded = true, Data = avaliacao };
        }

        public async Task<ServiceResult> GetDtoByConsultaIdAsync(int consultaId, int clinicaId)
        {
            // REUTILIZAÇÃO: Chama o método de serviço que já contém a lógica de validação.
            var resultadoBusca = await ObterPorConsultaIdAsync(consultaId, clinicaId);
            if (!resultadoBusca.Succeeded)
            {
                return resultadoBusca;
            }

            var avaliacao = (Avaliacao)resultadoBusca.Data;
            var avaliacaoDto = _mapper.Map<GetAvaliacaoDto>(avaliacao);

            return new ServiceResult { Succeeded = true, Data = avaliacaoDto };
        }

        // --- MÉTODOS DE ESCRITA (WRITE) ---

        public async Task<ServiceResult> UpsertAsync(int consultaId, UpsertAvaliacaoDto dto, int clinicaId)
        {
            var consultaResult = await _consultaService.ObterPorIdComDetalhesAsync(consultaId);

            // Log do resultado do serviço de busca
            if (!consultaResult.Succeeded)
            {
                _logger.LogWarning("Falha ao obter consulta com detalhes. Motivo: {Message}", consultaResult.Message);
                return new ServiceResult { Succeeded = false, Message = consultaResult.Message };
            }

            var consulta = (Consulta)consultaResult.Data;

            if (consulta == null)
            {
                _logger.LogWarning("O objeto Consulta retornado por ObterPorIdComDetalhesAsync é nulo, embora o resultado tenha sido Succeeded.");
            }
            else
            {
                _logger.LogInformation("Animal (ID: {AnimalId}) encontrado. Verificando Acompanhamento...", consulta.Animal.Id);

                if (consulta.Animal.Acompanhamento == null)
                {
                    _logger.LogWarning("A propriedade 'Acompanhamento' do Animal (ID: {AnimalId}) é nula.", consulta.Animal.Id);
                }
                else
                {
                    _logger.LogInformation("Acompanhamento (ID: {AcompanhamentoId}) encontrado.", consulta.Animal.Acompanhamento.Id);
                }
            }

            if (consulta?.Animal?.Acompanhamento == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta, animal ou acompanhamento associado não encontrado." };
            }
            var animal = consulta.Animal;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var resultadoUpdateAnimal = await AtualizarEstadoComDadosAvaliacaoAsync(animal.Id, clinicaId, dto);
                if (!resultadoUpdateAnimal.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoUpdateAnimal; // Retorna o erro vindo do AnimalRepository
                }

                var resultadoBusca = await ObterPorConsultaIdAsync(consultaId, clinicaId);

                Avaliacao avaliacao;
                if (resultadoBusca.Succeeded)
                {
                    avaliacao = (Avaliacao)resultadoBusca.Data;
                }
                else
                {
                    avaliacao = new Avaliacao
                    {
                        ConsultaId = consultaId,
                        AcompanhamentoId = animal.Acompanhamento.Id
                    };
                }

                avaliacao.UsuarioResponsavelId = GetUserId();
                _mapper.Map(dto, avaliacao);
                avaliacao.Idade = animal.Idade;

                if (avaliacao.Id == 0)
                {
                    await _avaliacaoRepository.AddAsync(avaliacao);
                }

                // O SaveChanges irá persistir tanto as alterações no Animal (rastreadas)
                // quanto as alterações na avaliação (adicionado/modificado).
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var resultDto = _mapper.Map<GetAvaliacaoDto>(avaliacao);
                return new ServiceResult { Succeeded = true, Message = "Avaliação salva e animal atualizado com sucesso.", Data = resultDto };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Succeeded = false, Message = "Erro ao salvar avaliação." };
            }
        }

        /// <summary>
        /// Atualiza o estado de uma entidade Animal com base nos dados de um relatório.
        /// Este método NÃO persiste as alterações no banco de dados (não chama SaveChanges).
        /// Ele é projetado para ser usado como um auxiliar dentro de uma transação maior, como a criação de um relatório.
        /// </summary>
        /// <param name="animalId">ID do animal a ser atualizado.</param>
        /// <param name="clinicaId">ID da clínica para validação.</param>
        /// <param name="dadosAvaliacao">DTO do relatório contendo os novos dados do animal.</param>
        /// <returns>ServiceResult com a entidade Animal atualizada em memória.</returns>
        public async Task<ServiceResult> AtualizarEstadoComDadosAvaliacaoAsync(int animalId, int clinicaId, UpsertAvaliacaoDto dadosAvaliacao)
        {
            var animal = await _animalRepository.GetByIdAndClinicIdWithDetailsAsync(animalId, clinicaId);

            _mapper.Map(dadosAvaliacao, animal);
            animal.DataModificado = DateTime.UtcNow;

            _logger.LogInformation("Estado da entidade Animal {AnimalId} atualizado em memória com dados da avaliação.", animalId);
            return new ServiceResult { Succeeded = true, Message = "Estado do animal atualizado.", Data = animal };
        }

        public async Task<ServiceResult> DeleteAsync(int avaliacaoId, int clinicaId)
        {
            // REUTILIZAÇÃO: Usa o método de serviço que já valida o acesso e a existência.
            var resultadoBusca = await ObterPorIdAsync(avaliacaoId, clinicaId);
            if (!resultadoBusca.Succeeded)
            {
                return resultadoBusca;
            }

            var avaliacao = (Avaliacao)resultadoBusca.Data;
            _avaliacaoRepository.Delete(avaliacao);
            await _context.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = "Avaliação excluída com sucesso." };
        }

        private int? GetUserId()
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
}