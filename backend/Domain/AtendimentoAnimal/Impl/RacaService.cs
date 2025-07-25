using backend.Domain.AtendimentoAnimal.Entities;
using AutoMapper;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.AtendimentoAnimal.DTOs.Raca; // You might want to use a mapper like AutoMapper

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class RacaService : IRacaService
    {
        private readonly IRacaRepository _racaRepository;
        private readonly IMapper _mapper; // Injected mapper

        public RacaService(IRacaRepository racaRepository, IMapper mapper)
        {
            _racaRepository = racaRepository;
            _mapper = mapper;
        }

        public async Task<GetRacaDto> CreateAsync(CadastrarRacaoDto criarRacaDto)
        {
            var raca = _mapper.Map<Raca>(criarRacaDto);
            await _racaRepository.AddAsync(raca);
            await _racaRepository.SaveChangesAsync();
            return _mapper.Map<GetRacaDto>(raca);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var raca = await _racaRepository.GetByIdAsync(id);
            if (raca == null) return false;

            _racaRepository.Delete(raca);
            var result = await _racaRepository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<GetRacaDto?> GetByIdAsync(int id)
        {
            var raca = await _racaRepository.GetByIdAsync(id);
            return _mapper.Map<GetRacaDto>(raca);
        }

        public async Task<List<GetRacaDto>> ListAllAsync(EspecieAnimal? especie)
        {
            var racas = especie.HasValue
                ? await _racaRepository.ListByEspecieAsync(especie.Value)
                : await _racaRepository.ListAllAsync();
            
            return _mapper.Map<List<GetRacaDto>>(racas);
        }

        public async Task<bool> UpdateAsync(UpdateRacaDto atualizarRacaDto)
        {
            var raca = await _racaRepository.GetByIdAsync(atualizarRacaDto.Id);
            if (raca == null) return false;

            // Map the changes from DTO to the entity
            _mapper.Map(atualizarRacaDto, raca);
            
            _racaRepository.Update(raca);
            var result = await _racaRepository.SaveChangesAsync();
            return result > 0;
        }
    }
}