using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Domain.AtendimentoAnimal.Entities;
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
