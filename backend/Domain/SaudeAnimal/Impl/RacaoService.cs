using AutoMapper;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;

namespace backend.Domain.SaudeAnimal.Impl
{
    public class RacaoService : IRacaoService
    {
        private readonly IRacaoRepository _racaoRepository;
        // Usaremos AutoMapper para simplificar o mapeamento
        private readonly IMapper _mapper;

        public RacaoService(IRacaoRepository racaoRepository, IMapper mapper)
        {
            _racaoRepository = racaoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetRacoesDto>> GetAllByClinicIdAsync(int clinicaId, EspecieAnimal? especie)
        {
            var racoes = await _racaoRepository.GetAllByClinicIdAsync(clinicaId, especie);
            return _mapper.Map<IEnumerable<GetRacoesDto>>(racoes);
        }

        public async Task<GetRacoesDto?> GetByIdAsync(int racaoId, int clinicaId)
        {
            var racao = await _racaoRepository.GetByIdAndClinicIdAsync(racaoId, clinicaId);
            if (racao == null) return null;
            // Mapeia a entidade única para o DTO
            return _mapper.Map<GetRacoesDto>(racao);
        }

        public async Task<ServiceResult> CreateAsync(CadastrarRacaoDto dto, int clinicaId)
        {
            var racao = _mapper.Map<Racao>(dto);
            racao.ClinicaId = clinicaId;

            // Lógica de Negócio: Cálculo da Matéria Seca
            racao.MateriaSeca = 100 - racao.Umidade;

            await _racaoRepository.AddAsync(racao);
            await _racaoRepository.SaveChangesAsync();

            var resultDto = _mapper.Map<GetRacoesDto>(racao);
            return new ServiceResult { Succeeded = true, Message = "Ração cadastrada com sucesso.", Data = resultDto };
        }

        public async Task<ServiceResult> UpdateAsync(AtualizarRacaoDto dto, int clinicaId)
        {
            var racao = await _racaoRepository.GetByIdAndClinicIdAsync(dto.Id, clinicaId);
            if (racao == null) return new ServiceResult { Succeeded = false, Message = "Ração não encontrada." };

            // Mapeamento parcial usando AutoMapper (ele ignora campos nulos do DTO)
            _mapper.Map(dto, racao);

            // Lógica de Negócio: Recalcula a Matéria Seca se a Umidade for alterada
            if (dto.Umidade.HasValue)
            {
                racao.MateriaSeca = 100 - racao.Umidade;
            }

            await _racaoRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Ração atualizada com sucesso." };
        }

        public async Task<ServiceResult> DeleteAsync(int racaoId, int clinicaId)
        {
            var racao = await _racaoRepository.GetByIdAndClinicIdAsync(racaoId, clinicaId);
            if (racao == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Ração não encontrada." };
            }

            // Lógica de negócio antes de deletar (se houver, ex: verificar se está em uso)
            // ...

            _racaoRepository.Delete(racao);
            await _racaoRepository.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = "Ração excluída com sucesso." };
        }
    }
}