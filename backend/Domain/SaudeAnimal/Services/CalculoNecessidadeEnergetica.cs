using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Enums;
using System.Text.Json;

namespace backend.Domain.SaudeAnimal.Services
{
    public interface ICalculoNecessidadeEnergetica
    {
        double CalcularGestacao(Animal animal, Dieta dieta);
        double CalcularLactacao(Animal animal, Dieta dieta);
        double CalcularManutencao(Animal animal, Dieta dieta);
        double CalcularNecessidadeTotal(Animal animal, Dieta dieta);
    }

    public class CalculoNecessidadeEnergetica : ICalculoNecessidadeEnergetica
    {
        private readonly ILogger<CalculoNecessidadeEnergetica> _logger;

        public CalculoNecessidadeEnergetica(ILogger<CalculoNecessidadeEnergetica> logger)
        {
            _logger = logger;
        }

        public double CalcularNecessidadeTotal(Animal animal, Dieta dieta)
        {
            _logger.LogInformation(
                "Iniciando cálculo de necessidade energética total para o animal ID: {AnimalId} com tipo de dieta: {TipoDieta}.",
                animal.Id, dieta.TipoDieta);

            try
            {
                double resultado = dieta.TipoDieta switch
                {
                    TipoDieta.Manutencao => CalcularManutencao(animal, dieta),
                    TipoDieta.Crescimento => CalcularCrescimento(animal, dieta) ?? 0,
                    TipoDieta.Lactacao => CalcularLactacao(animal, dieta),
                    TipoDieta.Gestacao => CalcularGestacao(animal, dieta),
                    _ => throw new InvalidOperationException($"Tipo de dieta '{dieta.TipoDieta}' não implementado.")
                };

                var resultadoArredondado = Math.Round(resultado, 3);

                _logger.LogInformation(
                    "Cálculo de necessidade total concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);

                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular a necessidade energética total para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        public double CalcularManutencao(Animal animal, Dieta dieta)
        {
            _logger.LogInformation("Iniciando cálculo de manutenção para o animal ID: {AnimalId}.", animal.Id);

            try
            {
                if (!animal.Peso.HasValue)
                {
                    throw new ArgumentException("O peso do animal não pode ser nulo para o cálculo de manutenção.");
                }

                var perfil = PerfilMetabolicoResolver.Resolver(animal, dieta);
                
                double multiplicador = 0;
                double potencia = 0;
                string descricao = $"Manutenção ({animal.Especie} - Customizado)";

                dieta.IsCustomizado = false;

                if (!dieta.MultiplicadorEnergetico.HasValue || !dieta.Potencia.HasValue)
                {
                    throw new InvalidOperationException("O Multiplicador (K) e a Potência (P) são obrigatórios para o cálculo.");
                }

                multiplicador = dieta.MultiplicadorEnergetico.Value;
                potencia = dieta.Potencia.Value;

                ValidarCustomizacaoPerfil(dieta, perfil, multiplicador, potencia, ref descricao);

                _logger.LogInformation("Perfil metabólico resolvido: {Perfil}. Multiplicador final: {Multiplicador}, Potência padrão: {Potencia}.",
                    perfil, multiplicador, potencia);

                // Injeta transparência para Manutenção
                dieta.FatorK = multiplicador;
                dieta.Expoente = potencia;
                dieta.PerfilNutricional = descricao;

                var resultado = multiplicador * Math.Pow(animal.Peso.Value, potencia);
                var resultadoArredondado = Math.Round(resultado, 3);

                _logger.LogInformation(
                    "Cálculo de manutenção concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);

                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de manutenção para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        private double? CalcularCrescimento(Animal animal, Dieta dieta)
        {
            _logger.LogInformation("Iniciando cálculo para crescimento para o animal ID: {AnimalId}.", animal.Id);

            try
            {
                if (animal.Peso == null || animal.Peso <= 0)
                {
                    _logger.LogWarning("Peso do animal ID: {AnimalId} é nulo ou inválido para cálculo de crescimento.", animal.Id);
                    return null;
                }

                double? pesoMedio = animal.Sexo == Sexo.Macho ? animal.Raca?.PesoMedioMacho : animal.Raca?.PesoMedioFemea;
                double PCa = animal.Peso.Value;
                double PCm;

                // Priority: 1. dieta.PesoObjetivo, 2. animal.PesoIdeal, 3. pesoMedio, 4. animal.PesoMaximo
                if (dieta.PesoObjetivo.HasValue && dieta.PesoObjetivo > 0)
                {
                    PCm = dieta.PesoObjetivo.Value;
                }
                else if (animal.PesoIdeal.HasValue && animal.PesoIdeal > 0)
                {
                    PCm = animal.PesoIdeal.Value;
                }
                else if (pesoMedio.HasValue && pesoMedio > 0)
                {
                    PCm = pesoMedio.Value;
                }
                else if (animal.PesoMaximo.HasValue && animal.PesoMaximo > 0)
                {
                    PCm = animal.PesoMaximo.Value;
                }
                else
                {
                    throw new InvalidOperationException("Peso adulto esperado (PCm) não pôde ser determinado para o cálculo de crescimento.");
                }

                _logger.LogInformation("Cálculo de crescimento para animal ID: {AnimalId}. Peso Atual (PCa): {PCa}, Peso Adulto (PCm): {PCm}.", animal.Id, PCa, PCm);

                double p = PCa / PCm;
                double necessidadeKcal;

                var perfil = PerfilMetabolicoResolver.Resolver(animal, dieta);
                
                double multiplicador = 0;
                double potencia = 0;
                string descricao = $"Crescimento ({animal.Especie} - Customizado)";

                dieta.IsCustomizado = false;

                if (!dieta.MultiplicadorEnergetico.HasValue || !dieta.Potencia.HasValue)
                {
                    throw new InvalidOperationException("O Multiplicador (K) e a Potência (P) são obrigatórios para o cálculo.");
                }

                multiplicador = dieta.MultiplicadorEnergetico.Value;
                potencia = dieta.Potencia.Value;

                ValidarCustomizacaoPerfil(dieta, perfil, multiplicador, potencia, ref descricao);

                switch (animal.Especie)
                {
                    case EspecieAnimal.Gato:
                        var pesoMetabolicoGato = Math.Pow(PCa, potencia);
                        necessidadeKcal = multiplicador * pesoMetabolicoGato * 6.7 * (Math.Exp(-0.189 * p) - 0.66);
                        break;
                    case EspecieAnimal.Cao:
                        var pesoMetabolicoCao = Math.Pow(PCa, potencia);
                        necessidadeKcal = multiplicador * pesoMetabolicoCao * 3.2 * (Math.Exp(-0.87 * p) - 0.1);
                        break;
                    default:
                        throw new InvalidOperationException($"Espécie '{animal.Especie}' não suportada para cálculo de crescimento.");
                }

                // Injeta transparência para Crescimento
                dieta.FatorK = multiplicador;
                dieta.Expoente = potencia;
                dieta.PerfilNutricional = descricao;

                var resultadoFinal = Math.Max(necessidadeKcal, 0);
                var resultadoArredondado = Math.Round(resultadoFinal, 3);

                _logger.LogInformation(
                    "Cálculo de crescimento concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);

                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de crescimento para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        public double CalcularGestacao(Animal animal, Dieta dieta)
        {
            _logger.LogInformation("Iniciando cálculo de gestação para o animal ID: {AnimalId} da espécie: {Especie}.", animal.Id, animal.Especie);

            try
            {
                double necessidadeKcal;
                
                // Puxa o PCa (Peso Atual)
                double PCa = animal.Peso ?? 0;
                if (PCa <= 0)
                {
                    throw new InvalidOperationException("Peso atual não informado ou inválido para o cálculo de gestação.");
                }

                // Resolve o perfil (agora reaproveitando perfis de manutenção)
                var perfil = PerfilMetabolicoResolver.Resolver(animal, dieta);
                double multiplicador = 0;
                double potencia = 0;
                string descricao = $"Gestação ({animal.Especie} - Customizado)";

                dieta.IsCustomizado = false;

                if (!dieta.MultiplicadorEnergetico.HasValue || !dieta.Potencia.HasValue)
                {
                    throw new InvalidOperationException("O Multiplicador (K) e a Potência (P) são obrigatórios para o cálculo.");
                }

                multiplicador = dieta.MultiplicadorEnergetico.Value;
                potencia = dieta.Potencia.Value;

                ValidarCustomizacaoPerfil(dieta, perfil, multiplicador, potencia, ref descricao);

                if (animal.Especie == EspecieAnimal.Gato)
                {
                    // Gatas não possuem equação NRC específica (aviso será tratado no frontend).
                    // Calcula-se como manutenção com os multiplicadores selecionados.
                    necessidadeKcal = multiplicador * Math.Pow(PCa, potencia);
                }
                else if (animal.Especie == EspecieAnimal.Cao)
                {
                    necessidadeKcal = multiplicador * Math.Pow(PCa, potencia);

                    // Adiciona + 26 * Peso se a flag for verdadeira (terço final de gestação, 1 a 4 filhotes)
                    if (dieta.HasAdicionalGestacaoCao)
                    {
                        necessidadeKcal += 26.0 * PCa;
                        if (!descricao.Contains("+ 26 × PC"))
                        {
                            descricao += $" + 26 × PC";
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Espécie '{animal.Especie}' não suportada para cálculo de gestação.");
                }

                // Injeta transparência
                dieta.FatorK = multiplicador;
                dieta.Expoente = potencia;
                dieta.PerfilNutricional = descricao;

                var resultadoFinal = Math.Max(necessidadeKcal, 0);
                var resultadoArredondado = Math.Round(resultadoFinal, 3);
                _logger.LogInformation(
                    "Cálculo de gestação concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);
                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de gestação para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        public double CalcularLactacao(Animal animal, Dieta dieta)
        {
            int numeroFilhotes = dieta.NumeroFilhotes ?? throw new InvalidOperationException("Número de filhotes não informado na dieta para cálculo de lactação.");
            int semanaLactacao = dieta.SemanaLactacao ?? throw new InvalidOperationException("Semana de lactação não informada na dieta para cálculo de lactação.");

            _logger.LogInformation(
                "Iniciando cálculo de lactação para o animal ID: {AnimalId}, com {NumeroFilhotes} filhote(s) na {SemanaLactacao}ª semana.",
                animal.Id, numeroFilhotes, semanaLactacao);

            try
            {
                if (semanaLactacao < 1) throw new ArgumentException("Semana de lactação inválida.");
                var peso = animal.Peso ?? throw new InvalidOperationException("Peso não informado para cálculo de lactação.");

                double resultado = 0;
                
                if (animal.Especie == EspecieAnimal.Gato)
                {
                    resultado = CalcularLactacaoGato(animal, peso, numeroFilhotes, semanaLactacao, dieta);
                }
                else
                {
                    resultado = CalcularLactacaoCao(animal, peso, numeroFilhotes, semanaLactacao, dieta);
                }
                
                // Gerar previsões
                dieta.PrevisoesJson = GerarPrevisoesLactacao(animal, peso, numeroFilhotes, semanaLactacao, dieta);

                var resultadoArredondado = Math.Round(resultado, 3);
                _logger.LogInformation(
                    "Cálculo de lactação concluído para o animal ID: {AnimalId}. Resultado: {ResultadoKcal} kcal/dia.",
                    animal.Id, resultadoArredondado);
                return resultadoArredondado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cálculo de lactação para o animal ID: {AnimalId}.", animal.Id);
                throw;
            }
        }

        private double CalcularLactacaoGato(Animal animal, double peso, int numeroFilhotes, int semana, Dieta dieta)
        {
            _logger.LogInformation("Executando cálculo de lactação para Gato. Peso: {Peso}, Filhotes: {NumeroFilhotes}, Semana: {Semana}.", peso, numeroFilhotes, semana);

            var perfil = PerfilMetabolicoResolver.Resolver(animal, dieta);
            double multiplicador = 100;
            double potencia = 0.67;
            string descricao = "Gato em Lactação";

            dieta.IsCustomizado = false;

            if (!dieta.MultiplicadorEnergetico.HasValue || !dieta.Potencia.HasValue)
            {
                throw new InvalidOperationException("O Multiplicador (K) e a Potência (P) são obrigatórios para o cálculo.");
            }

            multiplicador = dieta.MultiplicadorEnergetico.Value;
            potencia = dieta.Potencia.Value;

            ValidarCustomizacaoPerfil(dieta, perfil, multiplicador, potencia, ref descricao);

            var pesoMetabolico = Math.Pow(peso, potencia);
            double manutencao = multiplicador * pesoMetabolico;

            double[] fatores = { 0.9, 0.9, 1.2, 1.2, 1.1, 1.0, 0.8 };
            double fatorL = fatores[Math.Clamp(semana - 1, 0, fatores.Length - 1)];

            double multiplicadorFilhotes = numeroFilhotes switch
            {
                < 3 => 18,
                3 or 4 => 60,
                > 4 => 70,
            };

            _logger.LogInformation("Valores para Gato: Manutenção: {Manutencao}, Fator Lactação: {FatorL}, Constante Filhotes: {Multiplicador}.",
                manutencao, fatorL, multiplicadorFilhotes);

            dieta.FatorK = multiplicador;
            dieta.Expoente = potencia;
            dieta.PerfilNutricional = descricao;

            return manutencao + (multiplicadorFilhotes * peso * fatorL);
        }

        private double CalcularLactacaoCao(Animal animal, double peso, int numeroFilhotes, int semana, Dieta dieta)
        {
            _logger.LogInformation("Executando cálculo de lactação para Cão. Peso: {Peso}, Filhotes: {NumeroFilhotes}, Semana: {Semana}.", peso, numeroFilhotes, semana);

            var perfil = PerfilMetabolicoResolver.Resolver(animal, dieta);

            if (!dieta.MultiplicadorEnergetico.HasValue || !dieta.Potencia.HasValue)
            {
                throw new InvalidOperationException("Para cães em lactação, o cálculo de manutenção deve ser obrigatoriamente personalizado pelo veterinário (informe K e P).");
            }

            double multiplicador = dieta.MultiplicadorEnergetico.Value;
            double potencia = dieta.Potencia.Value;
            
            var pesoMetabolico = Math.Pow(peso, potencia);
            double manutencao = multiplicador * pesoMetabolico;

            double[] fatores = { 0.75, 0.95, 1.1, 1.2 };
            double fatorL = fatores[Math.Clamp(semana - 1, 0, fatores.Length - 1)];

            int n = Math.Clamp(numeroFilhotes, 0, 4);
            int m = numeroFilhotes > 4 ? numeroFilhotes - 4 : 0;

            _logger.LogInformation("Valores para Cão: Manutenção: {Manutencao}, Fator Lactação: {FatorL}, n={n}, m={m}.",
                manutencao, fatorL, n, m);

            string descricao = "Cão em Lactação (Customizado)";
            
            ValidarCustomizacaoPerfil(dieta, perfil, multiplicador, potencia, ref descricao);

            dieta.FatorK = multiplicador;
            dieta.Expoente = potencia;
            dieta.PerfilNutricional = descricao;

            return manutencao + (peso * ((24 * n) + (12 * m)) * fatorL);
        }

        private string GerarPrevisoesLactacao(Animal animal, double peso, int numeroFilhotes, int semanaAtual, Dieta dieta)
        {
            var previsoes = new List<object>();
            
            if (animal.Especie == EspecieAnimal.Gato)
            {
                double multiplicador = dieta.FatorK;
                double potencia = dieta.Expoente;
                double manutencao = multiplicador * Math.Pow(peso, potencia);
                double[] fatores = { 0.9, 0.9, 1.2, 1.2, 1.1, 1.0, 0.8 };
                double constante = numeroFilhotes switch
                {
                    < 3 => 18,
                    3 or 4 => 60,
                    > 4 => 70,
                };

                for (int s = semanaAtual + 1; s <= 7; s++)
                {
                    double fatorL = fatores[Math.Clamp(s - 1, 0, fatores.Length - 1)];
                    double kcal = manutencao + (constante * peso * fatorL);
                    previsoes.Add(new { semana = s, kcal = Math.Round(kcal, 3) });
                }
            }
            else if (animal.Especie == EspecieAnimal.Cao)
            {
                double multiplicador = dieta.FatorK;
                double potencia = dieta.Expoente;
                double manutencao = multiplicador * Math.Pow(peso, potencia);
                double[] fatores = { 0.75, 0.95, 1.1, 1.2 };
                
                int n = Math.Clamp(numeroFilhotes, 0, 4);
                int m = numeroFilhotes > 4 ? numeroFilhotes - 4 : 0;

                for (int s = semanaAtual + 1; s <= 4; s++)
                {
                    double fatorL = fatores[Math.Clamp(s - 1, 0, fatores.Length - 1)];
                    double kcal = manutencao + (peso * ((24 * n) + (12 * m)) * fatorL);
                    previsoes.Add(new { semana = s, kcal = Math.Round(kcal, 3) });
                }
            }

            return JsonSerializer.Serialize(previsoes);
        }

        private void ValidarCustomizacaoPerfil(Dieta dieta, PerfilMetabolico? perfil, double multiplicador, double potencia, ref string descricao)
        {
            if (perfil.HasValue && PerfilMetabolicoDados.Dados.TryGetValue(perfil.Value, out var dados))
            {
                if (dados.multiplicador.HasValue && dados.potencia.HasValue)
                {
                    if (Math.Abs(dados.multiplicador.Value - multiplicador) < 0.001 && Math.Abs(dados.potencia.Value - potencia) < 0.001)
                    {
                        descricao = dados.descricao;
                        dieta.IsCustomizado = false;
                    }
                    else
                    {
                        descricao = $"{dados.descricao} (Customizado)";
                        dieta.IsCustomizado = true;
                    }
                }
                else
                {
                    // Generic profile (already customized)
                    descricao = dados.descricao;
                    dieta.IsCustomizado = true;
                }
            }
            else
            {
                dieta.IsCustomizado = true;
            }
        }
    }
}