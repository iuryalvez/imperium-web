using System.Net;
using System.Net.Http.Json;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.SaudeAnimal.DTOs.Racao;
using backend.Domain.Utils.Paginacao;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Sistema
{
    /// <summary>
    /// Testes de integração do catálogo do sistema mantido pelo admin: api/sistema/racas e api/sistema/racoes
    /// (CRUD, status, exclusão bloqueada por uso, cópias das clínicas, acesso só do admin e paginação).
    /// O catálogo é compartilhado por todos os testes, então as buscas usam nomes únicos.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class CatalogoSistemaTests
    {
        private const string RotaRacas = "/api/sistema/racas";
        private const string RotaRacoes = "/api/sistema/racoes";

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public CatalogoSistemaTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Raças do sistema

        [Fact]
        public async Task CadastrarRaca_Admin_CriaRacaDoSistemaVisivelParaAsClinicas()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var nome = $"Sistema {Cenario.Unico()}";

            var resposta = await admin.PostAsJsonAsync($"{RotaRacas}/cadastrar", new { nome, especie = Especie.Gato });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var id = await LerIdDoDataAsync(resposta, "id");
            var raca = await LerAsync<GetRacaDto>(await admin.GetAsync($"{RotaRacas}/{id}"));
            Assert.True(raca.IsSistema);
            Assert.Null(raca.ClinicaId);
            var gestor = await GestorDeClinicaNovaAsync();
            Assert.Equal(HttpStatusCode.OK, (await gestor.GetAsync($"/api/clinica/racas/{id}")).StatusCode);
        }

        [Fact]
        public async Task EditarRaca_Admin_AlteraOProprioItemSemCopiaEMantemPesos()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var id = await CadastrarRacaDoSistemaAsync(admin);

            var resposta = await admin.PutAsJsonAsync($"{RotaRacas}/editar", new { id, nome = "Editada pelo admin", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(id, await LerIdDoDataAsync(resposta, "id"));
            var raca = await LerAsync<GetRacaDto>(await admin.GetAsync($"{RotaRacas}/{id}"));
            Assert.Equal("Editada pelo admin", raca.Nome);
            Assert.Equal(20.0, raca.PesoMinimoReferenciaMacho);
            Assert.Equal(30.0, raca.PesoMaximoReferenciaMacho);
        }

        [Fact]
        public async Task MudarStatusRaca_AdminDesativa_ClinicaNaoUsaEmCadastroNovo()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var id = await CadastrarRacaDoSistemaAsync(admin);
            var gestor = await GestorDeClinicaNovaAsync();

            var resposta = await admin.PutAsync($"{RotaRacas}/{id}/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.False((await LerAsync<GetRacaDto>(await gestor.GetAsync($"/api/clinica/racas/{id}"))).IsAtivo);
            var animal = await gestor.PostAsJsonAsync("/api/clinica/animais/cadastrar", NovoAnimal(id));
            Assert.Equal(HttpStatusCode.BadRequest, animal.StatusCode);
        }

        [Fact]
        public async Task ExcluirRaca_EmUsoPorAnimalDeClinica_RetornaConflict()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var id = await CadastrarRacaDoSistemaAsync(admin);
            var gestor = await GestorDeClinicaNovaAsync();
            await CadastrarAnimalAsync(gestor, id);

            var resposta = await admin.DeleteAsync($"{RotaRacas}/{id}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync($"{RotaRacas}/{id}")).StatusCode);
        }

        [Fact]
        public async Task ExcluirRaca_SemUso_RetornaOkERemove()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var id = await CadastrarRacaDoSistemaAsync(admin);

            Assert.Equal(HttpStatusCode.OK, (await admin.DeleteAsync($"{RotaRacas}/{id}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{RotaRacas}/{id}")).StatusCode);
        }

        [Fact]
        public async Task RotasComIdDeRaca_Inexistente_RetornaNotFound()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            const int inexistente = 999999999;

            Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{RotaRacas}/{inexistente}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.PutAsJsonAsync($"{RotaRacas}/editar", new { id = inexistente, nome = "X", especie = Especie.Cao })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.PutAsync($"{RotaRacas}/{inexistente}/status?ativo=false", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.DeleteAsync($"{RotaRacas}/{inexistente}")).StatusCode);
        }

        [Fact]
        public async Task CadastrarRaca_SemNome_RetornaBadRequestComMensagem()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();

            var resposta = await admin.PostAsJsonAsync($"{RotaRacas}/cadastrar", new { especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome da raça é obrigatório.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task ListarRacas_Sistema_TrazSoOCatalogoSemAsDasClinicas()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var prefixo = $"Lista{Cenario.Unico()}";
            var doSistema = await CadastrarRacaDoSistemaAsync(admin, $"{prefixo} sistema");
            var gestor = await GestorDeClinicaNovaAsync();
            var daClinica = await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} clínica");

            var pagina = await LerAsync<PaginaResultado<GetRacaDto>>(await admin.GetAsync($"{RotaRacas}?busca={prefixo}"));

            Assert.Equal(doSistema, Assert.Single(pagina.Itens).Id);
            Assert.DoesNotContain(pagina.Itens, r => r.Id == daClinica);
        }

        [Fact]
        public async Task ListarCopiasDeRaca_CopiaFeitaPelaClinica_ApareceComNomeDaClinica()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var original = await CadastrarRacaDoSistemaAsync(admin);
            var clinica = await _cenario.CriarClinicaAsync("Copiadora");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var nomeCopia = $"Cópia {Cenario.Unico()}";
            var edicao = await gestor.PutAsJsonAsync("/api/clinica/racas/editar", new { id = original, nome = nomeCopia, especie = Especie.Cao });
            var idCopia = await LerIdDoDataAsync(edicao, "id");

            var pagina = await LerAsync<PaginaResultado<GetRacaDto>>(await admin.GetAsync($"{RotaRacas}/copias?busca={Uri.EscapeDataString(nomeCopia)}"));

            var copia = Assert.Single(pagina.Itens);
            Assert.Equal(idCopia, copia.Id);
            Assert.Equal(clinica.Id, copia.ClinicaId);
            Assert.Equal(clinica.Nome, copia.NomeClinica);
        }

        [Fact]
        public async Task ListarCopiasDeRaca_BuscaPeloNomeDaClinica_RetornaAsDaClinica()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinica = await _cenario.CriarClinicaAsync($"Busca{Cenario.Unico()}");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var propria = await CadastrarRacaDaClinicaAsync(gestor);

            var pagina = await LerAsync<PaginaResultado<GetRacaDto>>(await admin.GetAsync($"{RotaRacas}/copias?busca={Uri.EscapeDataString(clinica.Nome)}"));

            Assert.Equal(propria, Assert.Single(pagina.Itens).Id);
        }

        #endregion

        #region Rações do sistema

        [Fact]
        public async Task CadastrarRacao_Admin_CriaRacaoDoSistemaComEnergiaCalculada()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();

            var resposta = await admin.PostAsJsonAsync($"{RotaRacoes}/cadastrar", NovaRacao($"Sistema {Cenario.Unico()}"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var racao = await LerAsync<GetRacoesDto>(await admin.GetAsync($"{RotaRacoes}/{await LerIdDoDataAsync(resposta, "id")}"));
            Assert.True(racao.IsSistema);
            Assert.Equal(3.668, racao.EnergiaMetabolizavel);
        }

        [Fact]
        public async Task EditarRacao_AdminSoONome_MantemComposicao()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var id = await CadastrarRacaoDoSistemaAsync(admin);

            var resposta = await admin.PutAsJsonAsync($"{RotaRacoes}/editar", new { id, nome = "Renomeada pelo admin" });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var racao = await LerAsync<GetRacoesDto>(await admin.GetAsync($"{RotaRacoes}/{id}"));
            Assert.Equal("Renomeada pelo admin", racao.Nome);
            Assert.True(racao.IsPorcentagem);
            Assert.Equal(25.0, racao.ProteinaBruta);
            Assert.Equal(3.668, racao.EnergiaMetabolizavel);
        }

        [Fact]
        public async Task ExcluirRacao_EmUsoPorAvaliacao_RetornaConflict()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var racaoId = await CadastrarRacaoDoSistemaAsync(admin);
            var gestor = await GestorDeClinicaNovaAsync();
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            var animalId = await CadastrarAnimalAsync(gestor, racaId);
            var consultaId = await CadastrarConsultaAsync(gestor, animalId);
            await RegistrarAvaliacaoComRacaoAsync(_factory, consultaId, animalId, racaoId);

            var resposta = await admin.DeleteAsync($"{RotaRacoes}/{racaoId}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync($"{RotaRacoes}/{racaoId}")).StatusCode);
        }

        [Fact]
        public async Task MudarStatusRacao_AdminDesativa_ClinicaVeInativa()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var id = await CadastrarRacaoDoSistemaAsync(admin);
            var gestor = await GestorDeClinicaNovaAsync();

            Assert.Equal(HttpStatusCode.OK, (await admin.PutAsync($"{RotaRacoes}/{id}/status?ativo=false", null)).StatusCode);

            Assert.False((await LerAsync<GetRacoesDto>(await gestor.GetAsync($"/api/clinica/racoes/{id}"))).IsAtivo);
        }

        [Fact]
        public async Task RotasComIdDeRacao_Inexistente_RetornaNotFound()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            const int inexistente = 999999999;

            Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{RotaRacoes}/{inexistente}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.PutAsJsonAsync($"{RotaRacoes}/editar", new { id = inexistente, nome = "X" })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.PutAsync($"{RotaRacoes}/{inexistente}/status?ativo=false", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.DeleteAsync($"{RotaRacoes}/{inexistente}")).StatusCode);
        }

        [Fact]
        public async Task ListarCopiasDeRacao_CopiaFeitaPelaClinica_ApareceComNomeDaClinica()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var original = await CadastrarRacaoDoSistemaAsync(admin);
            var clinica = await _cenario.CriarClinicaAsync("Copiadora");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var nomeCopia = $"Cópia {Cenario.Unico()}";
            var edicao = await gestor.PutAsJsonAsync("/api/clinica/racoes/editar", new { id = original, nome = nomeCopia });
            var idCopia = await LerIdDoDataAsync(edicao, "id");

            var pagina = await LerAsync<PaginaResultado<GetRacoesDto>>(await admin.GetAsync($"{RotaRacoes}/copias?busca={Uri.EscapeDataString(nomeCopia)}"));

            var copia = Assert.Single(pagina.Itens);
            Assert.Equal(idCopia, copia.Id);
            Assert.Equal(clinica.Nome, copia.NomeClinica);
        }

        #endregion

        #region Acesso e paginação

        [Theory]
        [InlineData(RotaRacas)]
        [InlineData(RotaRacoes)]
        [InlineData(RotaRacas + "/copias")]
        [InlineData(RotaRacoes + "/copias")]
        public async Task RotasDoSistema_GestorDeClinica_RetornaForbidden(string rota)
        {
            var gestor = await GestorDeClinicaNovaAsync();

            Assert.Equal(HttpStatusCode.Forbidden, (await gestor.GetAsync(rota)).StatusCode);
        }

        [Fact]
        public async Task RotasDoSistema_SemToken_RetornaUnauthorized()
        {
            Assert.Equal(HttpStatusCode.Unauthorized, (await _cenario.ClienteAnonimo().GetAsync(RotaRacas)).StatusCode);
        }

        [Fact]
        public async Task ListarRacoes_TamanhoPaginaAcimaDoMaximo_LimitaA100()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();

            var pagina = await LerAsync<PaginaResultado<GetRacoesDto>>(await admin.GetAsync($"{RotaRacoes}?tamanhoPagina=500"));

            Assert.Equal(100, pagina.TamanhoPagina);
        }

        [Fact]
        public async Task ListarRacoes_BuscaEOrdenacaoPorNome_RetornaEmOrdemAlfabetica()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var prefixo = $"Ord{Cenario.Unico()}";
            await CadastrarRacaoDoSistemaAsync(admin, $"{prefixo} C");
            await CadastrarRacaoDoSistemaAsync(admin, $"{prefixo} A");
            await CadastrarRacaoDoSistemaAsync(admin, $"{prefixo} B");

            var pagina = await LerAsync<PaginaResultado<GetRacoesDto>>(await admin.GetAsync($"{RotaRacoes}?busca={prefixo}&ordenarPor=nome&ordem=asc"));

            Assert.Equal(new[] { $"{prefixo} A", $"{prefixo} B", $"{prefixo} C" }, pagina.Itens.Select(r => r.Nome));
        }

        #endregion

        private async Task<HttpClient> GestorDeClinicaNovaAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            return await _cenario.ClienteAsync(clinica, Perfis.Gestor);
        }
    }
}
