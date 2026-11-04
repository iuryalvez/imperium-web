using System.Net;
using System.Net.Http.Json;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.Utils.Paginacao;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Racas
{
    /// <summary>
    /// Testes de integração das rotas api/clinica/racas: raças próprias da clínica e uso do catálogo do sistema
    /// (cópia ao editar, ocultar só para a clínica, sem excluir/desativar item do sistema), isolamento, perfis,
    /// validação, exclusão bloqueada por uso e paginação.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class RacaTests
    {
        private const string Rota = "/api/clinica/racas";
        private const string RotaSistema = "/api/sistema/racas";

        private readonly Cenario _cenario;

        public RacaTests(ImperiumApiFactory factory)
        {
            _cenario = new Cenario(factory);
        }

        #region Caminho feliz

        [Fact]
        public async Task Cadastrar_DadosValidos_RetornaOkComRacaDaClinica()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { nome = "Vira-lata Caramelo", especie = Especie.Cao, pesoMinimoReferenciaMacho = 10.0 });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var raca = await ObterAsync(gestor, await LerIdDoDataAsync(resposta, "id"));
            Assert.Equal("Vira-lata Caramelo", raca.Nome);
            Assert.Equal(clinica.Id, raca.ClinicaId);
            Assert.False(raca.IsSistema);
            Assert.True(raca.IsAtivo);
            Assert.Equal(10.0, raca.PesoMinimoReferenciaMacho);
        }

        [Fact]
        public async Task ObterPorId_RacaDoSistema_RetornaParaQualquerClinica()
        {
            var racaSistema = await RacaDoSistemaAsync();
            var (_, gestor) = await ClinicaComGestorAsync();

            var raca = await ObterAsync(gestor, racaSistema);

            Assert.True(raca.IsSistema);
            Assert.Null(raca.ClinicaId);
        }

        [Fact]
        public async Task Editar_RacaPropria_AtualizaSemCriarCopia()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaDaClinicaAsync(gestor, "Nome Antigo");

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Nome Novo", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(id, await LerIdDoDataAsync(resposta, "id"));
            Assert.Equal("Nome Novo", (await ObterAsync(gestor, id)).Nome);
        }

        [Fact]
        public async Task MudarStatus_RacaPropria_DesativaEReativa()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaDaClinicaAsync(gestor);

            Assert.Equal(HttpStatusCode.OK, (await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null)).StatusCode);
            Assert.False((await ObterAsync(gestor, id)).IsAtivo);

            Assert.Equal(HttpStatusCode.OK, (await gestor.PutAsync($"{Rota}/{id}/status?ativo=true", null)).StatusCode);
            Assert.True((await ObterAsync(gestor, id)).IsAtivo);
        }

        [Fact]
        public async Task Excluir_RacaPropriaSemUso_RetornaOkERemove()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaDaClinicaAsync(gestor);

            var resposta = await gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region Catálogo do sistema

        [Fact]
        public async Task Editar_RacaDoSistema_CriaCopiaDaClinicaEOriginalNaoMuda()
        {
            var nomeOriginal = $"Original {Cenario.Unico()}";
            var racaSistema = await RacaDoSistemaAsync(nomeOriginal);
            var (clinicaA, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();

            var resposta = await gestorA.PutAsJsonAsync($"{Rota}/editar", new { id = racaSistema, nome = "Versão da clínica A", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var idCopia = await LerIdDoDataAsync(resposta, "id");
            Assert.NotEqual(racaSistema, idCopia);
            var copia = await ObterAsync(gestorA, idCopia);
            Assert.Equal("Versão da clínica A", copia.Nome);
            Assert.Equal(clinicaA.Id, copia.ClinicaId);
            Assert.Equal(nomeOriginal, (await ObterAsync(gestorA, racaSistema)).Nome);
            Assert.Equal(nomeOriginal, (await ObterAsync(gestorB, racaSistema)).Nome);
            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.GetAsync($"{Rota}/{idCopia}")).StatusCode);
        }

        [Fact]
        public async Task Editar_RacaDoSistemaSemPesos_CopiaMantemPesosDoOriginal()
        {
            var racaSistema = await RacaDoSistemaAsync();
            var (_, gestor) = await ClinicaComGestorAsync();
            var original = await ObterAsync(gestor, racaSistema);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id = racaSistema, nome = "Cópia só com nome", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var copia = await ObterAsync(gestor, await LerIdDoDataAsync(resposta, "id"));
            Assert.Equal(original.PesoMinimoReferenciaMacho, copia.PesoMinimoReferenciaMacho);
            Assert.Equal(original.PesoMaximoReferenciaMacho, copia.PesoMaximoReferenciaMacho);
            Assert.Equal(original.PesoMinimoReferenciaFemea, copia.PesoMinimoReferenciaFemea);
            Assert.Equal(original.PesoMaximoReferenciaFemea, copia.PesoMaximoReferenciaFemea);
        }

        [Fact]
        public async Task Editar_RacaPropriaSemPesos_MantemPesos()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var cadastro = await gestor.PostAsJsonAsync($"{Rota}/cadastrar",
                new { nome = "Com pesos", especie = Especie.Cao, pesoMinimoReferenciaMacho = 8.0, pesoMaximoReferenciaMacho = 12.0 });
            var id = await LerIdDoDataAsync(cadastro, "id");

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Com pesos editada", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var raca = await ObterAsync(gestor, id);
            Assert.Equal(8.0, raca.PesoMinimoReferenciaMacho);
            Assert.Equal(12.0, raca.PesoMaximoReferenciaMacho);
        }

        [Fact]
        public async Task Excluir_RacaDoSistema_RetornaBadRequestENaoRemove()
        {
            var racaSistema = await RacaDoSistemaAsync();
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.DeleteAsync($"{Rota}/{racaSistema}");

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Raças do sistema não podem ser excluídas pela clínica.", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await gestor.GetAsync($"{Rota}/{racaSistema}")).StatusCode);
        }

        [Fact]
        public async Task MudarStatus_RacaDoSistema_RetornaBadRequestENaoAltera()
        {
            var racaSistema = await RacaDoSistemaAsync();
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PutAsync($"{Rota}/{racaSistema}/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.True((await ObterAsync(gestor, racaSistema)).IsAtivo);
        }

        [Fact]
        public async Task Ocultar_RacaDoSistema_TiraDasOpcoesSoDaClinica()
        {
            var nome = $"Oculta {Cenario.Unico()}";
            var racaSistema = await RacaDoSistemaAsync(nome);
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();

            var resposta = await gestorA.PutAsync($"{Rota}/{racaSistema}/ocultar?oculta=true", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.DoesNotContain(await OpcoesAsync(gestorA, nome), o => o.Id == racaSistema);
            Assert.Contains(await OpcoesAsync(gestorB, nome), o => o.Id == racaSistema);
            Assert.True((await ObterAsync(gestorA, racaSistema)).IsOculta);
            Assert.False((await ObterAsync(gestorB, racaSistema)).IsOculta);
        }

        [Fact]
        public async Task Ocultar_Desfazer_VoltaParaAsOpcoes()
        {
            var nome = $"Volta {Cenario.Unico()}";
            var racaSistema = await RacaDoSistemaAsync(nome);
            var (_, gestor) = await ClinicaComGestorAsync();
            await gestor.PutAsync($"{Rota}/{racaSistema}/ocultar?oculta=true", null);

            var resposta = await gestor.PutAsync($"{Rota}/{racaSistema}/ocultar?oculta=false", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Contains(await OpcoesAsync(gestor, nome), o => o.Id == racaSistema);
        }

        [Fact]
        public async Task Ocultar_RacaPropria_RetornaBadRequest()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaDaClinicaAsync(gestor);

            var resposta = await gestor.PutAsync($"{Rota}/{id}/ocultar?oculta=true", null);

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Só raças do sistema podem ser ocultadas.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Listar_FiltroOcultaTrue_RetornaSoAsOcultasPelaClinica()
        {
            var racaSistema = await RacaDoSistemaAsync();
            var (_, gestor) = await ClinicaComGestorAsync();
            await gestor.PutAsync($"{Rota}/{racaSistema}/ocultar?oculta=true", null);

            var pagina = await ListarAsync(gestor, "?oculta=true");

            Assert.Equal(racaSistema, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Opcoes_RacaInativa_VemComIsAtivoFalseDepoisDasAtivas()
        {
            var prefixo = $"Opc {Cenario.Unico()}";
            var (_, gestor) = await ClinicaComGestorAsync();
            var inativa = await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} A");
            var ativa = await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} B");
            await gestor.PutAsync($"{Rota}/{inativa}/status?ativo=false", null);

            var opcoes = await OpcoesAsync(gestor, prefixo);

            Assert.Equal(new[] { ativa, inativa }, opcoes.Select(o => o.Id));
            Assert.False(opcoes.Single(o => o.Id == inativa).IsAtivo);
        }

        [Fact]
        public async Task Opcoes_FiltroEspecie_RetornaSoAEspecie()
        {
            var prefixo = $"Esp {Cenario.Unico()}";
            var (_, gestor) = await ClinicaComGestorAsync();
            await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} cão", Especie.Cao);
            var gato = await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} gato", Especie.Gato);

            var resposta = await gestor.GetAsync($"{Rota}/opcoes?busca={Uri.EscapeDataString(prefixo)}&especie={Especie.Gato}");

            Assert.Equal(gato, Assert.Single(await LerAsync<List<OpcaoDto>>(resposta)).Id);
        }

        [Fact]
        public async Task Excluir_RacaEmUsoPorAnimal_RetornaConflictENaoRemove()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaDaClinicaAsync(gestor);
            await CadastrarAnimalAsync(gestor, id);

            var resposta = await gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("Esta raça está em uso por animais", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task ObterPorId_RacaDeOutraClinica_RetornaNotFound()
        {
            var (_, gestorB, idA) = await RacaEmAComGestorDeBAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.GetAsync($"{Rota}/{idA}")).StatusCode);
        }

        [Fact]
        public async Task Editar_RacaDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (gestorA, gestorB, idA) = await RacaEmAComGestorDeBAsync();

            var resposta = await gestorB.PutAsJsonAsync($"{Rota}/editar", new { id = idA, nome = "Alterada por B", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Equal("Raça de A", (await ObterAsync(gestorA, idA)).Nome);
        }

        [Fact]
        public async Task MudarStatus_RacaDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (gestorA, gestorB, idA) = await RacaEmAComGestorDeBAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.PutAsync($"{Rota}/{idA}/status?ativo=false", null)).StatusCode);
            Assert.True((await ObterAsync(gestorA, idA)).IsAtivo);
        }

        [Fact]
        public async Task Ocultar_RacaDeOutraClinica_RetornaNotFound()
        {
            var (_, gestorB, idA) = await RacaEmAComGestorDeBAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.PutAsync($"{Rota}/{idA}/ocultar?oculta=true", null)).StatusCode);
        }

        [Fact]
        public async Task Excluir_RacaDeOutraClinica_RetornaNotFoundENaoRemove()
        {
            var (gestorA, gestorB, idA) = await RacaEmAComGestorDeBAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.DeleteAsync($"{Rota}/{idA}")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await gestorA.GetAsync($"{Rota}/{idA}")).StatusCode);
        }

        [Fact]
        public async Task ListarEOpcoes_OutraClinica_NaoMostraRacaDeA()
        {
            var (_, gestorB, idA) = await RacaEmAComGestorDeBAsync();

            Assert.Equal(0, (await ListarAsync(gestorB, "?origem=clinica")).Total);
            Assert.DoesNotContain(await OpcoesAsync(gestorB, "Raça de A"), o => o.Id == idA);
        }

        #endregion

        #region Perfis

        [Theory]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Tecnico)]
        public async Task Cadastrar_PerfilComGerenciaAnimais_RetornaOk(string perfil)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            var resposta = await cliente.PostAsJsonAsync($"{Rota}/cadastrar", new { nome = "Por perfil", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Theory]
        [InlineData("POST", "cadastrar")]
        [InlineData("PUT", "editar")]
        [InlineData("PUT", "{id}/status?ativo=false")]
        [InlineData("PUT", "{id}/ocultar?oculta=true")]
        [InlineData("DELETE", "{id}")]
        public async Task RotasDeEscrita_PerfilSecretario_RetornaForbidden(string metodo, string caminho)
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var secretario = await _cenario.ClienteAsync(clinica, Perfis.Secretario);
            var id = await CadastrarRacaDaClinicaAsync(gestor, "Protegida");

            var requisicao = new HttpRequestMessage(new HttpMethod(metodo), $"{Rota}/{caminho.Replace("{id}", id.ToString())}");
            if (caminho is "cadastrar" or "editar")
                requisicao.Content = JsonContent.Create(new { id, nome = "Alterada", especie = Especie.Cao });

            var resposta = await secretario.SendAsync(requisicao);

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
            var raca = await ObterAsync(gestor, id);
            Assert.Equal("Protegida", raca.Nome);
            Assert.True(raca.IsAtivo);
        }

        [Fact]
        public async Task Leitura_PerfilSecretario_RetornaOk()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var secretario = await _cenario.ClienteAsync(clinica, Perfis.Secretario);
            var id = await CadastrarRacaDaClinicaAsync(gestor);

            Assert.Equal(HttpStatusCode.OK, (await secretario.GetAsync(Rota)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await secretario.GetAsync($"{Rota}/{id}")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await secretario.GetAsync($"{Rota}/opcoes")).StatusCode);
        }

        [Fact]
        public async Task RotasDoSistema_GestorDaClinica_RetornaForbidden()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{RotaSistema}/cadastrar", new { nome = "Tentativa", especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }

        #endregion

        #region Validação

        [Fact]
        public async Task Cadastrar_SemNome_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome da raça é obrigatório.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_NomeComMaisDe100Caracteres_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { nome = new string('x', 101), especie = Especie.Cao });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome da raça deve ter no máximo 100 caracteres.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_EspecieInvalida_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { nome = "Espécie ruim", especie = 99 });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A espécie é inválida.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task RotasComId_RacaInexistente_RetornaNotFound()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            const int inexistente = 999999999;

            Assert.Equal(HttpStatusCode.NotFound, (await gestor.GetAsync($"{Rota}/{inexistente}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.PutAsJsonAsync($"{Rota}/editar", new { id = inexistente, nome = "X", especie = Especie.Cao })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.PutAsync($"{Rota}/{inexistente}/status?ativo=false", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.PutAsync($"{Rota}/{inexistente}/ocultar?oculta=true", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.DeleteAsync($"{Rota}/{inexistente}")).StatusCode);
        }

        #endregion

        #region Paginação

        [Fact]
        public async Task Listar_OrigemClinica_PaginaSoAsPropriasMaisRecentesPrimeiro()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var ids = new List<int>();
            for (var i = 1; i <= 12; i++) ids.Add(await CadastrarRacaDaClinicaAsync(gestor, $"Raça {i:00}"));

            var pagina = await ListarAsync(gestor, "?origem=clinica");

            Assert.Equal(12, pagina.Total);
            Assert.Equal(10, pagina.TamanhoPagina);
            Assert.Equal(ids.AsEnumerable().Reverse().Take(10), pagina.Itens.Select(r => r.Id));
        }

        [Fact]
        public async Task Listar_OrigemSistema_NaoTrazAsPropriasDaClinica()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var propria = await CadastrarRacaDaClinicaAsync(gestor);

            var pagina = await ListarAsync(gestor, "?origem=sistema&tamanhoPagina=100");

            Assert.DoesNotContain(pagina.Itens, r => r.Id == propria);
            Assert.All(pagina.Itens, r => Assert.True(r.IsSistema));
        }

        [Fact]
        public async Task Listar_TamanhoPaginaAcimaDoMaximo_LimitaA100()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            Assert.Equal(100, (await ListarAsync(gestor, "?tamanhoPagina=101")).TamanhoPagina);
        }

        [Fact]
        public async Task Listar_BuscaEOrdenacaoPorNome_RetornaEmOrdemAlfabetica()
        {
            var prefixo = $"Busca{Cenario.Unico()}";
            var (_, gestor) = await ClinicaComGestorAsync();
            await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} Cocker");
            await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} Akita");
            await CadastrarRacaDaClinicaAsync(gestor, $"{prefixo} Beagle");

            var pagina = await ListarAsync(gestor, $"?busca={prefixo}&ordenarPor=nome&ordem=asc");

            Assert.Equal(new[] { $"{prefixo} Akita", $"{prefixo} Beagle", $"{prefixo} Cocker" }, pagina.Itens.Select(r => r.Nome));
        }

        #endregion

        #region Auxiliares

        private async Task<(ClinicaDeTeste clinica, HttpClient gestor)> ClinicaComGestorAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            return (clinica, await _cenario.ClienteAsync(clinica, Perfis.Gestor));
        }

        private async Task<int> RacaDoSistemaAsync(string? nome = null)
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            return await CadastrarRacaDoSistemaAsync(admin, nome);
        }

        private async Task<(HttpClient gestorA, HttpClient gestorB, int idA)> RacaEmAComGestorDeBAsync()
        {
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var idA = await CadastrarRacaDaClinicaAsync(gestorA, "Raça de A");
            return (gestorA, gestorB, idA);
        }

        private static async Task<GetRacaDto> ObterAsync(HttpClient cliente, int id)
        {
            var resposta = await cliente.GetAsync($"{Rota}/{id}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<GetRacaDto>(resposta);
        }

        private static async Task<PaginaResultado<GetRacaDto>> ListarAsync(HttpClient cliente, string query)
        {
            var resposta = await cliente.GetAsync($"{Rota}{query}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<PaginaResultado<GetRacaDto>>(resposta);
        }

        private static async Task<List<OpcaoDto>> OpcoesAsync(HttpClient cliente, string busca)
        {
            var resposta = await cliente.GetAsync($"{Rota}/opcoes?busca={Uri.EscapeDataString(busca)}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<List<OpcaoDto>>(resposta);
        }

        #endregion
    }
}
