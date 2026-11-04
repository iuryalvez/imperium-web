using System.Net;
using System.Net.Http.Json;
using backend.Domain.SaudeAnimal.DTOs.Racao;
using backend.Domain.Utils.Paginacao;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Racoes
{
    /// <summary>
    /// Testes de integração das rotas api/clinica/racoes: rações próprias e catálogo do sistema (cópia ao editar,
    /// ocultar só para a clínica), recálculo da energia, isolamento, perfis, validação, exclusão bloqueada por uso
    /// e paginação.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class RacaoTests
    {
        private const string Rota = "/api/clinica/racoes";

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public RacaoTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Caminho feliz

        [Fact]
        public async Task Cadastrar_DadosValidos_RetornaOkECalculaEnergia()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovaRacao("Premium Adulto"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var racao = await ObterAsync(gestor, await LerIdDoDataAsync(resposta, "id"));
            Assert.Equal("Premium Adulto", racao.Nome);
            Assert.Equal(clinica.Id, racao.ClinicaId);
            Assert.False(racao.IsSistema);
            Assert.True(racao.IsAtivo);
            Assert.Equal(90.0, racao.MateriaSeca);
            Assert.Equal(41.0, racao.ExtrativoNaoNitrogenado);
            // NRC (2006), cão: EB = 5,7·PB + 9,4·EE + 4,1·(ENN + FB); CDE = 91,2 − 1,43·FB(MS); EM = ED − 1,04·PB
            Assert.Equal(4.545, racao.EnergiaBruta);
            Assert.Equal(86.433, racao.CoeficienteDigestibilidadeEnergia);
            Assert.Equal(3.668, racao.EnergiaMetabolizavel);
        }

        [Fact]
        public async Task Editar_RacaoPropria_AtualizaERecalculaSemCriarCopia()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaoDaClinicaAsync(gestor);
            var antes = await ObterAsync(gestor, id);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Light", isPorcentagem = true, umidade = 20.0 });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(id, await LerIdDoDataAsync(resposta, "id"));
            var depois = await ObterAsync(gestor, id);
            Assert.Equal("Light", depois.Nome);
            Assert.Equal(80.0, depois.MateriaSeca);
            Assert.NotEqual(antes.EnergiaMetabolizavel, depois.EnergiaMetabolizavel);
        }

        [Fact]
        public async Task MudarStatus_RacaoPropria_DesativaEReativa()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaoDaClinicaAsync(gestor);

            Assert.Equal(HttpStatusCode.OK, (await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null)).StatusCode);
            Assert.False((await ObterAsync(gestor, id)).IsAtivo);

            Assert.Equal(HttpStatusCode.OK, (await gestor.PutAsync($"{Rota}/{id}/status?ativo=true", null)).StatusCode);
            Assert.True((await ObterAsync(gestor, id)).IsAtivo);
        }

        [Fact]
        public async Task Excluir_RacaoPropriaSemUso_RetornaOkERemove()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaoDaClinicaAsync(gestor);

            var resposta = await gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region Edição parcial

        [Fact]
        public async Task Editar_SoONome_MantemComposicaoEUnidade()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaoDaClinicaAsync(gestor);
            var antes = await ObterAsync(gestor, id);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Só o nome mudou" });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var depois = await ObterAsync(gestor, id);
            Assert.Equal("Só o nome mudou", depois.Nome);
            Assert.Equal(antes.IsPorcentagem, depois.IsPorcentagem);
            Assert.Equal(antes.Umidade, depois.Umidade);
            Assert.Equal(antes.ProteinaBruta, depois.ProteinaBruta);
            Assert.Equal(antes.ExtratoEtereo, depois.ExtratoEtereo);
            Assert.Equal(antes.MateriaMineral, depois.MateriaMineral);
            Assert.Equal(antes.FibraBruta, depois.FibraBruta);
            Assert.Equal(antes.Especie, depois.Especie);
            Assert.Equal(antes.EnergiaMetabolizavel, depois.EnergiaMetabolizavel);
        }

        #endregion

        #region Catálogo do sistema

        [Fact]
        public async Task Editar_RacaoDoSistema_CriaCopiaDaClinicaEOriginalNaoMuda()
        {
            var nomeOriginal = $"Original {Cenario.Unico()}";
            var racaoSistema = await RacaoDoSistemaAsync(nomeOriginal);
            var (clinicaA, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var original = await ObterAsync(gestorA, racaoSistema);

            var resposta = await gestorA.PutAsJsonAsync($"{Rota}/editar", new { id = racaoSistema, nome = "Versão de A", isPorcentagem = true, proteinaBruta = 30.0 });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var idCopia = await LerIdDoDataAsync(resposta, "id");
            Assert.NotEqual(racaoSistema, idCopia);
            var copia = await ObterAsync(gestorA, idCopia);
            Assert.Equal(clinicaA.Id, copia.ClinicaId);
            Assert.Equal("Versão de A", copia.Nome);
            Assert.Equal(30.0, copia.ProteinaBruta);
            Assert.Equal(original.Umidade, copia.Umidade);
            Assert.Equal(original.FibraBruta, copia.FibraBruta);
            var sistemaDepois = await ObterAsync(gestorB, racaoSistema);
            Assert.Equal(nomeOriginal, sistemaDepois.Nome);
            Assert.Equal(original.ProteinaBruta, sistemaDepois.ProteinaBruta);
            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.GetAsync($"{Rota}/{idCopia}")).StatusCode);
        }

        [Fact]
        public async Task Excluir_RacaoDoSistema_RetornaBadRequestENaoRemove()
        {
            var racaoSistema = await RacaoDoSistemaAsync();
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.DeleteAsync($"{Rota}/{racaoSistema}");

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Rações do sistema não podem ser excluídas pela clínica.", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await gestor.GetAsync($"{Rota}/{racaoSistema}")).StatusCode);
        }

        [Fact]
        public async Task MudarStatus_RacaoDoSistema_RetornaBadRequestENaoAltera()
        {
            var racaoSistema = await RacaoDoSistemaAsync();
            var (_, gestor) = await ClinicaComGestorAsync();

            Assert.Equal(HttpStatusCode.BadRequest, (await gestor.PutAsync($"{Rota}/{racaoSistema}/status?ativo=false", null)).StatusCode);
            Assert.True((await ObterAsync(gestor, racaoSistema)).IsAtivo);
        }

        [Fact]
        public async Task Ocultar_RacaoDoSistema_TiraDasOpcoesSoDaClinica()
        {
            var nome = $"Oculta {Cenario.Unico()}";
            var racaoSistema = await RacaoDoSistemaAsync(nome);
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();

            var resposta = await gestorA.PutAsync($"{Rota}/{racaoSistema}/ocultar?oculta=true", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.DoesNotContain(await OpcoesAsync(gestorA, nome), o => o.Id == racaoSistema);
            Assert.Contains(await OpcoesAsync(gestorB, nome), o => o.Id == racaoSistema);
            Assert.True((await ObterAsync(gestorA, racaoSistema)).IsOculta);
        }

        [Fact]
        public async Task Ocultar_RacaoPropria_RetornaBadRequest()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaoDaClinicaAsync(gestor);

            var resposta = await gestor.PutAsync($"{Rota}/{id}/ocultar?oculta=true", null);

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Só rações do sistema podem ser ocultadas.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Excluir_RacaoEmUsoPorAvaliacao_RetornaConflictENaoRemove()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaoDaClinicaAsync(gestor);
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            var animalId = await CadastrarAnimalAsync(gestor, racaId);
            var consultaId = await CadastrarConsultaAsync(gestor, animalId);
            await RegistrarAvaliacaoComRacaoAsync(_factory, consultaId, animalId, id);

            var resposta = await gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("Esta ração está em uso", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        [Fact]
        public async Task Opcoes_RacaoInativa_VemComIsAtivoFalse()
        {
            var nome = $"Inativa {Cenario.Unico()}";
            var (_, gestor) = await ClinicaComGestorAsync();
            var id = await CadastrarRacaoDaClinicaAsync(gestor, nome);
            await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null);

            var opcao = Assert.Single(await OpcoesAsync(gestor, nome));

            Assert.Equal(id, opcao.Id);
            Assert.False(opcao.IsAtivo);
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task RotasComId_RacaoDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var idA = await CadastrarRacaoDaClinicaAsync(gestorA, "Ração de A");

            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.GetAsync($"{Rota}/{idA}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.PutAsJsonAsync($"{Rota}/editar", new { id = idA, nome = "Alterada por B", isPorcentagem = true })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.PutAsync($"{Rota}/{idA}/status?ativo=false", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.PutAsync($"{Rota}/{idA}/ocultar?oculta=true", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.DeleteAsync($"{Rota}/{idA}")).StatusCode);

            var racao = await ObterAsync(gestorA, idA);
            Assert.Equal("Ração de A", racao.Nome);
            Assert.True(racao.IsAtivo);
        }

        [Fact]
        public async Task ListarEOpcoes_OutraClinica_NaoMostraRacaoDeA()
        {
            var nome = $"Só de A {Cenario.Unico()}";
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var idA = await CadastrarRacaoDaClinicaAsync(gestorA, nome);

            Assert.Equal(0, (await ListarAsync(gestorB, "?origem=clinica")).Total);
            Assert.DoesNotContain(await OpcoesAsync(gestorB, nome), o => o.Id == idA);
        }

        #endregion

        #region Perfis

        [Theory]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Tecnico)]
        public async Task Cadastrar_PerfilComGerenciaRacoes_RetornaOk(string perfil)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            Assert.Equal(HttpStatusCode.OK, (await cliente.PostAsJsonAsync($"{Rota}/cadastrar", NovaRacao())).StatusCode);
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
            var id = await CadastrarRacaoDaClinicaAsync(gestor, "Protegida");

            var requisicao = new HttpRequestMessage(new HttpMethod(metodo), $"{Rota}/{caminho.Replace("{id}", id.ToString())}");
            if (caminho == "cadastrar") requisicao.Content = JsonContent.Create(NovaRacao());
            if (caminho == "editar") requisicao.Content = JsonContent.Create(new { id, nome = "Alterada", isPorcentagem = true });

            var resposta = await secretario.SendAsync(requisicao);

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
            var racao = await ObterAsync(gestor, id);
            Assert.Equal("Protegida", racao.Nome);
            Assert.True(racao.IsAtivo);
        }

        [Fact]
        public async Task Leitura_PerfilSecretario_RetornaOk()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var secretario = await _cenario.ClienteAsync(clinica, Perfis.Secretario);
            var id = await CadastrarRacaoDaClinicaAsync(gestor);

            Assert.Equal(HttpStatusCode.OK, (await secretario.GetAsync(Rota)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await secretario.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region Validação

        [Fact]
        public async Task Cadastrar_SemNome_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar",
                new { especie = Especie.Cao, isPorcentagem = true, umidade = 10.0, proteinaBruta = 25.0, extratoEtereo = 14.0, materiaMineral = 7.0, fibraBruta = 3.0 });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome da ração é obrigatório.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_EspecieInvalida_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovaRacao(especie: 99));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A espécie é inválida.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task RotasComId_RacaoInexistente_RetornaNotFound()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            const int inexistente = 999999999;

            Assert.Equal(HttpStatusCode.NotFound, (await gestor.GetAsync($"{Rota}/{inexistente}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.PutAsJsonAsync($"{Rota}/editar", new { id = inexistente, nome = "X", isPorcentagem = true })).StatusCode);
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
            for (var i = 1; i <= 12; i++) ids.Add(await CadastrarRacaoDaClinicaAsync(gestor, $"Ração {i:00}"));

            var pagina = await ListarAsync(gestor, "?origem=clinica");

            Assert.Equal(12, pagina.Total);
            Assert.Equal(10, pagina.TamanhoPagina);
            Assert.Equal(ids.AsEnumerable().Reverse().Take(10), pagina.Itens.Select(r => r.Id));
        }

        [Fact]
        public async Task Listar_TamanhoPaginaAcimaDoMaximo_LimitaA100()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            Assert.Equal(100, (await ListarAsync(gestor, "?tamanhoPagina=999")).TamanhoPagina);
        }

        [Fact]
        public async Task Listar_BuscaEOrdenacaoPorNome_RetornaEmOrdemAlfabetica()
        {
            var prefixo = $"Busca{Cenario.Unico()}";
            var (_, gestor) = await ClinicaComGestorAsync();
            await CadastrarRacaoDaClinicaAsync(gestor, $"{prefixo} Senior");
            await CadastrarRacaoDaClinicaAsync(gestor, $"{prefixo} Filhote");
            await CadastrarRacaoDaClinicaAsync(gestor, $"{prefixo} Light");

            var pagina = await ListarAsync(gestor, $"?busca={prefixo}&ordenarPor=nome&ordem=asc");

            Assert.Equal(new[] { $"{prefixo} Filhote", $"{prefixo} Light", $"{prefixo} Senior" }, pagina.Itens.Select(r => r.Nome));
        }

        [Fact]
        public async Task Listar_FiltroEspecie_RetornaSoAEspecie()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            await CadastrarRacaoDaClinicaAsync(gestor, especie: Especie.Cao);
            var gato = await CadastrarRacaoDaClinicaAsync(gestor, especie: Especie.Gato);

            var pagina = await ListarAsync(gestor, $"?origem=clinica&especie={Especie.Gato}");

            Assert.Equal(gato, Assert.Single(pagina.Itens).Id);
        }

        #endregion

        #region Auxiliares

        private async Task<(ClinicaDeTeste clinica, HttpClient gestor)> ClinicaComGestorAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            return (clinica, await _cenario.ClienteAsync(clinica, Perfis.Gestor));
        }

        private async Task<int> RacaoDoSistemaAsync(string? nome = null)
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            return await CadastrarRacaoDoSistemaAsync(admin, nome);
        }

        private static async Task<GetRacoesDto> ObterAsync(HttpClient cliente, int id)
        {
            var resposta = await cliente.GetAsync($"{Rota}/{id}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<GetRacoesDto>(resposta);
        }

        private static async Task<PaginaResultado<GetRacoesDto>> ListarAsync(HttpClient cliente, string query)
        {
            var resposta = await cliente.GetAsync($"{Rota}{query}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<PaginaResultado<GetRacoesDto>>(resposta);
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
