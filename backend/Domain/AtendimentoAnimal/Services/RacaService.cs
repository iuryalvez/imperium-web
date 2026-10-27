using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Services
{


    /// <summary>
    /// Serviço responsável por manipular a lógica de negócio relacionada à entidade 'Raça'.
    /// </summary>
    public class RacaService : IRacaService
    {
        private readonly IRacaRepository _racaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RacaService> _logger;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="RacaService"/>.
        /// </summary>
        /// <param name="racaRepository">O repositório para acesso aos dados das entidades 'Raça'.</param>
        /// <param name="mapper">A instância do AutoMapper para mapeamento de objetos.</param>
        /// <param name="logger">O logger para registrar as operações do serviço.</param>
        public RacaService(IRacaRepository racaRepository, IMapper mapper, ILogger<RacaService> logger)
        {
            _racaRepository = racaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova raça específica para uma clínica.
        /// </summary>
        /// <param name="criarRacaDto">O DTO contendo os dados para a nova raça.</param>
        /// <param name="clinicaId">O ID da clínica à qual a raça será associada.</param>
        /// <returns>Um DTO da raça recém-criada.</returns>
        public async Task<GetRacaDto> CreateAsync(CadastrarRacaDto criarRacaDto, int clinicaId)
        {
            _logger.LogInformation("Tentando criar uma nova raça '{NomeRaca}' para a clínica {ClinicaId}.", criarRacaDto.Nome, clinicaId);

            try
            {
                var raca = _mapper.Map<Raca>(criarRacaDto);
                raca.ClinicaId = clinicaId;

                await _racaRepository.AddAsync(raca);
                await _racaRepository.SaveChangesAsync();

                _logger.LogInformation("Raça com ID {RacaId} criada com sucesso.", raca.Id);
                return _mapper.Map<GetRacaDto>(raca);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao criar a raça '{NomeRaca}' para a clínica {ClinicaId}.", criarRacaDto.Nome, clinicaId);
                throw; // Relança a exceção para ser tratada por um handler de nível superior.
            }
        }

        /// <summary>
        /// Exclui uma raça pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da raça a ser excluída.</param>
        /// <returns>Verdadeiro se a exclusão foi bem-sucedida, caso contrário, falso.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Tentando excluir a raça com ID {RacaId}.", id);

            try
            {
                var raca = await _racaRepository.GetByIdAsync(id);
                if (raca == null)
                {
                    _logger.LogWarning("Raça com ID {RacaId} não encontrada para exclusão.", id);
                    return false;
                }

                _racaRepository.Delete(raca);
                var result = await _racaRepository.SaveChangesAsync();

                bool success = result > 0;
                if (success)
                {
                    _logger.LogInformation("Raça com ID {RacaId} excluída com sucesso.", id);
                }
                else
                {
                    _logger.LogWarning("A exclusão para a raça de ID {RacaId} não afetou nenhum registro.", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao excluir a raça com ID {RacaId}.", id);
                throw;
            }
        }

        /// <summary>
        /// Obtém uma raça específica pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da raça.</param>
        /// <returns>Um DTO da raça, ou nulo se não for encontrada.</returns>
        public async Task<GetRacaDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Buscando raça com ID {RacaId}.", id);
            var raca = await _racaRepository.GetByIdAsync(id);

            if (raca == null)
            {
                _logger.LogWarning("Raça com ID {RacaId} não encontrada.", id);
                return null;
            }

            return _mapper.Map<GetRacaDto>(raca);
        }

        /// <summary>
        /// Lista todas as raças padrão do sistema, opcionalmente filtradas por espécie.
        /// </summary>
        /// <param name="especie">A espécie opcional para filtro.</param>
        /// <returns>Uma lista de DTOs de raças do sistema.</returns>
        public async Task<List<GetRacaDto>> GetAllSystemByEspecieAsync(EspecieAnimal? especie)
        {
            _logger.LogInformation("Buscando todas as raças do sistema. Filtro por espécie: {Especie}", especie?.ToString() ?? "Nenhum");

            var racas = await _racaRepository.GetAllSystemAsync();

            if (especie.HasValue)
            {
                racas = racas.Where(r => r.Especie == especie.Value).ToList();
            }

            return _mapper.Map<List<GetRacaDto>>(racas);
        }

        /// <summary>
        /// Lista todas as raças de uma clínica específica, opcionalmente filtradas por espécie.
        /// </summary>
        /// <param name="clinicaId">O ID da clínica.</param>
        /// <param name="especie">A espécie opcional para filtro.</param>
        /// <returns>A list of breed DTOs for the specified clinic.</returns>
        public async Task<List<GetRacaDto>> ListByClinicaIdAndEspecieAsync(int clinicaId, EspecieAnimal? especie)
        {
            _logger.LogInformation("Buscando raças para a clínica {ClinicaId}. Filtro por espécie: {Especie}", clinicaId, especie?.ToString() ?? "Nenhum");

            var racas = await _racaRepository.ListByClinicaIdAsync(clinicaId);

            if (especie.HasValue)
            {
                racas = racas.Where(r => r.Especie == especie.Value).ToList();
            }

            return _mapper.Map<List<GetRacaDto>>(racas);
        }

        /// <summary>
        /// Lista todas as raças disponíveis para a clínica (sistema + específicas), opcionalmente filtradas por espécie.
        /// </summary>
        public async Task<List<GetRacaDto>> ListAvailableForClinicaAndEspecieAsync(int clinicaId, EspecieAnimal? especie)
        {
            _logger.LogInformation("Buscando raças disponíveis (sistema + clínica) para a clínica {ClinicaId}. Filtro por espécie: {Especie}", clinicaId, especie?.ToString() ?? "Nenhum");

            var racas = await _racaRepository.ListAvailableForClinicaAsync(clinicaId);

            if (especie.HasValue)
            {
                racas = racas.Where(r => r.Especie == especie.Value).ToList();
            }

            return _mapper.Map<List<GetRacaDto>>(racas);
        }

        /// <summary>
        /// Atualiza uma raça existente. Se a raça for padrão do sistema (ClinicaId nulo), 
        /// cria uma nova cópia específica para a clínica com as alterações. 
        /// Se a raça já for da clínica, atualiza seus dados.
        /// </summary>
        /// <param name="atualizarRacaDto">O DTO com os dados atualizados da raça.</param>
        /// <param name="clinicaId">O ID da clínica que está realizando a operação.</param>
        /// <returns>Verdadeiro se a operação (atualização ou criação de cópia) foi bem-sucedida, caso contrário, falso.</returns>
        public async Task<bool> CreateOrUpdateAsync(UpdateRacaDto atualizarRacaDto, int clinicaId)
        {
            _logger.LogInformation("Tentando atualizar a raça com ID {RacaId} para a clínica {ClinicaId}.", atualizarRacaDto.Id, clinicaId);

            try
            {
                var raca = await _racaRepository.GetByIdAsync(atualizarRacaDto.Id);
                if (raca == null)
                {
                    _logger.LogWarning("Raça com ID {RacaId} não encontrada para atualização.", atualizarRacaDto.Id);
                    return false;
                }

                // Se a raça é do sistema (sem ID de clínica), cria uma nova cópia editável para a clínica.
                if (raca.ClinicaId == null)
                {
                    _logger.LogInformation("A raça {RacaId} é do sistema. Criando uma nova cópia específica para a clínica {ClinicaId}.", raca.Id, clinicaId);
                    return await CreateClinicSpecificCopyAsync(atualizarRacaDto, clinicaId);
                }

                // Se for uma raça da própria clínica, atualiza diretamente.
                _logger.LogInformation("A raça {RacaId} é específica da clínica. Atualizando diretamente.", raca.Id);
                _mapper.Map(atualizarRacaDto, raca);
                _racaRepository.Update(raca);
                var updated = await _racaRepository.SaveChangesAsync();
                return updated > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao atualizar a raça de ID {RacaId} para a clínica {ClinicaId}.", atualizarRacaDto.Id, clinicaId);
                throw;
            }
        }

        /// <summary>
        /// Cria uma nova raça específica para a clínica com base nos dados de uma raça do sistema.
        /// </summary>
        private async Task<bool> CreateClinicSpecificCopyAsync(UpdateRacaDto dto, int clinicaId)
        {
            var novaRaca = _mapper.Map<Raca>(dto);
            novaRaca.Id = 0; // Garante que será tratada como uma nova entidade pelo ORM.
            novaRaca.ClinicaId = clinicaId;

            await _racaRepository.AddAsync(novaRaca);
            var result = await _racaRepository.SaveChangesAsync();
            return result > 0;
        }
    }
}