using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Domain.AtendimentoAnimal.DTOs.AcompanhamentoDto;
using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests.Infra
{
    /// <summary>Espécies como o backend recebe no JSON (EspecieAnimal: Cao = 1, Gato = 2).</summary>
    public static class Especie
    {
        public const int Cao = 1;
        public const int Gato = 2;
    }

    /// <summary>Formato do ServiceResult na resposta, com o Data tipado.</summary>
    public class ResultadoComDados<T>
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; } = default!;
    }

    /// <summary>
    /// Chamadas de API usadas para montar cenários (cadastrar tutor, animal, raça, ração, consulta),
    /// sempre pelas rotas reais, mais leitura de JSON. Cada helper confere o status de sucesso.
    /// </summary>
    public static class ApiDeTeste
    {
        public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        public static async Task<T> LerAsync<T>(HttpResponseMessage resposta) =>
            (await resposta.Content.ReadFromJsonAsync<T>(Json))!;

        public static async Task<string> TextoAsync(HttpResponseMessage resposta) =>
            await resposta.Content.ReadAsStringAsync();

        /// <summary>
        /// Cookie da sessão devolvido pela resposta, no formato "auth_token=valor" (pronto para o cabeçalho Cookie).
        /// Nulo se a resposta não definiu o cookie ou se o apagou (valor vazio).
        /// </summary>
        public static string? CookieDaSessao(HttpResponseMessage resposta)
        {
            var setCookie = CabecalhoDoCookieDaSessao(resposta);
            var par = setCookie?.Split(';')[0];
            return string.IsNullOrEmpty(par) || par.EndsWith('=') ? null : par;
        }

        /// <summary>O cabeçalho Set-Cookie inteiro do cookie da sessão (com HttpOnly, SameSite, Expires...), ou nulo.</summary>
        public static string? CabecalhoDoCookieDaSessao(HttpResponseMessage resposta) =>
            resposta.Headers.TryGetValues("Set-Cookie", out var valores)
                ? valores.LastOrDefault(v => v.StartsWith("auth_token="))
                : null;

        /// <summary>Lê <c>data.&lt;campo&gt;</c> de um ServiceResult (ex.: "tutorId", "animalId", "id").</summary>
        public static async Task<int> LerIdDoDataAsync(HttpResponseMessage resposta, string campo)
        {
            using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
            return json.RootElement.GetProperty("data").GetProperty(campo).GetInt32();
        }

        public static async Task<int> CadastrarTutorAsync(HttpClient cliente, string? nome = null, string? cpf = null)
        {
            var resposta = await cliente.PostAsJsonAsync("/api/clinica/tutores/cadastrar", new
            {
                nome = nome ?? $"Tutor {Cenario.Unico()}",
                cpf = cpf ?? GeradorCpf.Novo(),
                email = $"tutor.{Cenario.Unico()}@teste.local",
            });
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerIdDoDataAsync(resposta, "tutorId");
        }

        /// <summary>Nascimento usado em <see cref="NovoAnimal"/>.</summary>
        public static readonly DateTime NascimentoPadrao = new(2020, 5, 10, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>Idade em anos completos do animal de <see cref="NovoAnimal"/>, como o backend calcula.</summary>
        public static int IdadeEmAnos()
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - NascimentoPadrao.Year;
            if (NascimentoPadrao.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }

        /// <summary>Corpo de cadastro de animal com todos os campos obrigatórios.</summary>
        public static object NovoAnimal(int racaId, int especie = Especie.Cao, string? nome = null, IEnumerable<int>? tutorIds = null) => new
        {
            nome = nome ?? $"Animal {Cenario.Unico()}",
            racaId,
            especie,
            sexo = 1,
            isCastrado = false,
            nascimento = new DateTime(2020, 5, 10, 0, 0, 0, DateTimeKind.Utc),
            tutorIds = tutorIds?.ToList(),
        };

        public static async Task<int> CadastrarAnimalAsync(HttpClient cliente, int racaId, int especie = Especie.Cao, string? nome = null, IEnumerable<int>? tutorIds = null)
        {
            var resposta = await cliente.PostAsJsonAsync("/api/clinica/animais/cadastrar", NovoAnimal(racaId, especie, nome, tutorIds));
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerIdDoDataAsync(resposta, "animalId");
        }

        public static async Task<int> CadastrarRacaDaClinicaAsync(HttpClient cliente, string? nome = null, int especie = Especie.Cao)
        {
            var resposta = await cliente.PostAsJsonAsync("/api/clinica/racas/cadastrar", new { nome = nome ?? $"Raça {Cenario.Unico()}", especie });
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerIdDoDataAsync(resposta, "id");
        }

        /// <summary>Cadastra uma raça do sistema (catálogo compartilhado) com o cliente do admin.</summary>
        public static async Task<int> CadastrarRacaDoSistemaAsync(HttpClient admin, string? nome = null, int especie = Especie.Cao)
        {
            var resposta = await admin.PostAsJsonAsync("/api/sistema/racas/cadastrar", new
            {
                nome = nome ?? $"Raça do sistema {Cenario.Unico()}",
                especie,
                pesoMinimoReferenciaMacho = 20.0,
                pesoMaximoReferenciaMacho = 30.0,
                pesoMinimoReferenciaFemea = 18.0,
                pesoMaximoReferenciaFemea = 26.0,
            });
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerIdDoDataAsync(resposta, "id");
        }

        /// <summary>Corpo de cadastro de ração (valores em %, como no formulário).</summary>
        public static object NovaRacao(string? nome = null, int especie = Especie.Cao) => new
        {
            nome = nome ?? $"Ração {Cenario.Unico()}",
            especie,
            isPorcentagem = true,
            umidade = 10.0,
            proteinaBruta = 25.0,
            extratoEtereo = 14.0,
            materiaMineral = 7.0,
            fibraBruta = 3.0,
        };

        public static async Task<int> CadastrarRacaoDaClinicaAsync(HttpClient cliente, string? nome = null, int especie = Especie.Cao)
        {
            var resposta = await cliente.PostAsJsonAsync("/api/clinica/racoes/cadastrar", NovaRacao(nome, especie));
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerIdDoDataAsync(resposta, "id");
        }

        public static async Task<int> CadastrarRacaoDoSistemaAsync(HttpClient admin, string? nome = null, int especie = Especie.Cao)
        {
            var resposta = await admin.PostAsJsonAsync("/api/sistema/racoes/cadastrar", NovaRacao(nome ?? $"Ração do sistema {Cenario.Unico()}", especie));
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerIdDoDataAsync(resposta, "id");
        }

        public static async Task<int> CadastrarConsultaAsync(HttpClient cliente, int animalId, int? tutorId = null, DateTime? dataConsulta = null)
        {
            var resposta = await cliente.PostAsJsonAsync("/api/clinica/consultas/cadastrar", new
            {
                animalId,
                tutorId,
                dataConsulta = dataConsulta ?? DateTime.UtcNow.AddDays(1),
            });
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
            return json.RootElement.GetProperty("id").GetInt32();
        }

        /// <summary>Clínica com GESTOR, raça, tutor, animal vinculado ao tutor, ração e uma consulta marcada.</summary>
        public record Atendimento(ClinicaDeTeste Clinica, UsuarioDeTeste UsuarioGestor, int RacaId, int TutorId, int AnimalId, int RacaoId, string NomeRacao, int ConsultaId, DateTime DataConsulta)
        {
            public HttpClient Gestor => UsuarioGestor.Cliente;
        }

        /// <summary>Monta, pelas rotas reais, tudo o que uma avaliação precisa: clínica, gestor, animal com tutor, ração e consulta.</summary>
        public static async Task<Atendimento> MontarAtendimentoAsync(Cenario cenario, string apelido = "A", int especie = Especie.Cao)
        {
            var clinica = await cenario.CriarClinicaAsync(apelido);
            var usuario = await cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);
            var gestor = usuario.Cliente;
            var racaId = await CadastrarRacaDaClinicaAsync(gestor, especie: especie);
            var tutorId = await CadastrarTutorAsync(gestor);
            var animalId = await CadastrarAnimalAsync(gestor, racaId, especie, tutorIds: new[] { tutorId });
            var nomeRacao = $"Ração {Cenario.Unico()}";
            var racaoId = await CadastrarRacaoDaClinicaAsync(gestor, nomeRacao, especie);
            var data = new DateTime(2030, 3, 15, 13, 30, 0, DateTimeKind.Utc);
            var consultaId = await CadastrarConsultaAsync(gestor, animalId, tutorId, data);
            return new Atendimento(clinica, usuario, racaId, tutorId, animalId, racaoId, nomeRacao, consultaId, data);
        }

        /// <summary>Corpo da avaliação (PUT consultas/{id}/avaliacao) com todos os campos preenchidos.</summary>
        public static object NovaAvaliacao(int racaoId, double peso = 20.5, int? condicaoCorporal = 5, int? grauAtividade = 2) => new
        {
            peso,
            condicaoCorporal,
            grauAtividade,
            racaoId,
            descricaoDieta = "Duas refeições ao dia",
            tipoDieta = 1,
            isGestante = false,
            isLactanteNumeroFilhotes = 0,
            semanaLactacao = 0,
            semanaGestacao = 0,
        };

        /// <summary>Salva a avaliação da consulta pela rota real e devolve o DTO da resposta.</summary>
        public static async Task<GetAvaliacaoDto> SalvarAvaliacaoAsync(HttpClient cliente, int consultaId, int racaoId, double peso = 20.5)
        {
            var resposta = await cliente.PutAsJsonAsync($"/api/clinica/consultas/{consultaId}/avaliacao", NovaAvaliacao(racaoId, peso));
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return (await LerAsync<ResultadoComDados<GetAvaliacaoDto>>(resposta)).Data;
        }

        /// <summary>Cria (ou recalcula) a dieta da avaliação pela rota real e devolve o DTO da resposta.</summary>
        public static async Task<GetDietaDto> CriarDietaAsync(HttpClient cliente, GetAvaliacaoDto avaliacao, object? corpo = null)
        {
            var resposta = await cliente.PostAsJsonAsync("/api/clinica/dietas", corpo ?? NovaDieta(avaliacao));
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return (await LerAsync<ResultadoComDados<GetDietaDto>>(resposta)).Data;
        }

        /// <summary>Corpo de dieta de manutenção para cão ativo (perfil 4: 130 × PC^0,75), com o peso da avaliação.</summary>
        public static object NovaDieta(GetAvaliacaoDto avaliacao, double? pesoAtual = null) => new
        {
            avaliacaoId = avaliacao.Id,
            acompanhamentoId = avaliacao.AcompanhamentoId,
            racaoId = avaliacao.RacaoId,
            tipoDieta = 1,
            perfilMetabolico = 4,
            multiplicadorEnergetico = 130.0,
            potencia = 0.75,
            pesoAtual = pesoAtual ?? avaliacao.Peso,
            observacoes = "Dieta de teste",
        };

        /// <summary>Corpo de dieta de emagrecimento para cão (perfil 18), com peso alvo e nível de restrição.</summary>
        public static object NovaDietaEmagrecimento(GetAvaliacaoDto avaliacao, double pesoAtual = 30, double pesoObjetivo = 25, int nivelRestricao = 2) => new
        {
            avaliacaoId = avaliacao.Id,
            acompanhamentoId = avaliacao.AcompanhamentoId,
            racaoId = avaliacao.RacaoId,
            tipoDieta = 5,
            perfilMetabolico = 18,
            multiplicadorEnergetico = 130.0,
            potencia = 0.75,
            pesoAtual,
            pesoObjetivo,
            nivelRestricao,
        };

        /// <summary>Histórico completo do animal (GET acompanhamentos/{id}/historico-completo).</summary>
        public static async Task<AnimalCompletoDto> HistoricoAsync(HttpClient cliente, int acompanhamentoId)
        {
            var resposta = await cliente.GetAsync($"/api/clinica/acompanhamentos/{acompanhamentoId}/historico-completo");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return (await LerAsync<ResultadoComDados<AnimalCompletoDto>>(resposta)).Data;
        }

        /// <summary>Id do acompanhamento criado junto com o animal.</summary>
        public static async Task<int> AcompanhamentoDoAnimalAsync(ImperiumApiFactory factory, int animalId)
        {
            await using var contexto = factory.CriarContexto();
            return (await contexto.AcompanhamentosInds.AsNoTracking().FirstAsync(a => a.AnimalId == animalId)).Id;
        }

        /// <summary>
        /// Grava direto no banco uma avaliação mínima da consulta usando a ração (deixa a ração "em uso").
        /// O fluxo real de avaliação tem muitos campos; aqui só importa o vínculo com a ração.
        /// </summary>
        public static async Task RegistrarAvaliacaoComRacaoAsync(ImperiumApiFactory factory, int consultaId, int animalId, int racaoId)
        {
            await using var contexto = factory.CriarContexto();
            var acompanhamento = await contexto.AcompanhamentosInds.FirstAsync(a => a.AnimalId == animalId);
            contexto.Avaliacoes.Add(new Avaliacao
            {
                ConsultaId = consultaId,
                AcompanhamentoId = acompanhamento.Id,
                RacaoId = racaoId,
                Peso = 20,
                Idade = 4,
                TipoDieta = TipoDieta.Manutencao,
            });
            await contexto.SaveChangesAsync();
        }
    }
}
