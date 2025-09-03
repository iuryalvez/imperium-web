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

        private void CalcularEnergiaMetabolizavel(Racao racao)
        {
            float pb = NormalizarValor(racao.ProteinaBruta, racao.IsPorcentagem);
            float eea = NormalizarValor(racao.ExtratoEtereo, racao.IsPorcentagem);
            float mm = NormalizarValor(racao.MateriaMineral, racao.IsPorcentagem);
            float fb = NormalizarValor(racao.FibraBruta, racao.IsPorcentagem);
            float umidade = NormalizarValor(racao.Umidade, racao.IsPorcentagem);

            // 1. Converter % em fração
            umidade = racao.Umidade / 100f;
            pb = racao.ProteinaBruta / 100f;
            eea = racao.ExtratoEtereo / 100f;
            fb = racao.FibraBruta / 100f;
            mm = racao.MateriaMineral / 100f;

            // 2. Calcular matéria seca (MS)
            float ms = 1 - umidade;
            racao.MateriaSeca = ms * 100; // armazenar em %

            // 3. Calcular ENN
            float enn = 1 - (umidade + pb + eea + fb + mm);
            racao.ENN = enn * 100; // armazenar em %

            // 4. Calcular EB
            // EB = (5,7 x g PB) + (9,4 x g EEA) + [4,1 x (g ENN + g FB)]
            float eb = (5.7f * pb) + (9.4f * eea) + (4.1f * (enn + fb));
            racao.EnergiaBruta = eb; // kcal/g

            // 5. Calcular CDE (depende da espécie)
            float cde;
            switch (racao.Especie)
            {
                case EspecieAnimal.Cao:
                    // Fórmula do cão: 91,2 – (1,43 x %FB na MS)
                    float fbMsCao = fb / ms * 100f;
                    cde = 91.2f - (1.43f * fbMsCao);
                    break;
                case EspecieAnimal.Gato:
                    // Fórmula do gato: 87,9 – (0,88 x %FB na MS)
                    float fbMsGato = fb / ms * 100f;
                    cde = 87.9f - (0.88f * fbMsGato);
                    break;
                default:
                    throw new InvalidOperationException("Espécie não suportada para cálculo de EM.");
            }
            racao.CoeficienteDigestibilidadeEnergia = cde;

            // 6. Calcular ED
            float ed = eb * (cde / 100f);
            racao.EnergiaDigestivel = ed;

            // 7. Calcular EM
            float em;
            if (racao.Especie == EspecieAnimal.Cao)
                em = ed - (1.04f * pb);
            else
                em = ed - (0.77f * pb);

            racao.EnergiaMetabolizavel = em; // kcal/g
        }

        private float NormalizarValor(float valor, bool isPorcentagem)
        {
            // Se o valor estiver em g/kg, converte para porcentagem
            if (!isPorcentagem)
            {
                // g/kg -> %
                return valor / 10f;
            }
            return valor;
        }
    }
}