using System.Security.Claims;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;
using backend.Infra.Data.Repositories; // Para AppDbContext (apenas para transação)

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IConsultaService _consultaService;
        private readonly IAnimalRepository _animalRepository; // Necessário para atualizar o animal
        private readonly AppDbContext _context; // Mantido apenas para controle de transação
        private readonly ILogger<RelatorioService> _logger;
        private readonly IMapper _mapper;

        public RelatorioService(
            IRelatorioRepository relatorioRepository,
            IConsultaService consultaService,
            IAnimalRepository animalRepository,
            IMapper mapper,
            ILogger<RelatorioService> logger,
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _relatorioRepository = relatorioRepository;
            _consultaService = consultaService;
            _animalRepository = animalRepository;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // --- MÉTODOS DE LEITURA (PARA USO INTERNO E EXTERNO) ---

        public async Task<ServiceResult> ObterPorIdEIdAcompanhamentoAsync(int relatorioId, int acompanhamentoId, int clinicaId)
        {
            var relatorio = await _relatorioRepository.GetByIdAndAcompanhamentoIdAsync(relatorioId, acompanhamentoId);
            if (relatorio == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Relatório não encontrado ou não pertence a este acompanhamento." };
            }

            var consultaResult = await _consultaService.ObterPorIdAsync(relatorio.ConsultaId);
            var consulta = (Consulta)consultaResult.Data;
            if (consultaResult == null || consulta.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Acesso negado ao relatório." };
            }

            return new ServiceResult { Succeeded = true, Data = relatorio };
        }

        public async Task<ServiceResult> ObterPorIdAsync(int relatorioId, int clinicaId)
        {
            var relatorio = await _relatorioRepository.GetByIdWithConsultaAsync(relatorioId);
            if (relatorio == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Relatório não encontrado." };
            }

            if (relatorio.Consulta == null || relatorio.Consulta.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Acesso negado ao relatório." };
            }

            return new ServiceResult { Succeeded = true, Data = relatorio };
        }

        public async Task<ServiceResult> ObterPorConsultaIdAsync(int consultaId, int clinicaId)
        {
            var consultaResult = await _consultaService.ObterPorIdAsync(consultaId);
            var consulta = (Consulta)consultaResult.Data;
            if (consulta == null || consulta.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada nesta clínica." };
            }

            var relatorio = await _relatorioRepository.GetByConsultaIdAsync(consultaId);
            if (relatorio == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Nenhum relatório encontrado para esta consulta." };
            }

            return new ServiceResult { Succeeded = true, Data = relatorio };
        }

        public async Task<ServiceResult> GetDtoByConsultaIdAsync(int consultaId, int clinicaId)
        {
            // REUTILIZAÇÃO: Chama o método de serviço que já contém a lógica de validação.
            var resultadoBusca = await ObterPorConsultaIdAsync(consultaId, clinicaId);
            if (!resultadoBusca.Succeeded)
            {
                return resultadoBusca;
            }

            var relatorio = (Relatorio)resultadoBusca.Data;
            var relatorioDto = _mapper.Map<GetRelatorioDto>(relatorio);

            return new ServiceResult { Succeeded = true, Data = relatorioDto };
        }

        // --- MÉTODOS DE ESCRITA (WRITE) ---

        public async Task<ServiceResult> UpsertAsync(int consultaId, UpsertRelatorioDto dto, int clinicaId)
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
                _logger.LogInformation("Animal (ID: {AnimalId}) encontrado. Verificando AcompanhamentoIndividualizado...", consulta.Animal.Id);

                if (consulta.Animal.AcompanhamentoIndividualizado == null)
                {
                    _logger.LogWarning("A propriedade 'AcompanhamentoIndividualizado' do Animal (ID: {AnimalId}) é nula.", consulta.Animal.Id);
                }
                else
                {
                    _logger.LogInformation("AcompanhamentoIndividualizado (ID: {AcompanhamentoId}) encontrado.", consulta.Animal.AcompanhamentoIndividualizado.Id);
                }
            }

            if (consulta?.Animal?.AcompanhamentoIndividualizado == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta, animal ou acompanhamento associado não encontrado." };
            }
            var animal = consulta.Animal;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var resultadoUpdateAnimal = await AtualizarEstadoComDadosRelatorioAsync(animal.Id, clinicaId, dto);
                if (!resultadoUpdateAnimal.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoUpdateAnimal; // Retorna o erro vindo do AnimalRepository
                }

                var resultadoBusca = await ObterPorConsultaIdAsync(consultaId, clinicaId);

                Relatorio relatorio;
                if (resultadoBusca.Succeeded)
                {
                    relatorio = (Relatorio)resultadoBusca.Data;
                }
                else
                {
                    relatorio = new Relatorio
                    {
                        ConsultaId = consultaId,
                        AcompanhamentoIndividualizadoId = animal.AcompanhamentoIndividualizado.Id
                    };
                }

                relatorio.UsuarioResponsavelId = GetUserId();
                _mapper.Map(dto, relatorio);
                relatorio.Idade = animal.Idade;

                if (relatorio.Id == 0)
                {
                    await _relatorioRepository.AddAsync(relatorio);
                }

                // O SaveChanges irá persistir tanto as alterações no Animal (rastreadas)
                // quanto as alterações no Relatório (adicionado/modificado).
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var resultDto = _mapper.Map<GetRelatorioDto>(relatorio);
                return new ServiceResult { Succeeded = true, Message = "Relatório salvo e animal atualizado com sucesso.", Data = resultDto };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Succeeded = false, Message = "Erro ao salvar o relatório." };
            }
        }

        /// <summary>
        /// Atualiza o estado de uma entidade Animal com base nos dados de um relatório.
        /// Este método NÃO persiste as alterações no banco de dados (não chama SaveChanges).
        /// Ele é projetado para ser usado como um auxiliar dentro de uma transação maior, como a criação de um relatório.
        /// </summary>
        /// <param name="animalId">ID do animal a ser atualizado.</param>
        /// <param name="clinicaId">ID da clínica para validação.</param>
        /// <param name="dadosRelatorio">DTO do relatório contendo os novos dados do animal.</param>
        /// <returns>ServiceResult com a entidade Animal atualizada em memória.</returns>
        public async Task<ServiceResult> AtualizarEstadoComDadosRelatorioAsync(int animalId, int clinicaId, UpsertRelatorioDto dadosRelatorio)
        {
            var animal = await _animalRepository.GetByIdAndClinicIdWithDetailsAsync(animalId, clinicaId);

            _mapper.Map(dadosRelatorio, animal);
            animal.DataModificado = DateTime.UtcNow;

            _logger.LogInformation("Estado da entidade Animal {AnimalId} atualizado em memória com dados do relatório.", animalId);
            return new ServiceResult { Succeeded = true, Message = "Estado do animal atualizado.", Data = animal };
        }

        public async Task<ServiceResult> DeleteAsync(int relatorioId, int clinicaId)
        {
            // REUTILIZAÇÃO: Usa o método de serviço que já valida o acesso e a existência.
            var resultadoBusca = await ObterPorIdAsync(relatorioId, clinicaId);
            if (!resultadoBusca.Succeeded)
            {
                return resultadoBusca;
            }

            var relatorio = (Relatorio)resultadoBusca.Data;
            _relatorioRepository.Delete(relatorio);
            await _context.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = "Relatório excluído com sucesso." };
        }

        private int? GetUserId()
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
}