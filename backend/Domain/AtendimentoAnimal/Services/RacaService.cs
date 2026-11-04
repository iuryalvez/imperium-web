using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Utils;
using backend.Domain.Utils.Paginacao;

namespace backend.Domain.AtendimentoAnimal.Services
{
    /// <summary>
    /// Regras de negócio de 'Raça'.
    /// Raças do sistema (ClinicaId nulo) são compartilhadas: a clínica pode usá-las, ocultá-las só para si
    /// ou editá-las (o que cria uma cópia própria). Só o admin altera, ativa/desativa ou exclui raças do sistema.
    /// Uma clínica nunca enxerga raças de outra clínica.
    /// </summary>
    public class RacaService : IRacaService
    {
        private const string MsgNaoEncontrada = "Raça não encontrada.";
        private const string MsgEmUso = "Esta raça está em uso por animais e não pode ser excluída. Desative-a.";

        private readonly IRacaRepository _racaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RacaService> _logger;

        public RacaService(IRacaRepository racaRepository, IMapper mapper, ILogger<RacaService> logger)
        {
            _racaRepository = racaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #region Clínica

        /// <summary>Lista paginada das raças disponíveis para a clínica, marcando as do sistema que ela ocultou.</summary>
        public async Task<PaginaResultado<GetRacaDto>> ListarDaClinicaAsync(RacaFiltro filtro, int clinicaId)
        {
            var pagina = await _racaRepository.ListarDaClinicaAsync(filtro, clinicaId);
            var ocultas = await _racaRepository.GetIdsOcultasAsync(clinicaId, pagina.Itens.Select(r => r.Id));

            return pagina.Converter(itens => itens.Select(r =>
            {
                var dto = _mapper.Map<GetRacaDto>(r);
                dto.IsOculta = ocultas.Contains(r.Id);
                return dto;
            }));
        }

        public async Task<List<OpcaoDto>> ListarOpcoesDaClinicaAsync(int clinicaId, string? busca, EspecieAnimal? especie)
        {
            return await _racaRepository.ListarOpcoesDaClinicaAsync(clinicaId, busca, especie);
        }

        public async Task<ServiceResult> ObterDaClinicaAsync(int id, int clinicaId)
        {
            var raca = await _racaRepository.GetDisponivelParaClinicaAsync(id, clinicaId);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);

            var dto = _mapper.Map<GetRacaDto>(raca);
            dto.IsOculta = (await _racaRepository.GetOcultaAsync(id, clinicaId)) != null;
            return new ServiceResult { Succeeded = true, Data = dto };
        }

        public async Task<ServiceResult> CadastrarDaClinicaAsync(CadastrarRacaDto dto, int clinicaId)
        {
            _logger.LogInformation("Criando a raça '{NomeRaca}' para a clínica {ClinicaId}.", dto.Nome, clinicaId);
            return await CadastrarAsync(dto, clinicaId);
        }

        /// <summary>
        /// Raça da própria clínica: atualiza. Raça do sistema: cria uma cópia da clínica com as alterações.
        /// </summary>
        public async Task<ServiceResult> EditarDaClinicaAsync(UpdateRacaDto dto, int clinicaId)
        {
            var raca = await _racaRepository.GetDisponivelParaClinicaAsync(dto.Id, clinicaId);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);

            if (raca.ClinicaId == null)
            {
                _logger.LogInformation("A raça {RacaId} é do sistema. Criando cópia para a clínica {ClinicaId}.", raca.Id, clinicaId);
                // A cópia parte dos dados da raça do sistema; o DTO só sobrescreve o que veio preenchido
                var copia = new Raca
                {
                    Nome = raca.Nome,
                    Especie = raca.Especie,
                    Porte = raca.Porte,
                    Referencia = raca.Referencia,
                    CategoriaMetabolica = raca.CategoriaMetabolica,
                    PesoMinimoReferenciaFemea = raca.PesoMinimoReferenciaFemea,
                    PesoMaximoReferenciaFemea = raca.PesoMaximoReferenciaFemea,
                    PesoMinimoReferenciaMacho = raca.PesoMinimoReferenciaMacho,
                    PesoMaximoReferenciaMacho = raca.PesoMaximoReferenciaMacho,
                };
                _mapper.Map(dto, copia);
                copia.Id = 0; // Garante que será tratada como uma nova entidade pelo ORM.
                copia.ClinicaId = clinicaId;
                copia.IsAtivo = true;
                copia.DataCadastro = DateTime.UtcNow;

                await _racaRepository.AddAsync(copia);
                await _racaRepository.SaveChangesAsync();
                return new ServiceResult
                {
                    Succeeded = true,
                    Message = "Raça do sistema copiada para a clínica com as alterações.",
                    Data = _mapper.Map<GetRacaDto>(copia)
                };
            }

