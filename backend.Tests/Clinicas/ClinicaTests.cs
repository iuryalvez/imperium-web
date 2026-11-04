using System.Net;
using System.Net.Http.Json;
using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.DTOs.Dashboard;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Clinicas
{
    /// <summary>
    /// Testes de integração da clínica: dashboard e dados da própria clínica (api/clinica/dashboard e
    /// api/clinica/editar, só GESTOR) e o cadastro de clínicas pelo admin (api/sistema/clinicas).
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class ClinicaTests
    {
        private const string RotaSistema = "/api/sistema/clinicas";

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public ClinicaTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Dashboard

        [Fact]
        public async Task Dashboard_ClinicaComDados_ContaSoOsDaPropriaClinica()
        {
            var clinica = await _cenario.CriarClinicaAsync("Dash");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            await _cenario.ClienteAsync(clinica, Perfis.Veterinario); // 2º vínculo
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            await CadastrarRacaoDaClinicaAsync(gestor);
            var tutorAtivo = await CadastrarTutorAsync(gestor);
            var tutorInativo = await CadastrarTutorAsync(gestor);
            await gestor.PutAsync($"/api/clinica/tutores/{tutorInativo}/status?ativo=false", null);
            var animal1 = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutorAtivo });
            await CadastrarAnimalAsync(gestor, racaId);
            var animalInativo = await CadastrarAnimalAsync(gestor, racaId);
            await gestor.PutAsync($"/api/clinica/animais/{animalInativo}/status?ativo=false", null);
            var consultaComAvaliacao = await CadastrarConsultaAsync(gestor, animal1);
            await CadastrarConsultaAsync(gestor, animal1);
            await RegistrarAvaliacaoComRacaoAsync(_factory, consultaComAvaliacao, animal1, await CadastrarRacaoDaClinicaAsync(gestor));

            // Outra clínica com dados não pode entrar na conta
            var outra = await _cenario.CriarClinicaAsync("Outra");
            var gestorOutra = await _cenario.ClienteAsync(outra, Perfis.Gestor);
            await CadastrarTutorAsync(gestorOutra);
            await CadastrarAnimalAsync(gestorOutra, await CadastrarRacaDaClinicaAsync(gestorOutra));

            var resposta = await gestor.GetAsync("/api/clinica/dashboard");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var dash = (await LerAsync<ResultadoComDados<GetClinicaDashboardDto>>(resposta)).Data;
            Assert.Equal(clinica.Id, dash.Id);
            Assert.Equal(clinica.Nome, dash.NomeClinica);
            Assert.Equal(2, dash.TotalUsuarios);
            Assert.Equal(1, dash.TotalTutores);
            Assert.Equal(1, dash.TotalTutoresInativos);
            Assert.Equal(2, dash.TotalAnimais);
            Assert.Equal(1, dash.TotalAnimaisInativos);
            Assert.Equal(1, dash.TotalRacas);
            Assert.Equal(2, dash.TotalRacoes);
            Assert.Equal(2, dash.TotalConsultas);
            Assert.Equal(1, dash.TotalAvaliacoes);
            Assert.Equal(3, dash.TotalAcompanhamentos);
        }

        [Fact]
        public async Task Dashboard_ClinicaNova_RetornaTudoZeradoMenosOGestor()
        {
            var clinica = await _cenario.CriarClinicaAsync("Vazia");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var dash = (await LerAsync<ResultadoComDados<GetClinicaDashboardDto>>(await gestor.GetAsync("/api/clinica/dashboard"))).Data;

            Assert.Equal(1, dash.TotalUsuarios);
            Assert.Equal(0, dash.TotalAnimais);
            Assert.Equal(0, dash.TotalTutores);
            Assert.Equal(0, dash.TotalConsultas);
        }

        [Theory]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Secretario)]
        [InlineData(Perfis.Tecnico)]
        public async Task Dashboard_PerfilSemGerenciaClinica_RetornaForbidden(string perfil)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.GetAsync("/api/clinica/dashboard")).StatusCode);
        }

        [Fact]
        public async Task Dashboard_AdminSimulandoGestor_RetornaDaClinicaSimulada()
        {
            var clinica = await _cenario.CriarClinicaAsync("Simulada");
            var admin = await _cenario.ClienteAdminSimulandoAsync(clinica, Perfis.Gestor);

            var resposta = await admin.GetAsync("/api/clinica/dashboard");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(clinica.Id, (await LerAsync<ResultadoComDados<GetClinicaDashboardDto>>(resposta)).Data.Id);
        }

        #endregion

        #region Dados da própria clínica

        [Fact]
        public async Task EditarPropria_DadosValidos_AtualizaSoAClinicaDoToken()
        {
            var clinicaA = await _cenario.CriarClinicaAsync("A");
            var clinicaB = await _cenario.CriarClinicaAsync("B");
            var gestorA = await _cenario.ClienteAsync(clinicaA, Perfis.Gestor);
            var gestorB = await _cenario.ClienteAsync(clinicaB, Perfis.Gestor);

            var resposta = await gestorA.PutAsJsonAsync("/api/clinica/editar", new { nomeClinica = "Clínica A Renovada", telefone = "62933334444", id = clinicaB.Id });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var dashA = (await LerAsync<ResultadoComDados<GetClinicaDashboardDto>>(await gestorA.GetAsync("/api/clinica/dashboard"))).Data;
            var dashB = (await LerAsync<ResultadoComDados<GetClinicaDashboardDto>>(await gestorB.GetAsync("/api/clinica/dashboard"))).Data;
            Assert.Equal("Clínica A Renovada", dashA.NomeClinica);
            Assert.Equal("62933334444", dashA.Telefone);
            Assert.Equal(clinicaB.Nome, dashB.NomeClinica);
        }

        [Fact]
        public async Task EditarPropria_SemCamposOpcionais_MantemValoresAtuais()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            await gestor.PutAsJsonAsync("/api/clinica/editar", new { nomeClinica = clinica.Nome, telefone = "62911112222", cidade = "Goiânia", uf = "GO" });

            var resposta = await gestor.PutAsJsonAsync("/api/clinica/editar", new { nomeClinica = "Só o nome" });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var dash = (await LerAsync<ResultadoComDados<GetClinicaDashboardDto>>(await gestor.GetAsync("/api/clinica/dashboard"))).Data;
            Assert.Equal("Só o nome", dash.NomeClinica);
            Assert.Equal("62911112222", dash.Telefone);
            Assert.Equal("Goiânia", dash.Cidade);
            Assert.Equal("GO", dash.Uf);
            Assert.True(dash.IsAtiva);
        }

        [Fact]
        public async Task EditarPropria_CnpjDeOutraClinica_RetornaConflict()
        {
            var cnpj = "11222333000181";
            var clinicaA = await _cenario.CriarClinicaAsync("A");
            var clinicaB = await _cenario.CriarClinicaAsync("B");
            var gestorA = await _cenario.ClienteAsync(clinicaA, Perfis.Gestor);
            var gestorB = await _cenario.ClienteAsync(clinicaB, Perfis.Gestor);
            await LimparCnpjAsync(cnpj);
            await gestorA.PutAsJsonAsync("/api/clinica/editar", new { nomeClinica = clinicaA.Nome, cnpj });

            var resposta = await gestorB.PutAsJsonAsync("/api/clinica/editar", new { nomeClinica = clinicaB.Nome, cnpj });

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("Este CNPJ já está em uso por outra clínica.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task EditarPropria_SemNome_RetornaBadRequestComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PutAsJsonAsync("/api/clinica/editar", new { telefone = "62900000000" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome da clínica é obrigatório.", await TextoAsync(resposta));
        }

        [Theory]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Secretario)]
        public async Task EditarPropria_PerfilSemGerenciaClinica_RetornaForbidden(string perfil)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PutAsJsonAsync("/api/clinica/editar", new { nomeClinica = "Invasão" })).StatusCode);
        }

        #endregion

        #region Clínicas pelo admin

        [Fact]
        public async Task CadastrarClinica_Admin_RetornaCreated()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var nome = $"Nova {Cenario.Unico()}";

            var resposta = await admin.PostAsJsonAsync($"{RotaSistema}/cadastrar", new { nomeClinica = nome, isAtiva = true });

            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            var criada = await LerAsync<GetClinicaDto>(resposta);
            Assert.Equal(nome, (await LerAsync<GetClinicaDto>(await admin.GetAsync($"{RotaSistema}/{criada.Id}"))).NomeClinica);
        }

        [Fact]
        public async Task EditarClinica_AdminSemIsAtiva_MantemClinicaAtiva()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinica = await _cenario.CriarClinicaAsync("A");

            var resposta = await admin.PutAsJsonAsync($"{RotaSistema}/editar", new { id = clinica.Id, nomeClinica = "Renomeada pelo admin" });

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            var depois = await LerAsync<GetClinicaDto>(await admin.GetAsync($"{RotaSistema}/{clinica.Id}"));
            Assert.Equal("Renomeada pelo admin", depois.NomeClinica);
            Assert.True(depois.IsAtiva);
        }

        [Fact]
        public async Task EditarClinica_AdminCnpjDeOutraClinica_RetornaConflict()
        {
            var cnpj = "45723174000110";
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinicaA = await _cenario.CriarClinicaAsync("A");
            var clinicaB = await _cenario.CriarClinicaAsync("B");
            await LimparCnpjAsync(cnpj);
            await admin.PutAsJsonAsync($"{RotaSistema}/editar", new { id = clinicaA.Id, nomeClinica = clinicaA.Nome, cnpj, isAtiva = true });

            var resposta = await admin.PutAsJsonAsync($"{RotaSistema}/editar", new { id = clinicaB.Id, nomeClinica = clinicaB.Nome, cnpj, isAtiva = true });

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
        }

        [Fact]
        public async Task ExcluirClinica_ComUsuariosVinculados_RetornaConflictENaoRemove()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinica = await _cenario.CriarClinicaAsync("A");
            await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await admin.DeleteAsync($"{RotaSistema}/{clinica.Id}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("Clínica possui usuários vinculados.", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync($"{RotaSistema}/{clinica.Id}")).StatusCode);
        }

        [Fact]
        public async Task ExcluirClinica_SemVinculos_RetornaNoContent()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinica = await _cenario.CriarClinicaAsync("A");

            Assert.Equal(HttpStatusCode.NoContent, (await admin.DeleteAsync($"{RotaSistema}/{clinica.Id}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{RotaSistema}/{clinica.Id}")).StatusCode);
        }

        [Fact]
        public async Task RotasComIdDeClinica_Inexistente_RetornaNotFound()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            const int inexistente = 999999999;

            Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync($"{RotaSistema}/{inexistente}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.PutAsJsonAsync($"{RotaSistema}/editar", new { id = inexistente, nomeClinica = "X", isAtiva = true })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.DeleteAsync($"{RotaSistema}/{inexistente}")).StatusCode);
        }

        [Fact]
        public async Task RotasDoSistema_GestorDeClinica_RetornaForbidden()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            Assert.Equal(HttpStatusCode.Forbidden, (await gestor.GetAsync(RotaSistema)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await gestor.DeleteAsync($"{RotaSistema}/{clinica.Id}")).StatusCode);
        }

        #endregion

        /// <summary>O CNPJ é único entre todas as clínicas do banco de teste: tira o valor de execuções anteriores do mesmo teste.</summary>
        private async Task LimparCnpjAsync(string cnpj)
        {
            await using var contexto = _factory.CriarContexto();
            foreach (var c in contexto.Clinicas.Where(c => c.Cnpj == cnpj)) c.Cnpj = null;
            await contexto.SaveChangesAsync();
        }
    }
}
