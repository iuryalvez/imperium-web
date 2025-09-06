using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Relatorio;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.SaudeAnimal.Impl;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Infra.Data;
using Microsoft.AspNetCore.Identity;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly IConsultaRepository _consultaRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IDietaRepository _dietaRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public RelatorioService(
            IRelatorioRepository relatorioRepository,
            IConsultaRepository consultaRepository,
            IAnimalRepository animalRepository,
            IDietaRepository dietaRepository,
            IMapper mapper,
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _relatorioRepository = relatorioRepository;
            _consultaRepository = consultaRepository;
            _animalRepository = animalRepository;
            _dietaRepository = dietaRepository;
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId != null ? int.Parse(userId) : null;
        }

        public async Task<ServiceResult> UpsertAsync(int consultaId, UpsertRelatorioDto dto, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var consulta = await _consultaRepository.GetByIdAsync(consultaId, clinicaId);
                if (consulta?.Animal == null)
                    return new ServiceResult { Succeeded = false, Message = "Consulta ou animal associado não encontrado." };

                // Verifica se já existe um relatório para esta consulta
                var relatorio = await _relatorioRepository.GetByConsultaIdAsync(consultaId) ?? new Relatorio { ConsultaId = consultaId };

                relatorio.UsuarioResponsavelId = GetUserId() ?? 0;
                // Mapeia os dados do DTO para a entidade Relatorio
                _mapper.Map(dto, relatorio);
                relatorio.Idade = consulta.Animal.Idade; // Calcula a idade no momento do relatório

                // LÓGICA DE NEGÓCIO PRINCIPAL: Atualiza a entidade Animal com os dados do relatório
                var animal = consulta.Animal;
                animal.Peso = dto.Peso;
                animal.CondicaoCorporal = dto.CondicaoCorporal;
                animal.GrauAtividade = dto.GrauAtividade;
                animal.IsGestante = dto.IsGestante;
                animal.IsLactanteNumeroFilhotes = dto.IsLactanteNumeroFilhotes;
                animal.SemanaGestacao = dto.SemanaGestacao;
                animal.SemanaLactacao = dto.SemanaLactacao;
                animal.DataModificado = DateTime.UtcNow;

                // Se for um novo relatório, adiciona ao contexto
                if (relatorio.Id == 0)
                {
                    await _relatorioRepository.AddAsync(relatorio);
                }

                // Exemplo: escolhe a ração associada ao animal (ou null se não houver)
                var racao = await _context.Racoes.FirstOrDefaultAsync(r => r.Id == dto.RacaoId);

                var acompanhamento = animal.AcompanhamentoIndividualizado;

                if (acompanhamento.Relatorios == null)
                    acompanhamento.Relatorios = new List<Relatorio>();

                if (!acompanhamento.Relatorios.Contains(relatorio))
                    acompanhamento.Relatorios.Add(relatorio);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var resultDto = _mapper.Map<GetRelatorioDto>(relatorio);
                return new ServiceResult { Succeeded = true, Message = "Relatório salvo com sucesso.", Data = resultDto };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Logar 'ex'
                return new ServiceResult { Succeeded = false, Message = "Erro ao salvar o relatório." };
            }
        }

        public async Task<GetRelatorioDto?> GetByConsultaIdAsync(int consultaId, int clinicaId)
        {
            // Primeiro, valida se a consulta pertence à clínica do usuário
            var consulta = await _consultaRepository.GetByIdAsync(consultaId, clinicaId);
            if (consulta == null)
            {
                return null; // Consulta não existe ou não pertence à clínica
            }

            var relatorio = await _relatorioRepository.GetByConsultaIdAsync(consultaId);
            if (relatorio == null)
            {
                return null; // Ainda não existe um relatório para esta consulta
            }

            return _mapper.Map<GetRelatorioDto>(relatorio);
        }

        public async Task<ServiceResult> DeleteAsync(int relatorioId, int clinicaId)
        {
            var relatorio = await _relatorioRepository.GetByIdWithConsultaAsync(relatorioId);

            if (relatorio == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Relatório não encontrado." };
            }

            // Verificação de segurança: o relatório pertence a uma consulta da clínica correta?
            if (relatorio.Consulta?.ClinicaId != clinicaId)
            {
                return new ServiceResult { Succeeded = false, Message = "Acesso negado. O relatório não pertence a esta clínica." };
            }

            _relatorioRepository.Delete(relatorio);

            await _context.SaveChangesAsync(); // salva todas as alterações (+ de 1 entidade envolvida)

            return new ServiceResult { Succeeded = true, Message = "Relatório excluído com sucesso." };
        }
    }
}