using System.Net;
using System.Net.Http.Json;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Animais
{
    /// <summary>
    /// Testes de integração das rotas api/clinica/animais: caminho feliz, isolamento entre clínicas, perfis,
    /// validação, raça e tutor inativos/ocultos, exclusão bloqueada por consulta, edição parcial e paginação.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class AnimalTests
    {
        private const string Rota = "/api/clinica/animais";

        private readonly Cenario _cenario;

        public AnimalTests(ImperiumApiFactory factory)
        {
            _cenario = new Cenario(factory);
        }

        #region Caminho feliz

        [Fact]
        public async Task Cadastrar_DadosValidos_RetornaOkECriaAcompanhamento()
        {
            var (clinica, gestor, racaId) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaId, Especie.Cao, "Rex"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var animal = await ObterAsync(gestor, await LerIdDoDataAsync(resposta, "animalId"));
            Assert.Equal("Rex", animal.Nome);
            Assert.Equal(racaId, animal.RacaId);
            Assert.True(animal.IsAtivo);
            Assert.True(animal.AcompanhamentoId > 0);
            Assert.Equal(Assert.Single((await ListarAsync(gestor, "")).Itens).AcompanhamentoId, animal.AcompanhamentoId);
        }

        [Fact]
        public async Task Cadastrar_ComTutoresDaClinica_VinculaTutores()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var tutor1 = await CadastrarTutorAsync(gestor);
            var tutor2 = await CadastrarTutorAsync(gestor);

            var id = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutor1, tutor2 });

            var animal = await ObterAsync(gestor, id);
            Assert.Equal(new[] { tutor1, tutor2 }.OrderBy(x => x), animal.TutorIds.OrderBy(x => x));
        }

        [Fact]
        public async Task ObterPorId_AnimalInexistente_RetornaNotFound()
        {
            var (_, gestor, _) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.GetAsync($"{Rota}/999999999");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("não encontrado", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Editar_DadosValidos_RetornaOkEAtualiza()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var id = await CadastrarAnimalAsync(gestor, racaId, nome: "Nome Antigo");

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Nome Novo", sexo = 2, isCastrado = true });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var animal = await ObterAsync(gestor, id);
            Assert.Equal("Nome Novo", animal.Nome);
            Assert.Equal(2, (int)animal.Sexo);
            Assert.True(animal.IsCastrado);
        }

        [Fact]
        public async Task Editar_SoONome_MantemDemaisCampos()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var tutorId = await CadastrarTutorAsync(gestor);
            var id = await CadastrarAnimalAsync(gestor, racaId, Especie.Cao, "Parcial", new[] { tutorId });
            var antes = await ObterAsync(gestor, id);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Parcial Editado" });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var depois = await ObterAsync(gestor, id);
            Assert.Equal("Parcial Editado", depois.Nome);
            Assert.Equal(antes.RacaId, depois.RacaId);
            Assert.Equal(antes.Especie, depois.Especie);
            Assert.Equal(antes.Sexo, depois.Sexo);
            Assert.Equal(antes.Nascimento, depois.Nascimento);
            Assert.Equal(antes.IsCastrado, depois.IsCastrado);
            Assert.Equal(antes.TutorIds, depois.TutorIds);
            Assert.True(depois.IsAtivo);
        }

        [Fact]
        public async Task MudarStatus_Desativar_MarcaInativo()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var id = await CadastrarAnimalAsync(gestor, racaId);

            var resposta = await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.False((await ObterAsync(gestor, id)).IsAtivo);
        }

        [Fact]
        public async Task MudarStatus_Reativar_MarcaAtivo()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var id = await CadastrarAnimalAsync(gestor, racaId);
            await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null);

            var resposta = await gestor.PutAsync($"{Rota}/{id}/status?ativo=true", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.True((await ObterAsync(gestor, id)).IsAtivo);
        }

        [Fact]
        public async Task Excluir_SemConsultas_RetornaOkERemove()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var id = await CadastrarAnimalAsync(gestor, racaId);

            var resposta = await gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region Regras de negócio

        [Fact]
        public async Task Excluir_ComConsultas_RetornaConflictENaoRemove()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var id = await CadastrarAnimalAsync(gestor, racaId);
            await CadastrarConsultaAsync(gestor, id);

            var resposta = await gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("possui consultas registradas", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        [Fact]
        public async Task Cadastrar_RacaInativa_RetornaBadRequest()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            await gestor.PutAsync($"/api/clinica/racas/{racaId}/status?ativo=false", null);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaId));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A raça selecionada está inativa.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_RacaDoSistemaOcultaPelaClinica_RetornaBadRequest()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var racaSistema = await CadastrarRacaDoSistemaAsync(admin);
            var (_, gestor, _) = await ClinicaComGestorERacaAsync();
            await gestor.PutAsync($"/api/clinica/racas/{racaSistema}/ocultar?oculta=true", null);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaSistema));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A raça selecionada está oculta para esta clínica.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_RacaDoSistemaOcultaPorOutraClinica_RetornaOk()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var racaSistema = await CadastrarRacaDoSistemaAsync(admin);
            var (_, gestorA, _) = await ClinicaComGestorERacaAsync();
            var (_, gestorB, _) = await ClinicaComGestorERacaAsync();
            await gestorB.PutAsync($"/api/clinica/racas/{racaSistema}/ocultar?oculta=true", null);

            var resposta = await gestorA.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaSistema));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task Cadastrar_RacaDeOutraClinica_RetornaBadRequest()
        {
            var (_, gestorA, _) = await ClinicaComGestorERacaAsync();
            var (_, _, racaDeB) = await ClinicaComGestorERacaAsync();

            var resposta = await gestorA.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaDeB));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A raça selecionada não existe ou não pertence a esta clínica.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Editar_MantendoRacaQueFicouInativa_RetornaOk()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var id = await CadastrarAnimalAsync(gestor, racaId);
            await gestor.PutAsync($"/api/clinica/racas/{racaId}/status?ativo=false", null);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Mantém a raça", racaId });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(racaId, (await ObterAsync(gestor, id)).RacaId);
        }

        [Fact]
        public async Task Editar_TrocandoParaRacaInativa_RetornaBadRequest()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var outraRaca = await CadastrarRacaDaClinicaAsync(gestor);
            var id = await CadastrarAnimalAsync(gestor, racaId);
            await gestor.PutAsync($"/api/clinica/racas/{outraRaca}/status?ativo=false", null);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, racaId = outraRaca });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Equal(racaId, (await ObterAsync(gestor, id)).RacaId);
        }

        [Fact]
        public async Task Cadastrar_RacaDeOutraEspecie_RetornaBadRequest()
        {
            var (_, gestor, racaCao) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaCao, Especie.Gato));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A raça escolhida não pertence à espécie do animal.", await TextoAsync(resposta));
            Assert.Equal(0, (await ListarAsync(gestor, "")).Total);
        }

        [Fact]
        public async Task Cadastrar_RacaDaMesmaEspecie_RetornaOk()
        {
            var (_, gestor, _) = await ClinicaComGestorERacaAsync();
            var racaGato = await CadastrarRacaDaClinicaAsync(gestor, especie: Especie.Gato);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaGato, Especie.Gato));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task Editar_TrocandoParaRacaDeOutraEspecie_RetornaBadRequestENaoAltera()
        {
            var (_, gestor, racaCao) = await ClinicaComGestorERacaAsync();
            var racaGato = await CadastrarRacaDaClinicaAsync(gestor, especie: Especie.Gato);
            var id = await CadastrarAnimalAsync(gestor, racaCao, Especie.Cao);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, racaId = racaGato });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A raça escolhida não pertence à espécie do animal.", await TextoAsync(resposta));
            Assert.Equal(racaCao, (await ObterAsync(gestor, id)).RacaId);
        }

        [Fact]
        public async Task Editar_TrocandoSoAEspecieSemTrocarARaca_RetornaBadRequestENaoAltera()
        {
            var (_, gestor, racaCao) = await ClinicaComGestorERacaAsync();
            var id = await CadastrarAnimalAsync(gestor, racaCao, Especie.Cao);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, especie = Especie.Gato });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Equal(Especie.Cao, (int)(await ObterAsync(gestor, id)).Especie);
        }

        [Fact]
        public async Task Editar_TrocandoEspecieERacaJuntasParaOutraEspecie_RetornaOk()
        {
            var (_, gestor, racaCao) = await ClinicaComGestorERacaAsync();
            var racaGato = await CadastrarRacaDaClinicaAsync(gestor, especie: Especie.Gato);
            var id = await CadastrarAnimalAsync(gestor, racaCao, Especie.Cao);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, especie = Especie.Gato, racaId = racaGato });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var animal = await ObterAsync(gestor, id);
            Assert.Equal(Especie.Gato, (int)animal.Especie);
            Assert.Equal(racaGato, animal.RacaId);
        }

        [Fact]
        public async Task Cadastrar_TutorInativo_RetornaBadRequest()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var tutorId = await CadastrarTutorAsync(gestor);
            await gestor.PutAsync($"/api/clinica/tutores/{tutorId}/status?ativo=false", null);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaId, tutorIds: new[] { tutorId }));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Um ou mais tutores selecionados estão inativos.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Editar_MantendoTutorQueFicouInativo_RetornaOk()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var tutorId = await CadastrarTutorAsync(gestor);
            var id = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutorId });
            await gestor.PutAsync($"/api/clinica/tutores/{tutorId}/status?ativo=false", null);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, nome = "Mantém o tutor", tutorIds = new[] { tutorId } });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(new[] { tutorId }, (await ObterAsync(gestor, id)).TutorIds);
        }

        [Fact]
        public async Task Cadastrar_TutorDeOutraClinica_RetornaBadRequest()
        {
            var (_, gestorA, racaId) = await ClinicaComGestorERacaAsync();
            var (_, gestorB, _) = await ClinicaComGestorERacaAsync();
            var tutorDeB = await CadastrarTutorAsync(gestorB);

            var resposta = await gestorA.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaId, tutorIds: new[] { tutorDeB }));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("não pertencem a esta clínica", await TextoAsync(resposta));
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task ObterPorId_AnimalDeOutraClinica_RetornaNotFound()
        {
            var (_, gestorB, idA, _) = await AnimalEmAComGestorDeBAsync();

            var resposta = await gestorB.GetAsync($"{Rota}/{idA}");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Listar_OutraClinica_NaoMostraAnimalDeA()
        {
            var (_, gestorB, idA, _) = await AnimalEmAComGestorDeBAsync();

            var pagina = await ListarAsync(gestorB, "");

            Assert.DoesNotContain(pagina.Itens, a => a.Id == idA);
            Assert.Equal(0, pagina.Total);
        }

        [Fact]
        public async Task Opcoes_OutraClinica_NaoMostraAnimalDeA()
        {
            var (_, gestorB, idA, _) = await AnimalEmAComGestorDeBAsync();

            var opcoes = await LerAsync<List<OpcaoDto>>(await gestorB.GetAsync($"{Rota}/opcoes"));

            Assert.DoesNotContain(opcoes, o => o.Id == idA);
        }

        [Fact]
        public async Task Editar_AnimalDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (gestorA, gestorB, idA, _) = await AnimalEmAComGestorDeBAsync();

            var resposta = await gestorB.PutAsJsonAsync($"{Rota}/editar", new { id = idA, nome = "Alterado por B" });

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Equal("Animal de A", (await ObterAsync(gestorA, idA)).Nome);
        }

        [Fact]
        public async Task MudarStatus_AnimalDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (gestorA, gestorB, idA, _) = await AnimalEmAComGestorDeBAsync();

            var resposta = await gestorB.PutAsync($"{Rota}/{idA}/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.True((await ObterAsync(gestorA, idA)).IsAtivo);
        }

        [Fact]
        public async Task Excluir_AnimalDeOutraClinica_RetornaNotFoundENaoRemove()
        {
            var (gestorA, gestorB, idA, _) = await AnimalEmAComGestorDeBAsync();

            var resposta = await gestorB.DeleteAsync($"{Rota}/{idA}");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await gestorA.GetAsync($"{Rota}/{idA}")).StatusCode);
        }

        [Fact]
        public async Task Cadastrar_ComClinicaIdDeOutraClinicaNoCorpo_UsaClinicaDoToken()
        {
            var (_, gestorA, racaId) = await ClinicaComGestorERacaAsync();
            var (clinicaB, gestorB, _) = await ClinicaComGestorERacaAsync();
            var corpo = new { nome = "Intruso", racaId, especie = Especie.Cao, sexo = 1, isCastrado = false, nascimento = "2021-01-01T00:00:00Z", clinicaId = clinicaB.Id };

            var resposta = await gestorA.PostAsJsonAsync($"{Rota}/cadastrar", corpo);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var id = await LerIdDoDataAsync(resposta, "animalId");
            Assert.Equal(HttpStatusCode.OK, (await gestorA.GetAsync($"{Rota}/{id}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestorB.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region Perfis

        [Theory]
        [InlineData(Perfis.Gestor)]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Tecnico)]
        public async Task Cadastrar_PerfilComGerenciaAnimais_RetornaOk(string perfil)
        {
            var (clinica, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            var resposta = await cliente.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaId));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Theory]
        [InlineData("POST", "cadastrar")]
        [InlineData("PUT", "editar")]
        [InlineData("PUT", "{id}/status?ativo=false")]
        [InlineData("DELETE", "{id}")]
        public async Task RotasDeEscrita_PerfilSecretario_RetornaForbidden(string metodo, string caminho)
        {
            var (clinica, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var secretario = await _cenario.ClienteAsync(clinica, Perfis.Secretario);
            var id = await CadastrarAnimalAsync(gestor, racaId, nome: "Protegido");

            var requisicao = new HttpRequestMessage(new HttpMethod(metodo), $"{Rota}/{caminho.Replace("{id}", id.ToString())}");
            if (caminho == "cadastrar") requisicao.Content = JsonContent.Create(NovoAnimal(racaId));
            if (caminho == "editar") requisicao.Content = JsonContent.Create(new { id, nome = "Alterado" });

            var resposta = await secretario.SendAsync(requisicao);

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
            var animal = await ObterAsync(gestor, id);
            Assert.Equal("Protegido", animal.Nome);
            Assert.True(animal.IsAtivo);
        }

        [Fact]
        public async Task Leitura_PerfilSecretario_RetornaOk()
        {
            var (clinica, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var secretario = await _cenario.ClienteAsync(clinica, Perfis.Secretario);
            var id = await CadastrarAnimalAsync(gestor, racaId);

            Assert.Equal(HttpStatusCode.OK, (await secretario.GetAsync(Rota)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await secretario.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        [Fact]
        public async Task Listar_SemToken_RetornaUnauthorized()
        {
            var resposta = await _cenario.ClienteAnonimo().GetAsync(Rota);

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        #endregion

        #region Validação

        [Fact]
        public async Task Cadastrar_SemNome_RetornaBadRequestComMensagem()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar",
                new { racaId, especie = Especie.Cao, sexo = 1, isCastrado = false, nascimento = "2021-01-01T00:00:00Z" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome do animal é obrigatório.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_EspecieInvalida_RetornaBadRequestComMensagem()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoAnimal(racaId, especie: 99));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A espécie do animal é inválida.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Editar_AnimalInexistente_RetornaNotFound()
        {
            var (_, gestor, _) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id = 999999999, nome = "Ninguém" });

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task MudarStatus_AnimalInexistente_RetornaNotFound()
        {
            var (_, gestor, _) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.PutAsync($"{Rota}/999999999/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Excluir_AnimalInexistente_RetornaNotFound()
        {
            var (_, gestor, _) = await ClinicaComGestorERacaAsync();

            var resposta = await gestor.DeleteAsync($"{Rota}/999999999");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        #endregion

        #region Paginação

        [Fact]
        public async Task Listar_SemParametros_RetornaPrimeiraPaginaCom10MaisRecentesPrimeiro()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var ids = new List<int>();
            for (var i = 1; i <= 12; i++) ids.Add(await CadastrarAnimalAsync(gestor, racaId, nome: $"Animal {i:00}"));

            var pagina = await ListarAsync(gestor, "");

            Assert.Equal(12, pagina.Total);
            Assert.Equal(10, pagina.TamanhoPagina);
            Assert.Equal(ids.AsEnumerable().Reverse().Take(10), pagina.Itens.Select(a => a.Id));
        }

        [Fact]
        public async Task Listar_TamanhoPaginaAcimaDoMaximo_LimitaA100()
        {
            var (_, gestor, _) = await ClinicaComGestorERacaAsync();

            Assert.Equal(100, (await ListarAsync(gestor, "?tamanhoPagina=1000")).TamanhoPagina);
        }

        [Fact]
        public async Task Listar_BuscaPorNomeDoTutor_RetornaAnimaisDoTutor()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var tutorId = await CadastrarTutorAsync(gestor, nome: "Hermengarda Teixeira");
            var idDoTutor = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutorId });
            await CadastrarAnimalAsync(gestor, racaId);

            var pagina = await ListarAsync(gestor, "?busca=hermengarda");

            Assert.Equal(idDoTutor, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Listar_FiltroEspecie_RetornaSoAEspecie()
        {
            var (_, gestor, racaCao) = await ClinicaComGestorERacaAsync();
            var racaGato = await CadastrarRacaDaClinicaAsync(gestor, especie: Especie.Gato);
            await CadastrarAnimalAsync(gestor, racaCao, Especie.Cao);
            var gato = await CadastrarAnimalAsync(gestor, racaGato, Especie.Gato);

            var pagina = await ListarAsync(gestor, $"?especie={Especie.Gato}");

            Assert.Equal(gato, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Listar_OrdenarPorNomeAsc_RetornaEmOrdemAlfabetica()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            await CadastrarAnimalAsync(gestor, racaId, nome: "Toby");
            await CadastrarAnimalAsync(gestor, racaId, nome: "Amora");
            await CadastrarAnimalAsync(gestor, racaId, nome: "Luna");

            var pagina = await ListarAsync(gestor, "?ordenarPor=nome&ordem=asc");

            Assert.Equal(new[] { "Amora", "Luna", "Toby" }, pagina.Itens.Select(a => a.Nome));
        }

        [Fact]
        public async Task Listar_FiltroAtivoFalse_RetornaSoInativos()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();
            var inativo = await CadastrarAnimalAsync(gestor, racaId);
            await CadastrarAnimalAsync(gestor, racaId);
            await gestor.PutAsync($"{Rota}/{inativo}/status?ativo=false", null);

            var pagina = await ListarAsync(gestor, "?ativo=false");

            Assert.Equal(inativo, Assert.Single(pagina.Itens).Id);
        }

        #endregion

        #region Campos de apoio

        [Fact]
        public async Task Listar_AnimalComRacaETutores_TrazNomeDaRacaETutores()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var nomeRaca = $"Raça {Cenario.Unico()}";
            var racaId = await CadastrarRacaDaClinicaAsync(gestor, nomeRaca);
            var nomeTutor = $"Tutor {Cenario.Unico()}";
            var tutorId = await CadastrarTutorAsync(gestor, nomeTutor);
            var antes = DateTime.UtcNow.AddSeconds(-5);

            var id = await CadastrarAnimalAsync(gestor, racaId, nome: "Bolinha", tutorIds: new[] { tutorId });

            var animal = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.Equal(id, animal.Id);
            Assert.Equal("Bolinha", animal.Nome);
            Assert.Equal(racaId, animal.RacaId);
            Assert.Equal(nomeRaca, animal.RacaNome);
            Assert.Equal(EspecieAnimal.Cao, animal.Especie);
            Assert.Equal(Sexo.Macho, animal.Sexo);
            Assert.Equal(NascimentoPadrao, animal.Nascimento.ToUniversalTime());
            Assert.False(animal.IsCastrado);
            Assert.Null(animal.Peso);
            Assert.Null(animal.CondicaoCorporal);
            Assert.Null(animal.GrauAtividade);
            Assert.InRange(animal.DataCadastro.ToUniversalTime(), antes, DateTime.UtcNow.AddSeconds(5));
            Assert.Equal(new[] { tutorId }, animal.TutorIds);
            var tutor = Assert.Single(animal.Tutores!);
            Assert.Equal(tutorId, tutor.Id);
            Assert.Equal(nomeTutor, tutor.Nome);
        }

        [Fact]
        public async Task ObterPorId_AnimalComRacaETutores_TrazNomeDaRacaETutoresComoNaListagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var nomeRaca = $"Raça {Cenario.Unico()}";
            var racaId = await CadastrarRacaDaClinicaAsync(gestor, nomeRaca);
            var nomeTutor = $"Tutor {Cenario.Unico()}";
            var tutorId = await CadastrarTutorAsync(gestor, nomeTutor);
            var id = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutorId });

            var animal = await ObterAsync(gestor, id);

            Assert.Equal(racaId, animal.RacaId);
            Assert.Equal(nomeRaca, animal.RacaNome);
            Assert.Equal(new[] { tutorId }, animal.TutorIds);
            var tutor = Assert.Single(animal.Tutores!);
            Assert.Equal(tutorId, tutor.Id);
            Assert.Equal(nomeTutor, tutor.Nome);
            var naLista = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.Equal(naLista.RacaNome, animal.RacaNome);
            Assert.Equal(naLista.Tutores!.Select(t => t.Nome), animal.Tutores!.Select(t => t.Nome));
        }

        [Fact]
        public async Task ObterPorId_AnimalSemTutores_TrazListasVazias()
        {
            var (_, gestor, racaId) = await ClinicaComGestorERacaAsync();

            var animal = await ObterAsync(gestor, await CadastrarAnimalAsync(gestor, racaId));

            Assert.Empty(animal.TutorIds);
            Assert.NotNull(animal.Tutores);
            Assert.Empty(animal.Tutores!);
        }

        [Fact]
        public async Task ObterPorId_DepoisDeUmaAvaliacao_TrazOEstadoRegistrado()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var resposta = await a.Gestor.PutAsJsonAsync($"/api/clinica/consultas/{a.ConsultaId}/avaliacao", new
            {
                peso = 17.5, condicaoCorporal = 6, grauAtividade = 3, racaoId = a.RacaoId, tipoDieta = 4,
                isGestante = true, isLactanteNumeroFilhotes = 3, semanaLactacao = 2, semanaGestacao = 5,
            });
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

            var animal = await ObterAsync(a.Gestor, a.AnimalId);

            Assert.Equal(17.5, animal.Peso);
            Assert.Equal(CondicaoCorporal.PoucoAcimaDoIdeal, animal.CondicaoCorporal);
            Assert.Equal(GrauAtividade.Ativo, animal.GrauAtividade);
            Assert.True(animal.Gestacao);
            Assert.Equal(3, animal.Lactacao);
            Assert.Equal(2, animal.SemanaLactacao);
            Assert.Equal(5, animal.SemanaGestacao);
        }

        #endregion

        #region Auxiliares

        /// <summary>Clínica nova com um GESTOR e uma raça própria de cão.</summary>
        private async Task<(ClinicaDeTeste clinica, HttpClient gestor, int racaId)> ClinicaComGestorERacaAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            return (clinica, gestor, racaId);
        }

        /// <summary>Animal "Animal de A" na clínica A, com um GESTOR em cada clínica.</summary>
        private async Task<(HttpClient gestorA, HttpClient gestorB, int idA, int racaDeA)> AnimalEmAComGestorDeBAsync()
        {
            var (_, gestorA, racaDeA) = await ClinicaComGestorERacaAsync();
            var (_, gestorB, _) = await ClinicaComGestorERacaAsync();
            var idA = await CadastrarAnimalAsync(gestorA, racaDeA, nome: "Animal de A");
            return (gestorA, gestorB, idA, racaDeA);
        }

        private static async Task<GetAnimaisClinicaDto> ObterAsync(HttpClient cliente, int id)
        {
            var resposta = await cliente.GetAsync($"{Rota}/{id}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return (await LerAsync<ResultadoComDados<GetAnimaisClinicaDto>>(resposta)).Data;
        }

        private static async Task<PaginaResultado<GetAnimaisClinicaDto>> ListarAsync(HttpClient cliente, string query)
        {
            var resposta = await cliente.GetAsync($"{Rota}{query}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<PaginaResultado<GetAnimaisClinicaDto>>(resposta);
        }

        #endregion
    }
}
