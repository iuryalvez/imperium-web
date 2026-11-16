using System.Net;
using System.Net.Http.Json;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Consultas
{
    /// <summary>
    /// Testes de integração das rotas api/clinica/consultas: agendar, detalhar, editar, cancelar, reabrir e excluir,
    /// animal e tutor inativos ou de outra clínica, isolamento, perfis, exclusão bloqueada por avaliação e paginação.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class ConsultaTests
    {
        private const string Rota = "/api/clinica/consultas";

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public ConsultaTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Caminho feliz

        [Fact]
        public async Task Cadastrar_DadosValidos_RetornaCreatedPendente()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var data = new DateTime(2030, 3, 15, 13, 30, 0, DateTimeKind.Utc);

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { animalId = c.AnimalId, tutorId = c.TutorId, dataConsulta = data });

            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            Assert.NotNull(resposta.Headers.Location);
            var criada = await LerAsync<DetalhesConsultaDto>(resposta);
            var consulta = await ObterAsync(c.Gestor, criada.Id);
            Assert.Equal(c.AnimalId, consulta.AnimalId);
            Assert.Equal(c.TutorId, consulta.TutorId);
            Assert.Equal(data, consulta.DataConsulta.ToUniversalTime());
            Assert.Equal("Pendente", consulta.Status);
        }

        [Fact]
        public async Task Cadastrar_SemTutor_RetornaCreated()
        {
            var c = await ClinicaComAnimalETutorAsync();

            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            Assert.Null((await ObterAsync(c.Gestor, id)).TutorId);
        }

        [Fact]
        public async Task Editar_NovaData_RetornaNoContentEAtualiza()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId, c.TutorId);
            var novaData = new DateTime(2031, 1, 2, 10, 0, 0, DateTimeKind.Utc);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/editar", new { id, animalId = c.AnimalId, tutorId = c.TutorId, dataConsulta = novaData });

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            Assert.Equal(novaData, (await ObterAsync(c.Gestor, id)).DataConsulta.ToUniversalTime());
        }

        [Fact]
        public async Task Cancelar_ConsultaPendente_MarcaCanceladaComMotivo()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{id}/cancelar", new { motivo = "  Tutor pediu para remarcar  " });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var consulta = await ObterAsync(c.Gestor, id);
            Assert.Equal("Cancelada", consulta.Status);
            Assert.NotNull(consulta.DataCancelamento);
            Assert.Equal("Tutor pediu para remarcar", consulta.MotivoCancelamento);
        }

        [Fact]
        public async Task Reabrir_ConsultaCancelada_VoltaAPendente()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);
            await c.Gestor.PutAsJsonAsync($"{Rota}/{id}/cancelar", new { motivo = "Engano" });

            var resposta = await c.Gestor.PutAsync($"{Rota}/{id}/reabrir", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var consulta = await ObterAsync(c.Gestor, id);
            Assert.Equal("Pendente", consulta.Status);
            Assert.Null(consulta.DataCancelamento);
            Assert.Null(consulta.MotivoCancelamento);
        }

        [Fact]
        public async Task Excluir_ConsultaSemAvaliacao_RetornaNoContentERemove()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var resposta = await c.Gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await c.Gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region Regras de negócio

        [Fact]
        public async Task Excluir_ConsultaComAvaliacao_RetornaConflictENaoRemove()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await ConsultaComAvaliacaoAsync(c);

            var resposta = await c.Gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("Não é possível excluir uma consulta que já possui um relatório preenchido.", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await c.Gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        [Fact]
        public async Task Cancelar_ConsultaComAvaliacao_RetornaBadRequest()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await ConsultaComAvaliacaoAsync(c);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{id}/cancelar", new { motivo = "Tarde demais" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Só consultas pendentes (sem avaliação) podem ser canceladas.", await TextoAsync(resposta));
            Assert.Null((await ObterAsync(c.Gestor, id)).DataCancelamento);
        }

        [Fact]
        public async Task Cancelar_ConsultaJaCancelada_RetornaBadRequest()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);
            await c.Gestor.PutAsJsonAsync($"{Rota}/{id}/cancelar", new { motivo = "Primeira vez" });

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{id}/cancelar", new { motivo = "Segunda vez" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Esta consulta já está cancelada.", await TextoAsync(resposta));
            Assert.Equal("Primeira vez", (await ObterAsync(c.Gestor, id)).MotivoCancelamento);
        }

        [Fact]
        public async Task Reabrir_ConsultaNaoCancelada_RetornaBadRequest()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var resposta = await c.Gestor.PutAsync($"{Rota}/{id}/reabrir", null);

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Só consultas canceladas podem ser reabertas.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Editar_ConsultaCancelada_RetornaBadRequest()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var dataOriginal = new DateTime(2030, 6, 1, 9, 0, 0, DateTimeKind.Utc);
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId, dataConsulta: dataOriginal);
            await c.Gestor.PutAsJsonAsync($"{Rota}/{id}/cancelar", new { motivo = "Cancelada" });

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/editar", new { id, animalId = c.AnimalId, dataConsulta = dataOriginal.AddDays(3) });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Consulta cancelada não pode ser editada.", await TextoAsync(resposta));
            Assert.Equal(dataOriginal, (await ObterAsync(c.Gestor, id)).DataConsulta.ToUniversalTime());
        }

        [Fact]
        public async Task Cadastrar_SemAnimal_RetornaBadRequestComMensagem()
        {
            var c = await ClinicaComAnimalETutorAsync();

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { dataConsulta = DateTime.UtcNow.AddDays(1) });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O animal da consulta é obrigatório.", await TextoAsync(resposta));
            Assert.Equal(0, (await ListarAsync(c.Gestor, "")).Total);
        }

        [Fact]
        public async Task Cadastrar_SemData_RetornaBadRequestComMensagem()
        {
            var c = await ClinicaComAnimalETutorAsync();

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { animalId = c.AnimalId });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A data da consulta é obrigatória.", await TextoAsync(resposta));
            Assert.Equal(0, (await ListarAsync(c.Gestor, "")).Total);
        }

        [Fact]
        public async Task Editar_SemData_RetornaBadRequestENaoAltera()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var data = new DateTime(2030, 9, 9, 9, 0, 0, DateTimeKind.Utc);
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId, dataConsulta: data);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/editar", new { id, animalId = c.AnimalId });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A data da consulta é obrigatória.", await TextoAsync(resposta));
            Assert.Equal(data, (await ObterAsync(c.Gestor, id)).DataConsulta.ToUniversalTime());
        }

        [Fact]
        public async Task Cadastrar_AnimalInativo_RetornaBadRequest()
        {
            var c = await ClinicaComAnimalETutorAsync();
            await c.Gestor.PutAsync($"/api/clinica/animais/{c.AnimalId}/status?ativo=false", null);

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { animalId = c.AnimalId, dataConsulta = DateTime.UtcNow.AddDays(1) });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O animal selecionado está inativo.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_TutorInativo_RetornaBadRequest()
        {
            var c = await ClinicaComAnimalETutorAsync();
            await c.Gestor.PutAsync($"/api/clinica/tutores/{c.TutorId}/status?ativo=false", null);

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { animalId = c.AnimalId, tutorId = c.TutorId, dataConsulta = DateTime.UtcNow.AddDays(1) });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O tutor selecionado está inativo.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Editar_MantendoAnimalETutorQueFicaramInativos_RetornaNoContent()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId, c.TutorId);
            await c.Gestor.PutAsync($"/api/clinica/animais/{c.AnimalId}/status?ativo=false", null);
            await c.Gestor.PutAsync($"/api/clinica/tutores/{c.TutorId}/status?ativo=false", null);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/editar", new { id, animalId = c.AnimalId, tutorId = c.TutorId, dataConsulta = DateTime.UtcNow.AddDays(2) });

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        }

        [Fact]
        public async Task Editar_TrocandoParaAnimalInativo_RetornaBadRequest()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var outroAnimal = await CadastrarAnimalAsync(c.Gestor, c.RacaId);
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);
            await c.Gestor.PutAsync($"/api/clinica/animais/{outroAnimal}/status?ativo=false", null);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/editar", new { id, animalId = outroAnimal, dataConsulta = DateTime.UtcNow.AddDays(2) });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Equal(c.AnimalId, (await ObterAsync(c.Gestor, id)).AnimalId);
        }

        [Fact]
        public async Task Cadastrar_AnimalOuTutorDeOutraClinica_RetornaBadRequest()
        {
            var a = await ClinicaComAnimalETutorAsync();
            var b = await ClinicaComAnimalETutorAsync();

            var comAnimalDeB = await a.Gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { animalId = b.AnimalId, dataConsulta = DateTime.UtcNow.AddDays(1) });
            var comTutorDeB = await a.Gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { animalId = a.AnimalId, tutorId = b.TutorId, dataConsulta = DateTime.UtcNow.AddDays(1) });

            Assert.Equal(HttpStatusCode.BadRequest, comAnimalDeB.StatusCode);
            Assert.Contains("Animal não encontrado ou não pertence a esta clínica.", await TextoAsync(comAnimalDeB));
            Assert.Equal(HttpStatusCode.BadRequest, comTutorDeB.StatusCode);
            Assert.Contains("Tutor não encontrado ou não pertence a esta clínica.", await TextoAsync(comTutorDeB));
            Assert.Equal(0, (await ListarAsync(a.Gestor, "")).Total);
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task RotasComId_ConsultaDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var a = await ClinicaComAnimalETutorAsync();
            var b = await ClinicaComAnimalETutorAsync();
            var dataOriginal = new DateTime(2030, 7, 1, 8, 0, 0, DateTimeKind.Utc);
            var idA = await CadastrarConsultaAsync(a.Gestor, a.AnimalId, dataConsulta: dataOriginal);

            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.GetAsync($"{Rota}/{idA}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.PutAsJsonAsync($"{Rota}/editar", new { id = idA, animalId = b.AnimalId, dataConsulta = dataOriginal.AddDays(1) })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.PutAsJsonAsync($"{Rota}/{idA}/cancelar", new { motivo = "B" })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.PutAsync($"{Rota}/{idA}/reabrir", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.DeleteAsync($"{Rota}/{idA}")).StatusCode);

            var consulta = await ObterAsync(a.Gestor, idA);
            Assert.Equal(dataOriginal, consulta.DataConsulta.ToUniversalTime());
            Assert.Equal(a.AnimalId, consulta.AnimalId);
            Assert.Equal("Pendente", consulta.Status);
        }

        [Fact]
        public async Task Listar_OutraClinica_NaoMostraConsultaDeA()
        {
            var a = await ClinicaComAnimalETutorAsync();
            var b = await ClinicaComAnimalETutorAsync();
            await CadastrarConsultaAsync(a.Gestor, a.AnimalId);

            Assert.Equal(0, (await ListarAsync(b.Gestor, "")).Total);
        }

        [Fact]
        public async Task RotasComId_ConsultaInexistente_RetornaNotFound()
        {
            var c = await ClinicaComAnimalETutorAsync();
            const int inexistente = 999999999;

            Assert.Equal(HttpStatusCode.NotFound, (await c.Gestor.GetAsync($"{Rota}/{inexistente}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await c.Gestor.PutAsJsonAsync($"{Rota}/editar", new { id = inexistente, animalId = c.AnimalId, dataConsulta = DateTime.UtcNow })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await c.Gestor.PutAsJsonAsync($"{Rota}/{inexistente}/cancelar", new { })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await c.Gestor.PutAsync($"{Rota}/{inexistente}/reabrir", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await c.Gestor.DeleteAsync($"{Rota}/{inexistente}")).StatusCode);
        }

        #endregion

        #region Perfis

        [Theory]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Secretario)]
        public async Task Cadastrar_PerfilComGerenciaConsultas_RetornaCreated(string perfil)
        {
            var c = await ClinicaComAnimalETutorAsync();
            var cliente = await _cenario.ClienteAsync(c.Clinica, perfil);

            var resposta = await cliente.PostAsJsonAsync($"{Rota}/cadastrar", new { animalId = c.AnimalId, dataConsulta = DateTime.UtcNow.AddDays(1) });

            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Theory]
        [InlineData("GET", "")]
        [InlineData("GET", "/{id}")]
        [InlineData("POST", "/cadastrar")]
        [InlineData("PUT", "/editar")]
        [InlineData("PUT", "/{id}/cancelar")]
        [InlineData("DELETE", "/{id}")]
        public async Task QualquerRota_PerfilTecnico_RetornaForbidden(string metodo, string caminho)
        {
            var c = await ClinicaComAnimalETutorAsync();
            var tecnico = await _cenario.ClienteAsync(c.Clinica, Perfis.Tecnico);
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var requisicao = new HttpRequestMessage(new HttpMethod(metodo), $"{Rota}{caminho.Replace("{id}", id.ToString())}");
            if (metodo is "POST" or "PUT")
                requisicao.Content = JsonContent.Create(new { id, animalId = c.AnimalId, dataConsulta = DateTime.UtcNow.AddDays(5), motivo = "Técnico" });

            var resposta = await tecnico.SendAsync(requisicao);

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
            var consulta = await ObterAsync(c.Gestor, id);
            Assert.Equal("Pendente", consulta.Status);
        }

        #endregion

        #region Paginação e filtros

        [Fact]
        public async Task Listar_SemParametros_RetornaPrimeiraPaginaPorDataDaConsultaMaisRecentePrimeiro()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var inicio = new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var ids = new List<int>();
            // Cadastradas fora de ordem: a lista ordena pela data da consulta, não pela ordem de cadastro
            foreach (var dia in new[] { 5, 1, 12, 3, 8, 10, 2, 11, 4, 9, 7, 6 })
                ids.Add(await CadastrarConsultaAsync(c.Gestor, c.AnimalId, dataConsulta: inicio.AddDays(dia)));

            var pagina = await ListarAsync(c.Gestor, "");

            Assert.Equal(12, pagina.Total);
            Assert.Equal(10, pagina.Itens.Count);
            var datas = pagina.Itens.Select(i => i.DataConsulta).ToList();
            Assert.Equal(datas.OrderByDescending(d => d).ToList(), datas);
            Assert.Equal(inicio.AddDays(12), datas[0].ToUniversalTime());
        }

        [Fact]
        public async Task Listar_TamanhoPaginaAcimaDoMaximo_LimitaA100()
        {
            var c = await ClinicaComAnimalETutorAsync();

            Assert.Equal(100, (await ListarAsync(c.Gestor, "?tamanhoPagina=250")).TamanhoPagina);
        }

        [Fact]
        public async Task Listar_FiltroStatusCancelada_RetornaSoCanceladas()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var cancelada = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);
            await CadastrarConsultaAsync(c.Gestor, c.AnimalId);
            await c.Gestor.PutAsJsonAsync($"{Rota}/{cancelada}/cancelar", new { motivo = "Filtro" });

            var pagina = await ListarAsync(c.Gestor, "?status=Cancelada");

            Assert.Equal(cancelada, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Listar_FiltroAnimal_RetornaSoDoAnimal()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var outroAnimal = await CadastrarAnimalAsync(c.Gestor, c.RacaId);
            var doOutro = await CadastrarConsultaAsync(c.Gestor, outroAnimal);
            await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var pagina = await ListarAsync(c.Gestor, $"?animalId={outroAnimal}");

            Assert.Equal(doOutro, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Listar_MarcadasPorMim_RetornaSoAsDoUsuario()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var secretario = await _cenario.ClienteAsync(c.Clinica, Perfis.Secretario);
            var minha = await CadastrarConsultaAsync(secretario, c.AnimalId);
            await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var pagina = await ListarAsync(secretario, "?marcadasPorMim=true");

            Assert.Equal(minha, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Listar_BuscaPorNomeDoAnimal_RetornaSoQuemCasa()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var bidu = await CadastrarAnimalAsync(c.Gestor, c.RacaId, nome: "Bidu Procurado");
            var doBidu = await CadastrarConsultaAsync(c.Gestor, bidu);
            await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var pagina = await ListarAsync(c.Gestor, "?busca=bidu");

            Assert.Equal(doBidu, Assert.Single(pagina.Itens).Id);
        }

        #endregion

        #region Campos de apoio

        [Fact]
        public async Task Listar_ConsultaPendente_TrazAnimalRacaTutorEQuemMarcou()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);
            var gestor = usuario.Cliente;
            var nomeRaca = $"Raça {Cenario.Unico()}";
            var respostaRaca = await gestor.PostAsJsonAsync("/api/clinica/racas/cadastrar", new
            {
                nome = nomeRaca, especie = Especie.Cao,
                pesoMinimoReferenciaMacho = 20.0, pesoMaximoReferenciaMacho = 30.0,
                pesoMinimoReferenciaFemea = 18.0, pesoMaximoReferenciaFemea = 26.0,
            });
            var racaId = await LerIdDoDataAsync(respostaRaca, "id");
            var nomeTutor = $"Tutor {Cenario.Unico()}";
            var tutorId = await CadastrarTutorAsync(gestor, nomeTutor);
            var nomeAnimal = $"Animal {Cenario.Unico()}";
            var animalId = await CadastrarAnimalAsync(gestor, racaId, nome: nomeAnimal, tutorIds: new[] { tutorId });
            var data = new DateTime(2030, 6, 1, 14, 0, 0, DateTimeKind.Utc);
            var id = await CadastrarConsultaAsync(gestor, animalId, tutorId, data);

            var item = Assert.Single((await ListarAsync(gestor, "")).Itens);

            Assert.Equal(id, item.Id);
            Assert.Equal(data, item.DataConsulta.ToUniversalTime());
            Assert.Equal(animalId, item.AnimalId);
            Assert.Equal(nomeAnimal, item.NomeAnimal);
            Assert.Equal(EspecieAnimal.Cao, item.EspecieAnimal);
            Assert.Equal(Sexo.Macho, item.SexoAnimal);
            Assert.Equal(await AcompanhamentoDoAnimalAsync(_factory, animalId), item.AcompanhamentoId);
            Assert.Equal(nomeRaca, item.NomeRaca);
            Assert.Equal(20.0, item.PesoMinimoRaca); // animal macho: usa a referência de macho
            Assert.Equal(30.0, item.PesoMaximoRaca);
            Assert.Equal(tutorId, item.TutorId);
            Assert.Equal(nomeTutor, item.NomeTutor);
            Assert.Equal(usuario.UsuarioId, item.UsuarioId);
            Assert.Equal(await NomeDoUsuarioAsync(usuario.UsuarioId), item.NomeUsuario);
            Assert.False(item.AvaliacaoPreenchida);
            Assert.Null(item.DietaId);
            Assert.Equal("Pendente", item.Status);
            Assert.Null(item.DataCancelamento);
            Assert.Null(item.MotivoCancelamento);
            Assert.Null(item.NomeCanceladaPor);
        }

        [Fact]
        public async Task Listar_ConsultaComAvaliacaoEDieta_MarcaAvaliacaoPreenchidaEDietaId()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var avaliacao = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            var comAvaliacao = Assert.Single((await ListarAsync(a.Gestor, "")).Itens);
            var dieta = await CriarDietaAsync(a.Gestor, avaliacao);
            var comDieta = Assert.Single((await ListarAsync(a.Gestor, "")).Itens);

            Assert.True(comAvaliacao.AvaliacaoPreenchida);
            Assert.Null(comAvaliacao.DietaId);
            Assert.Equal("EmAndamento", comAvaliacao.Status);
            Assert.True(comDieta.AvaliacaoPreenchida);
            Assert.Equal(dieta.Id, comDieta.DietaId);
            Assert.Equal("Concluida", comDieta.Status);
        }

        [Fact]
        public async Task Listar_ConsultaSemTutorDeRacaSemPesos_TrazTutorZeroEPesosNulos()
        {
            var c = await ClinicaComAnimalETutorAsync();
            await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var item = Assert.Single((await ListarAsync(c.Gestor, "")).Itens);

            Assert.Equal(0, item.TutorId);
            Assert.Null(item.NomeTutor);
            Assert.Null(item.PesoMinimoRaca);
            Assert.Null(item.PesoMaximoRaca);
        }

        [Fact]
        public async Task Listar_ConsultaCancelada_TrazQuemCancelou()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var secretario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Secretario);
            var animalId = await CadastrarAnimalAsync(gestor, await CadastrarRacaDaClinicaAsync(gestor));
            var id = await CadastrarConsultaAsync(gestor, animalId);

            await secretario.Cliente.PutAsJsonAsync($"{Rota}/{id}/cancelar", new { motivo = "Tutor desmarcou" });

            var item = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.Equal("Cancelada", item.Status);
            Assert.NotNull(item.DataCancelamento);
            Assert.Equal("Tutor desmarcou", item.MotivoCancelamento);
            Assert.Equal(await NomeDoUsuarioAsync(secretario.UsuarioId), item.NomeCanceladaPor);
        }

        [Fact]
        public async Task Obter_ConsultaComTutor_TrazContatoDoTutorEQuemMarcou()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);
            var gestor = usuario.Cliente;
            var nomeTutor = $"Tutor {Cenario.Unico()}";
            var cpf = GeradorCpf.Novo();
            var email = $"tutor.{Cenario.Unico()}@teste.local";
            var respostaTutor = await gestor.PostAsJsonAsync("/api/clinica/tutores/cadastrar", new { nome = nomeTutor, cpf, email, telefone = "62999990004" });
            var tutorId = await LerIdDoDataAsync(respostaTutor, "tutorId");
            var nomeAnimal = $"Animal {Cenario.Unico()}";
            var animalId = await CadastrarAnimalAsync(gestor, await CadastrarRacaDaClinicaAsync(gestor), nome: nomeAnimal, tutorIds: new[] { tutorId });
            var id = await CadastrarConsultaAsync(gestor, animalId, tutorId);

            var consulta = await ObterAsync(gestor, id);

            Assert.Equal(id, consulta.Id);
            Assert.Equal(animalId, consulta.AnimalId);
            Assert.Equal(nomeAnimal, consulta.NomeAnimal);
            Assert.Equal(tutorId, consulta.TutorId);
            Assert.Equal(nomeTutor, consulta.NomeTutor);
            Assert.Equal(cpf, consulta.CpfTutor);
            Assert.Equal(email, consulta.EmailTutor);
            Assert.Equal("62999990004", consulta.TelefoneTutor);
            Assert.Equal(usuario.UsuarioId, consulta.UsuarioId);
            Assert.Equal(await NomeDoUsuarioAsync(usuario.UsuarioId), consulta.NomeVeterinario);
            Assert.Null(consulta.AvaliacaoId);
            Assert.Equal("Pendente", consulta.Status);
        }

        [Fact]
        public async Task Obter_ConsultaSemTutor_TrazCamposDoTutorNulos()
        {
            var c = await ClinicaComAnimalETutorAsync();
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);

            var consulta = await ObterAsync(c.Gestor, id);

            Assert.Null(consulta.TutorId);
            Assert.Null(consulta.NomeTutor);
            Assert.Null(consulta.CpfTutor);
            Assert.Null(consulta.EmailTutor);
            Assert.Null(consulta.TelefoneTutor);
        }

        #endregion

        #region Auxiliares

        private record ClinicaComAnimal(ClinicaDeTeste Clinica, HttpClient Gestor, int RacaId, int AnimalId, int TutorId);

        /// <summary>Clínica nova com GESTOR, uma raça, um tutor e um animal vinculado a esse tutor.</summary>
        private async Task<ClinicaComAnimal> ClinicaComAnimalETutorAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            var tutorId = await CadastrarTutorAsync(gestor);
            var animalId = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutorId });
            return new ClinicaComAnimal(clinica, gestor, racaId, animalId, tutorId);
        }

        private async Task<int> ConsultaComAvaliacaoAsync(ClinicaComAnimal c)
        {
            var id = await CadastrarConsultaAsync(c.Gestor, c.AnimalId);
            var racaoId = await CadastrarRacaoDaClinicaAsync(c.Gestor);
            await RegistrarAvaliacaoComRacaoAsync(_factory, id, c.AnimalId, racaoId);
            return id;
        }

        private async Task<string> NomeDoUsuarioAsync(int usuarioId)
        {
            await using var contexto = _factory.CriarContexto();
            return (await contexto.Usuarios.FindAsync(usuarioId))!.Nome;
        }

        private static async Task<DetalhesConsultaDto> ObterAsync(HttpClient cliente, int id)
        {
            var resposta = await cliente.GetAsync($"{Rota}/{id}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<DetalhesConsultaDto>(resposta);
        }

        private static async Task<PaginaResultado<GetConsultaDto>> ListarAsync(HttpClient cliente, string query)
        {
            var resposta = await cliente.GetAsync($"{Rota}{query}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<PaginaResultado<GetConsultaDto>>(resposta);
        }

        #endregion
    }
}
