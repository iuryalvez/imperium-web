using System.Security.Claims;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data; // Para AppDbContext (apenas para transação)

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IConsultaService _consultaService;
        private readonly IAnimalService _animalService; // Necessário para atualizar o animal
        private readonly AppDbContext _context; // Mantido apenas para controle de transação
        private readonly IMapper _mapper;

        public RelatorioService(
            IRelatorioRepository relatorioRepository,
            IConsultaService consultaService,
            IAnimalService animalService,
            IMapper mapper,
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _relatorioRepository = relatorioRepository;
            _consultaService = consultaService;
            _animalService = animalService;
            _mapper = mapper;
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

        // Dentro do RelatorioService
        public async Task<ServiceResult> UpsertAsync(int consultaId, UpsertRelatorioDto dto, int clinicaId)
        {
            var consultaResult = await _consultaService.ObterPorIdComDetalhesAsync(consultaId);
            var consulta = (Consulta)consultaResult.Data;
            if (consulta?.Animal?.AcompanhamentoIndividualizado == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta, animal ou acompanhamento associado não encontrado." };
            }
            var animal = consulta.Animal;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var resultadoUpdateAnimal = await _animalService.AtualizarEstadoComDadosRelatorioAsync(animal.Id, clinicaId, dto);
                if (!resultadoUpdateAnimal.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoUpdateAnimal; // Retorna o erro vindo do AnimalService
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