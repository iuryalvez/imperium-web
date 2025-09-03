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

        public async Task<GetRacaDto> CreateAsync(CadastrarRacaDto criarRacaDto, int clinicaId)
        {
            var raca = _mapper.Map<Raca>(criarRacaDto);
            raca.ClinicaId = clinicaId;
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

        public async Task<List<GetRacaDto>> GetAllSystemByEspecieAsync(EspecieAnimal? especie)
        {
            // Primeiro, busca todas as raças dessa clínica
            var racas = await _racaRepository.GetAllSystemAsync();

            // Se tiver filtro de espécie, aplica agora
            if (especie.HasValue)
            {
                racas = racas.Where(r => r.Especie == especie.Value).ToList();
            }

            return _mapper.Map<List<GetRacaDto>>(racas);
        }

        public async Task<List<GetRacaDto>> ListByClinicaIdAndEspecieAsync(int clinicaId, EspecieAnimal? especie)
        {
            // Primeiro, busca todas as raças dessa clínica
            var racas = await _racaRepository.ListByClinicaIdAsync(clinicaId);

            // Se tiver filtro de espécie, aplica agora
            if (especie.HasValue)
            {
                racas = racas.Where(r => r.Especie == especie.Value).ToList();
            }

            return _mapper.Map<List<GetRacaDto>>(racas);
        }

        public async Task<bool> UpdateAsync(UpdateRacaDto atualizarRacaDto, int clinicaId)
        {
            var raca = await _racaRepository.GetByIdAsync(atualizarRacaDto.Id);
            if (raca == null) return false;

            // 🔹 Caso seja uma raça do sistema, criamos uma nova para a clínica
            if (raca.ClinicaId == null)
            {
                var novaRaca = new Raca
                {
                    Nome = atualizarRacaDto.Nome,
                    Especie = atualizarRacaDto.Especie,
                    PesoMinimoReferenciaFemea = atualizarRacaDto.PesoMinimoReferenciaFemea,
                    PesoMaximoReferenciaFemea = atualizarRacaDto.PesoMaximoReferenciaFemea,
                    PesoMinimoReferenciaMacho = atualizarRacaDto.PesoMinimoReferenciaMacho,
                    PesoMaximoReferenciaMacho = atualizarRacaDto.PesoMaximoReferenciaMacho,
                    ClinicaId = clinicaId
                };

                await _racaRepository.AddAsync(novaRaca);
                var result = await _racaRepository.SaveChangesAsync();
                return result > 0;
            }

            // 🔹 Caso já seja uma raça da clínica, atualiza normalmente
            _mapper.Map(atualizarRacaDto, raca);
            _racaRepository.Update(raca);
            var updated = await _racaRepository.SaveChangesAsync();
            return updated > 0;
        }
    }
}