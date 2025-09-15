using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class ConsultaService : IConsultaService
    {
        private readonly IConsultaRepository _consultaRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ConsultaService> _logger;

        public ConsultaService(IConsultaRepository consultaRepository,
                               IAnimalRepository animalRepository,
                               ITutorRepository tutorRepository,
                               ILogger<ConsultaService> logger,
                               IMapper mapper)
        {
            _consultaRepository = consultaRepository;
            _animalRepository = animalRepository;
            _tutorRepository = tutorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Busca uma consulta pelo seu id, tratando possíveis falhas.
        /// </summary>
        /// <param name="consultaId">O ID da consulta a ser buscada.</param>
        /// <returns>Um ServiceResult contendo a consulta em caso de sucesso ou uma mensagem de erro em caso de falha.</returns>
        public async Task<ServiceResult> ObterPorIdAsync(int consultaId)
        {
            _logger.LogInformation("Buscando consulta por ID: {ConsultaId}", consultaId);
            try
            {
                var consulta = await _consultaRepository.GetByIdAsync(consultaId);

                if (consulta == null)
                {
                    _logger.LogWarning("Consulta com ID {ConsultaId} não encontrada.", consultaId);
                    return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada." };
                }

                _logger.LogInformation("Consulta com ID {ConsultaId} encontrada com sucesso.", consultaId);
                return new ServiceResult { Succeeded = true, Data = consulta };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao buscar a consulta com ID {ConsultaId}.", consultaId);
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro inesperado ao buscar a consulta." };
            }
        }

        /// <summary>
        /// Busca uma consulta e seus detalhes pelo seu id, tratando possíveis falhas.
        /// </summary>
        /// <param name="consultaId">O ID da consulta a ser buscada.</param>
        /// <returns>Um ServiceResult contendo a consulta e seus detalhes em caso de sucesso ou uma mensagem de erro em caso de falha.</returns>
        public async Task<ServiceResult> ObterPorIdComDetalhesAsync(int consultaId)
        {
            _logger.LogInformation("Buscando consulta com detalhes por ID: {ConsultaId}", consultaId);
            try
            {
                var consulta = await _consultaRepository.GetByIdWithDetailsAsync(consultaId);

                if (consulta == null)
                {
                    _logger.LogWarning("Consulta com detalhes com ID {ConsultaId} não encontrada.", consultaId);
                    return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada." };
                }

                _logger.LogInformation("Consulta com detalhes com ID {ConsultaId} encontrada com sucesso.", consultaId);
                return new ServiceResult { Succeeded = true, Data = consulta };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao buscar a consulta com detalhes com ID {ConsultaId}.", consultaId);
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro inesperado ao buscar a consulta." };
            }
        }

        /// <summary>
        /// Busca todas as consultas de uma clínica.
        /// </summary>
        public async Task<IEnumerable<GetConsultaDto>> ObterTodasPorClinicaIdAsync(int clinicaId)
        {
            var consultas = await _consultaRepository.GetAllByClinicIdAsync(clinicaId);
            return _mapper.Map<IEnumerable<GetConsultaDto>>(consultas);
        }

        /// <summary>
        /// Busca os detalhes de uma consulta específica.
        /// </summary>
        public async Task<DetalhesConsultaDto?> ObterDetalhesPorIdEClinicaIdAsync(int consultaId, int clinicaId)
        {
            var consulta = await _consultaRepository.GetByIdAndClinicIdAsync(consultaId, clinicaId);
            return _mapper.Map<DetalhesConsultaDto>(consulta);
        }

        public async Task<ServiceResult> CreateAsync(UpsertConsultaDto dto, int clinicaId, int usuarioId)
        {
            // Validações de Negócio
            if (await _animalRepository.GetByIdAndClinicaIdWithIncludesAsync(dto.AnimalId, clinicaId) == null)
                return new ServiceResult { Succeeded = false, Message = "Animal não encontrado ou não pertence a esta clínica." };
            if (await _tutorRepository.GetByIdAndClinicIdWithIncludesAsync(dto.TutorId, clinicaId) == null)
                return new ServiceResult { Succeeded = false, Message = "Tutor não encontrado ou não pertence a esta clínica." };

            var consulta = _mapper.Map<Consulta>(dto);
            consulta.ClinicaId = clinicaId;
            consulta.UsuarioId = usuarioId;
            consulta.TutorIsCadastrado = true; // Já que validamos o ID

            await _consultaRepository.AddAsync(consulta);
            await _consultaRepository.SaveChangesAsync();

            var resultDto = _mapper.Map<DetalhesConsultaDto>(consulta);
            return new ServiceResult { Succeeded = true, Message = "Consulta agendada com sucesso.", Data = resultDto };
        }

        public async Task<ServiceResult> UpdateAsync(int consultaId, UpsertConsultaDto dto, int clinicaId)
        {
            var consulta = await _consultaRepository.GetByIdAndClinicIdAsync(consultaId, clinicaId);
            if (consulta == null)
                return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada." };

            // Mapeia apenas os campos do DTO para a entidade existente
            _mapper.Map(dto, consulta);

            await _consultaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Consulta atualizada com sucesso." };
        }

        public async Task<ServiceResult> DeleteAsync(int consultaId, int clinicaId)
        {
            var consulta = await _consultaRepository.GetByIdAndClinicIdAsync(consultaId, clinicaId);
            if (consulta == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada." };
            }

            // Regra de Negócio Opcional: Impedir exclusão se já houver um relatório.
            if (consulta.Relatorio != null)
            {
                return new ServiceResult { Succeeded = false, Message = "Não é possível excluir uma consulta que já possui um relatório preenchido." };
            }

            _consultaRepository.Delete(consulta);
            await _consultaRepository.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = "Consulta excluída com sucesso." };
        }
    }
}