            _mapper.Map(dto, raca);
            await _racaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Raça atualizada com sucesso.", Data = _mapper.Map<GetRacaDto>(raca) };
        }

        public async Task<ServiceResult> ExcluirDaClinicaAsync(int id, int clinicaId)
        {
            var raca = await _racaRepository.GetDisponivelParaClinicaAsync(id, clinicaId);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);
            if (raca.ClinicaId == null)
            {
                _logger.LogWarning("A clínica {ClinicaId} tentou excluir a raça do sistema {RacaId}.", clinicaId, id);
                return Falha("Raças do sistema não podem ser excluídas pela clínica. Você pode ocultá-la.");
            }

            return await ExcluirEntidadeAsync(raca);
        }

        public async Task<ServiceResult> SetStatusDaClinicaAsync(int id, int clinicaId, bool ativo)
        {
            var raca = await _racaRepository.GetDisponivelParaClinicaAsync(id, clinicaId);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);
            if (raca.ClinicaId == null)
                return Falha("Raças do sistema são ativadas/desativadas pelo administrador. Você pode ocultá-la para esta clínica.");

            return await AplicarStatusAsync(raca, ativo);
        }

        /// <summary>Oculta (ou volta a mostrar) uma raça do sistema só para a clínica.</summary>
        public async Task<ServiceResult> SetOcultaAsync(int id, int clinicaId, bool oculta)
        {
            var raca = await _racaRepository.GetDisponivelParaClinicaAsync(id, clinicaId);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);
            if (raca.ClinicaId != null)
                return Falha("Só raças do sistema podem ser ocultadas. Para raças da clínica, use desativar.");

            var registro = await _racaRepository.GetOcultaAsync(id, clinicaId);
            if (oculta && registro == null)
            {
                await _racaRepository.AddOcultaAsync(new RacaOcultaClinica { RacaId = id, ClinicaId = clinicaId, DataOcultacao = DateTime.UtcNow });
            }
            else if (!oculta && registro != null)
            {
                _racaRepository.RemoveOculta(registro);
            }

            await _racaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = oculta ? "Raça ocultada para esta clínica." : "Raça voltou a aparecer para esta clínica." };
        }

        /// <summary>
        /// Confere se a raça pode ser escolhida pela clínica em um cadastro novo: disponível, ativa e não oculta.
        /// </summary>
        public async Task<ServiceResult> ValidarSelecionavelAsync(int racaId, int clinicaId)
        {
            var raca = await _racaRepository.GetDisponivelParaClinicaAsync(racaId, clinicaId);
            if (raca == null) return Falha("A raça selecionada não existe ou não pertence a esta clínica.");
            if (!raca.IsAtivo) return Falha("A raça selecionada está inativa.");
            if (raca.ClinicaId == null && await _racaRepository.GetOcultaAsync(racaId, clinicaId) != null)
                return Falha("A raça selecionada está oculta para esta clínica.");

            return new ServiceResult { Succeeded = true };
        }

        /// <summary>
        /// Confere se a raça é da espécie do animal. Não olha ativa/oculta: serve também para a edição,
        /// em que manter a raça atual (mesmo inativa) é permitido.
        /// </summary>
        public async Task<ServiceResult> ValidarEspecieAsync(int racaId, int clinicaId, EspecieAnimal especie)
        {
            var raca = await _racaRepository.GetDisponivelParaClinicaAsync(racaId, clinicaId);
            if (raca == null) return Falha("A raça selecionada não existe ou não pertence a esta clínica.");
            if (raca.Especie != especie) return Falha("A raça escolhida não pertence à espécie do animal.");
            return new ServiceResult { Succeeded = true };
        }

        #endregion

        #region Sistema (admin)

        public async Task<PaginaResultado<GetRacaDto>> ListarSistemaAsync(RacaFiltro filtro)
        {
            var pagina = await _racaRepository.ListarSistemaAsync(filtro);
            return pagina.Converter(itens => _mapper.Map<IEnumerable<GetRacaDto>>(itens));
        }

        public async Task<PaginaResultado<GetRacaDto>> ListarCopiasAsync(RacaFiltro filtro)
        {
            var pagina = await _racaRepository.ListarCopiasAsync(filtro);
            return pagina.Converter(itens => _mapper.Map<IEnumerable<GetRacaDto>>(itens));
        }

        public async Task<ServiceResult> ObterAsync(int id)
        {
            var raca = await _racaRepository.GetByIdAsync(id);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);
            return new ServiceResult { Succeeded = true, Data = _mapper.Map<GetRacaDto>(raca) };
        }

        public async Task<ServiceResult> CadastrarSistemaAsync(CadastrarRacaDto dto)
        {
            _logger.LogInformation("Admin criando a raça do sistema '{NomeRaca}'.", dto.Nome);
            return await CadastrarAsync(dto, null);
        }

        /// <summary>Admin edita qualquer raça (do sistema ou cópia de clínica) diretamente, sem criar cópia.</summary>
        public async Task<ServiceResult> EditarAsync(UpdateRacaDto dto)
        {
            var raca = await _racaRepository.GetByIdAsync(dto.Id);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);

            _mapper.Map(dto, raca);
            await _racaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Raça atualizada com sucesso.", Data = _mapper.Map<GetRacaDto>(raca) };
        }

        public async Task<ServiceResult> ExcluirAsync(int id)
        {
            var raca = await _racaRepository.GetByIdAsync(id);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);
            return await ExcluirEntidadeAsync(raca);
        }

        public async Task<ServiceResult> SetStatusAsync(int id, bool ativo)
        {
            var raca = await _racaRepository.GetByIdAsync(id);
            if (raca == null) return ServiceResult.NaoEncontrado(MsgNaoEncontrada);
            return await AplicarStatusAsync(raca, ativo);
        }

        #endregion

        #region Auxiliares

        private async Task<ServiceResult> CadastrarAsync(CadastrarRacaDto dto, int? clinicaId)
        {
            var raca = _mapper.Map<Raca>(dto);
            raca.ClinicaId = clinicaId;
            raca.IsAtivo = true;
            raca.DataCadastro = DateTime.UtcNow;

            await _racaRepository.AddAsync(raca);
            await _racaRepository.SaveChangesAsync();

            _logger.LogInformation("Raça com ID {RacaId} criada com sucesso.", raca.Id);
            return new ServiceResult { Succeeded = true, Message = "Raça cadastrada com sucesso.", Data = _mapper.Map<GetRacaDto>(raca) };
        }

        private async Task<ServiceResult> ExcluirEntidadeAsync(Raca raca)
        {
            if (await _racaRepository.EmUsoAsync(raca.Id)) return ServiceResult.Conflito(MsgEmUso);

            _racaRepository.Delete(raca);
            await _racaRepository.SaveChangesAsync();
            _logger.LogInformation("Raça com ID {RacaId} excluída com sucesso.", raca.Id);
            return new ServiceResult { Succeeded = true, Message = "Raça excluída com sucesso." };
        }

        private async Task<ServiceResult> AplicarStatusAsync(Raca raca, bool ativo)
        {
            raca.IsAtivo = ativo;
            raca.DataDesativacao = ativo ? null : DateTime.UtcNow;
            await _racaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = ativo ? "Raça ativada com sucesso." : "Raça desativada com sucesso." };
        }

        private static ServiceResult Falha(string mensagem) => new() { Succeeded = false, Message = mensagem };

        #endregion
    }
}
