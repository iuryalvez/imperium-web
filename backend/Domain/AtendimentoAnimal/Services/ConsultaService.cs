using backend.Domain.Utils.Paginacao;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;

namespace backend.Domain.AtendimentoAnimal.Services
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

                _logger.LogInformation(
                    "Consulta (ID: {ConsultaId}) encontrada. A propriedade 'Animal' está carregada? {IsAnimalLoaded}. AnimalId: {AnimalId}", 
                    consultaId, 
                    consulta.Animal != null,
                    consulta.Animal?.Id // Usando o operador '?' para evitar NullReferenceException
                );

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
        /// Lista paginada das consultas da clínica, mais recentes primeiro por padrão.
        /// </summary>
        public async Task<PaginaResultado<GetConsultaDto>> ListarAsync(ConsultaFiltro filtro, int clinicaId)
        {
            var pagina = await _consultaRepository.ListarDaClinicaAsync(filtro, clinicaId);
            return pagina.Converter(itens => _mapper.Map<IEnumerable<GetConsultaDto>>(itens));
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
            // Validações de Negócio: animal e tutor da clínica e ativos
            var validacao = await ValidarAnimalETutorAsync(dto, clinicaId, null);
            if (!validacao.Succeeded) return validacao;

            var consulta = _mapper.Map<Consulta>(dto);
            consulta.ClinicaId = clinicaId;
            consulta.UsuarioId = usuarioId;
            consulta.TutorIsCadastrado = dto.TutorId.HasValue;

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

            // Validações de Negócio: trocar animal/tutor exige que o novo esteja ativo
            var validacao = await ValidarAnimalETutorAsync(dto, clinicaId, consulta);
            if (!validacao.Succeeded) return validacao;

            // Mapeia apenas os campos do DTO para a entidade existente
            _mapper.Map(dto, consulta);

            await _consultaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Consulta atualizada com sucesso." };
        }

        /// <summary>
        /// Confere se animal e tutor pertencem à clínica e, quando forem novos na consulta, se estão ativos.
        /// </summary>
        private async Task<ServiceResult> ValidarAnimalETutorAsync(UpsertConsultaDto dto, int clinicaId, Consulta? consultaAtual)
        {
            var animal = await _animalRepository.GetByIdAndClinicaIdWithIncludesAsync(dto.AnimalId, clinicaId);
            if (animal == null)
                return new ServiceResult { Succeeded = false, Message = "Animal não encontrado ou não pertence a esta clínica." };
            if (!animal.IsAtivo && consultaAtual?.AnimalId != dto.AnimalId)
                return new ServiceResult { Succeeded = false, Message = "O animal selecionado está inativo." };

            if (dto.TutorId.HasValue)
            {
                var tutor = await _tutorRepository.GetByIdAndClinicIdWithIncludesAsync(dto.TutorId.Value, clinicaId);
                if (tutor == null)
                    return new ServiceResult { Succeeded = false, Message = "Tutor não encontrado ou não pertence a esta clínica." };
                if (!tutor.IsAtivo && consultaAtual?.TutorId != dto.TutorId)
                    return new ServiceResult { Succeeded = false, Message = "O tutor selecionado está inativo." };
            }

            return new ServiceResult { Succeeded = true };
        }

        public async Task<ServiceResult> DeleteAsync(int consultaId, int clinicaId)
        {
            var consulta = await _consultaRepository.GetByIdAndClinicIdAsync(consultaId, clinicaId);
            if (consulta == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada." };
            }

            // Regra de Negócio Opcional: Impedir exclusão se já houver um relatório.
            if (consulta.Avaliacao != null)
            {
                return new ServiceResult { Succeeded = false, Message = "Não é possível excluir uma consulta que já possui um relatório preenchido." };
            }

            _consultaRepository.Delete(consulta);
            await _consultaRepository.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = "Consulta excluída com sucesso." };
        }
    }
}