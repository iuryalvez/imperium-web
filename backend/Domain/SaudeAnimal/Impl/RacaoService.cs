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
            Console.WriteLine($"[DEBUG] Iniciando cálculo de EM para ração {racao.Nome}, espécie: {racao.Especie}");

            float pb = racao.ProteinaBruta;
            float eea = racao.ExtratoEtereo;
            float mm = racao.MateriaMineral;
            float fb = racao.FibraBruta;
            float umidade = racao.Umidade;

            if (!racao.IsPorcentagem)
            {
                pb /= 10f;
                eea /= 10f;
                mm /= 10f;
                fb /= 10f;
                umidade /= 10f;
            }

            Console.WriteLine($"[DEBUG] Valores normalizados (%): PB={pb}, EEA={eea}, MM={mm}, FB={fb}, Umidade={umidade}");

            // 1. Converter % em fração
            umidade /= 100f;
            pb /= 100f;
            eea /= 100f;
            fb /= 100f;
            mm /= 100f;

            Console.WriteLine($"[DEBUG] Valores em fração: PB={pb:F4}, EEA={eea:F4}, MM={mm:F4}, FB={fb:F4}, Umidade={umidade:F4}");

            // 2. Calcular matéria seca (MS)
            float ms = 1 - umidade;
            racao.MateriaSeca = ms * 100; // armazenar em %
            Console.WriteLine($"[DEBUG] Matéria seca (MS) = {racao.MateriaSeca:F2}%");

            // 3. Calcular ENN
            float enn = 1 - (umidade + pb + eea + fb + mm);
            racao.ExtrativoNaoNitrogenado = enn * 100; // armazenar em %
            Console.WriteLine($"[DEBUG] ENN = {racao.ExtrativoNaoNitrogenado:F2}%");

            // 4. Calcular EB
            float eb = (5.7f * pb) + (9.4f * eea) + (4.1f * (enn + fb));
            racao.EnergiaBruta = eb; // kcal/g
            Console.WriteLine($"[DEBUG] Energia Bruta (EB) = {eb:F4} kcal/g");

            // 5. Calcular CDE
            float cde;
            switch (racao.Especie)
            {
                case EspecieAnimal.Cao:
                    float fbMsCao = fb / ms * 100f;
                    cde = 91.2f - (1.43f * fbMsCao);
                    Console.WriteLine($"[DEBUG] CDE (cão): FB na MS={fbMsCao:F2}%, CDE={cde:F2}%");
                    break;
                case EspecieAnimal.Gato:
                    float fbMsGato = fb / ms * 100f;
                    cde = 87.9f - (0.88f * fbMsGato);
                    Console.WriteLine($"[DEBUG] CDE (gato): FB na MS={fbMsGato:F2}%, CDE={cde:F2}%");
                    break;
                default:
                    throw new InvalidOperationException("Espécie não suportada para cálculo de EM.");
            }
            racao.CoeficienteDigestibilidadeEnergia = cde;

            // 6. Calcular ED
            float ed = eb * (cde / 100f);
            racao.EnergiaDigestivel = ed;
            Console.WriteLine($"[DEBUG] Energia Digestível (ED) = {ed:F4} kcal/g");

            // 7. Calcular EM
            float em;
            if (racao.Especie == EspecieAnimal.Cao)
                em = ed - (1.04f * pb);
            else
                em = ed - (0.77f * pb);

            racao.EnergiaMetabolizavel = em; // kcal/g
            Console.WriteLine($"[DEBUG] Energia Metabolizável (EM) = {em:F4} kcal/g");
            Console.WriteLine($"[DEBUG] Cálculo finalizado para ração {racao.Nome}");

            // O NRC (2006) não recomenda o uso destas equações para estimar a EM de
            // alimentos com elevada digestibilidade, como substitutos do leite e dietas líquidas
            // para nutrição enteral. Tais equações também podem não ser apropriadas para
            // estimar a EM de alimentos para cães com teor de fibra bruta superior a 8%, com
            // base na matéria seca.
        }

        private void CalcularDistribuicaoEnergetica(Racao racao)
        {
            if (!racao.ExtrativoNaoNitrogenado.HasValue)
                throw new InvalidOperationException("Não é possível calcular a distribuição energética sem o ENN calculado.");

            float pb = racao.ProteinaBruta;
            float eea = racao.ExtratoEtereo;
            float enn = racao.ExtrativoNaoNitrogenado.Value;

            // Normaliza para %
            if (!racao.IsPorcentagem)
            {
                pb = pb / 10f;
                eea = eea / 10f;
                enn = enn / 10f;
            }

            // Calcula em kcal/100g (modificado de Atwater)
            float energiaPB = pb * 3.5f;
            float energiaEEA = eea * 8.5f;
            float energiaENN = enn * 3.5f;

            float energiaTotal = energiaPB + energiaEEA + energiaENN;

            // energiaPB *= 100f / energiaTotal; 
            // energiaEEA *= 100f / energiaTotal; 
            // energiaENN *= 100f / energiaTotal; 

            racao.EnergiaProteina = energiaPB;
            racao.EnergiaExtratoEtereo = energiaEEA;
            racao.EnergiaExtrativoNaoNitrogenado = energiaENN;
            racao.EnergiaTotalEstimativa = energiaTotal;

            Console.WriteLine($"[DEBUG] Distribuição energética: PB={energiaPB:F2}kcal, EEA={energiaEEA:F2}kcal, ENN={energiaENN:F2}kcal, Total={energiaTotal:F2}kcal/100g");
        }
    }
}