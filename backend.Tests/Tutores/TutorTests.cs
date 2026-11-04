using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.Utils.Paginacao;
using backend.Tests.Infra;
using Xunit;

namespace backend.Tests.Tutores
{
    /// <summary>
    /// Testes de integração das rotas api/clinica/tutores: caminho feliz, isolamento entre clínicas,
    /// perfis, validação, CPF único por clínica, edição parcial e paginação.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class TutorTests
    {
        private const string Rota = "/api/clinica/tutores";
        private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        private readonly Cenario _cenario;

        public TutorTests(ImperiumApiFactory factory)
        {
            _cenario = new Cenario(factory);
        }

        #region Caminho feliz

        [Fact]
        public async Task Cadastrar_DadosValidos_RetornaOkEGravaNaClinicaDoToken()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoTutor(cpf, "Maria Tutora"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var id = await LerTutorIdAsync(resposta);
            var tutor = await ObterAsync(gestor, id);
            Assert.Equal(clinica.Id, tutor.ClinicaId);
            Assert.Equal("Maria Tutora", tutor.Nome);
            Assert.Equal(cpf, tutor.Cpf);
            Assert.True(tutor.IsAtivo);
            Assert.Null(tutor.DataDesativacao);
        }

        [Fact]
        public async Task Cadastrar_ComClinicaIdDeOutraClinicaNoCorpo_UsaClinicaDoToken()
        {
            var clinicaA = await _cenario.CriarClinicaAsync("A");
            var clinicaB = await _cenario.CriarClinicaAsync("B");
            var gestorA = await _cenario.ClienteAsync(clinicaA, Perfis.Gestor);

            var corpo = new { nome = "Tutor Intruso", cpf = GeradorCpf.Novo(), email = "intruso@teste.local", clinicaId = clinicaB.Id };
            var resposta = await gestorA.PostAsJsonAsync($"{Rota}/cadastrar", corpo);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var tutor = await ObterAsync(gestorA, await LerTutorIdAsync(resposta));
            Assert.Equal(clinicaA.Id, tutor.ClinicaId);
        }

        [Fact]
        public async Task ObterPorId_TutorDaClinica_RetornaDados()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            var id = await CadastrarAsync(gestor, cpf, "João Detalhe");

            var resposta = await gestor.GetAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var tutor = await LerAsync<GetTutoresClinicaDto>(resposta);
            Assert.Equal(id, tutor.Id);
            Assert.Equal("João Detalhe", tutor.Nome);
            Assert.Equal(cpf, tutor.Cpf);
            Assert.Equal("Goiânia", tutor.Cidade);
        }

        [Fact]
        public async Task ObterPorId_TutorInexistente_RetornaNotFound()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.GetAsync($"{Rota}/999999999");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Editar_TutorInexistente_RetornaNotFoundComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar",
                new { id = 999999999, nome = "Ninguém", cpf = GeradorCpf.Novo(), email = "ninguem@teste.local" });

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("não encontrado nesta clínica.", await resposta.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task MudarStatus_TutorInexistente_RetornaNotFoundComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PutAsync($"{Rota}/999999999/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("Tutor não encontrado nesta clínica.", await resposta.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Excluir_TutorInexistente_RetornaNotFoundComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.DeleteAsync($"{Rota}/999999999");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("Tutor não encontrado nesta clínica.", await resposta.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Listar_TutoresDaClinica_RetornaTutorCadastrado()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var id = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Ana Lista");

            var pagina = await ListarAsync(gestor, "");

            Assert.Equal(1, pagina.Total);
            var unico = Assert.Single(pagina.Itens);
            Assert.Equal(id, unico.Id);
        }

        [Fact]
        public async Task Editar_DadosValidos_RetornaNoContentEAtualiza()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var id = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Nome Antigo");
            var novoCpf = GeradorCpf.Novo();

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar",
                new { id, nome = "Nome Novo", cpf = novoCpf, email = "novo@teste.local", telefone = "62988887777" });

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            var tutor = await ObterAsync(gestor, id);
            Assert.Equal("Nome Novo", tutor.Nome);
            Assert.Equal(novoCpf, tutor.Cpf);
            Assert.Equal("novo@teste.local", tutor.Email);
            Assert.Equal("62988887777", tutor.Telefone);
        }

        [Fact]
        public async Task MudarStatus_Desativar_MarcaInativoEGravaDataDesativacao()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var id = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Tutor Status");

            var resposta = await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var tutor = await ObterAsync(gestor, id);
            Assert.False(tutor.IsAtivo);
            Assert.NotNull(tutor.DataDesativacao);
        }

        [Fact]
        public async Task MudarStatus_Reativar_MarcaAtivoELimpaDataDesativacao()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var id = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Tutor Reativado");
            await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null);

            var resposta = await gestor.PutAsync($"{Rota}/{id}/status?ativo=true", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var tutor = await ObterAsync(gestor, id);
            Assert.True(tutor.IsAtivo);
            Assert.Null(tutor.DataDesativacao);
        }

        [Fact]
        public async Task Excluir_TutorSemConsultas_RetornaNoContentERemove()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var id = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Tutor Excluído");

            var resposta = await gestor.DeleteAsync($"{Rota}/{id}");

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.GetAsync($"{Rota}/{id}")).StatusCode);
        }

        #endregion

        #region CPF único por clínica

        [Fact]
        public async Task Cadastrar_CpfJaExisteNaClinica_RetornaConflict()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            await CadastrarAsync(gestor, cpf, "Primeiro");

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoTutor(cpf, "Segundo"));

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("CPF já cadastrado nesta clínica.", await resposta.Content.ReadAsStringAsync());
            Assert.Equal(1, (await ListarAsync(gestor, "")).Total);
        }

        [Fact]
        public async Task Cadastrar_CpfJaExisteEmOutraClinica_RetornaOk()
        {
            var clinicaA = await _cenario.CriarClinicaAsync("A");
            var clinicaB = await _cenario.CriarClinicaAsync("B");
            var gestorA = await _cenario.ClienteAsync(clinicaA, Perfis.Gestor);
            var gestorB = await _cenario.ClienteAsync(clinicaB, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            await CadastrarAsync(gestorA, cpf, "Tutor em A");

            var resposta = await gestorB.PostAsJsonAsync($"{Rota}/cadastrar", NovoTutor(cpf, "Tutor em B"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var tutorB = await ObterAsync(gestorB, await LerTutorIdAsync(resposta));
            Assert.Equal(clinicaB.Id, tutorB.ClinicaId);
        }

        [Fact]
        public async Task Cadastrar_CpfDeTutorDeOutraClinica_NaoAlteraTutorDaOutraClinica()
        {
            var clinicaA = await _cenario.CriarClinicaAsync("A");
            var clinicaB = await _cenario.CriarClinicaAsync("B");
            var gestorA = await _cenario.ClienteAsync(clinicaA, Perfis.Gestor);
            var gestorB = await _cenario.ClienteAsync(clinicaB, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            var idA = await CadastrarAsync(gestorA, cpf, "Nome em A");
            var antes = await ObterAsync(gestorA, idA);

            await gestorB.PostAsJsonAsync($"{Rota}/cadastrar",
                new { nome = "Nome em B", cpf, email = "b@teste.local", telefone = "62911112222", cidade = "Anápolis" });

            var depois = await ObterAsync(gestorA, idA);
            Assert.Equal(antes.Nome, depois.Nome);
            Assert.Equal(antes.Email, depois.Email);
            Assert.Equal(antes.Telefone, depois.Telefone);
            Assert.Equal(antes.Cidade, depois.Cidade);
            Assert.Equal(clinicaA.Id, depois.ClinicaId);
        }

        [Fact]
        public async Task Editar_CpfDeOutroTutorDaClinica_RetornaConflict()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpfExistente = GeradorCpf.Novo();
            await CadastrarAsync(gestor, cpfExistente, "Dono do CPF");
            var cpfOriginal = GeradorCpf.Novo();
            var id = await CadastrarAsync(gestor, cpfOriginal, "Outro Tutor");

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar",
                new { id, nome = "Outro Tutor", cpf = cpfExistente, email = "outro@teste.local" });

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("CPF já cadastrado nesta clínica.", await resposta.Content.ReadAsStringAsync());
            Assert.Equal(cpfOriginal, (await ObterAsync(gestor, id)).Cpf);
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task ObterPorId_TutorDeOutraClinica_RetornaNotFound()
        {
            var (gestorA, gestorB, idA) = await TutorEmAComGestorDeBAsync();

            var resposta = await gestorB.GetAsync($"{Rota}/{idA}");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Listar_OutraClinica_NaoMostraTutorDaClinicaA()
        {
            var (_, gestorB, idA) = await TutorEmAComGestorDeBAsync();

            var pagina = await ListarAsync(gestorB, "");

            Assert.Equal(0, pagina.Total);
            Assert.DoesNotContain(pagina.Itens, t => t.Id == idA);
        }

        [Fact]
        public async Task Opcoes_OutraClinica_NaoMostraTutorDaClinicaA()
        {
            var (_, gestorB, idA) = await TutorEmAComGestorDeBAsync();

            var resposta = await gestorB.GetAsync($"{Rota}/opcoes");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var opcoes = await LerAsync<List<OpcaoDto>>(resposta);
            Assert.DoesNotContain(opcoes, o => o.Id == idA);
        }

        [Fact]
        public async Task Editar_TutorDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (gestorA, gestorB, idA) = await TutorEmAComGestorDeBAsync();
            var antes = await ObterAsync(gestorA, idA);

            var resposta = await gestorB.PutAsJsonAsync($"{Rota}/editar",
                new { id = idA, nome = "Alterado por B", cpf = antes.Cpf, email = "b@teste.local" });

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            var depois = await ObterAsync(gestorA, idA);
            Assert.Equal(antes.Nome, depois.Nome);
            Assert.Equal(antes.Email, depois.Email);
        }

        [Fact]
        public async Task MudarStatus_TutorDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (gestorA, gestorB, idA) = await TutorEmAComGestorDeBAsync();

            var resposta = await gestorB.PutAsync($"{Rota}/{idA}/status?ativo=false", null);

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            var tutor = await ObterAsync(gestorA, idA);
            Assert.True(tutor.IsAtivo);
            Assert.Null(tutor.DataDesativacao);
        }

        [Fact]
        public async Task Excluir_TutorDeOutraClinica_RetornaNotFoundENaoRemove()
        {
            var (gestorA, gestorB, idA) = await TutorEmAComGestorDeBAsync();

            var resposta = await gestorB.DeleteAsync($"{Rota}/{idA}");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await gestorA.GetAsync($"{Rota}/{idA}")).StatusCode);
        }

        #endregion

        #region Perfis

        [Fact]
        public async Task QualquerRota_SemToken_RetornaUnauthorized()
        {
            var anonimo = _cenario.ClienteAnonimo();

            var resposta = await anonimo.GetAsync(Rota);

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        [Theory]
        [InlineData(Perfis.Gestor)]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Secretario)]
        public async Task Cadastrar_PerfilComGerenciaTutores_RetornaOk(string perfil)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            var resposta = await cliente.PostAsJsonAsync($"{Rota}/cadastrar", NovoTutor(GeradorCpf.Novo(), "Tutor por perfil"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task Cadastrar_AdminSimulandoGestor_RetornaOk()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var admin = await _cenario.ClienteAdminSimulandoAsync(clinica, Perfis.Gestor);

            var resposta = await admin.PostAsJsonAsync($"{Rota}/cadastrar", NovoTutor(GeradorCpf.Novo(), "Tutor do admin"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(clinica.Id, (await ObterAsync(admin, await LerTutorIdAsync(resposta))).ClinicaId);
        }

        [Theory]
        [InlineData("POST", "cadastrar")]
        [InlineData("PUT", "editar")]
        [InlineData("PUT", "{id}/status?ativo=false")]
        [InlineData("DELETE", "{id}")]
        public async Task RotasDeEscrita_PerfilTecnico_RetornaForbidden(string metodo, string caminho)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var tecnico = await _cenario.ClienteAsync(clinica, Perfis.Tecnico);
            var cpf = GeradorCpf.Novo();
            var id = await CadastrarAsync(gestor, cpf, "Tutor Protegido");

            var requisicao = new HttpRequestMessage(new HttpMethod(metodo), $"{Rota}/{caminho.Replace("{id}", id.ToString())}");
            if (metodo != "DELETE" && !caminho.Contains("status"))
                requisicao.Content = JsonContent.Create(new { id, nome = "Alterado", cpf, email = "tec@teste.local" });

            var resposta = await tecnico.SendAsync(requisicao);

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
            var tutor = await ObterAsync(gestor, id);
            Assert.Equal("Tutor Protegido", tutor.Nome);
            Assert.True(tutor.IsAtivo);
        }

        [Theory]
        [InlineData("")]
        [InlineData("/{id}")]
        public async Task Leitura_PerfilTecnico_RetornaForbidden(string caminho)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var tecnico = await _cenario.ClienteAsync(clinica, Perfis.Tecnico);
            var id = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Tutor Sigiloso");

            var resposta = await tecnico.GetAsync($"{Rota}{caminho.Replace("{id}", id.ToString())}");

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }

        [Fact]
        public async Task Opcoes_PerfilTecnico_RetornaSoONomeSemCpf()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var tecnico = await _cenario.ClienteAsync(clinica, Perfis.Tecnico);
            var cpf = GeradorCpf.Novo();
            var id = await CadastrarAsync(gestor, cpf, "Tutor Opção");

            var resposta = await tecnico.GetAsync($"{Rota}/opcoes");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var opcao = Assert.Single(await LerAsync<List<OpcaoDto>>(resposta));
            Assert.Equal(id, opcao.Id);
            Assert.Equal("Tutor Opção", opcao.Nome);
            Assert.DoesNotContain(cpf, opcao.Nome);
        }

        #endregion

        #region Validação

        [Fact]
        public async Task Cadastrar_SemCpf_RetornaBadRequestComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", new { nome = "Sem CPF", email = "semcpf@teste.local" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O CPF do tutor é obrigatório.", await resposta.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Cadastrar_CpfComMaisDe11Digitos_RetornaBadRequestComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", NovoTutor("123456789012", "CPF Longo"));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O CPF do tutor deve ter no máximo 11 dígitos.", await resposta.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Cadastrar_EmailInvalido_RetornaBadRequestComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar",
                new { nome = "Email Ruim", cpf = GeradorCpf.Novo(), email = "nao-e-email" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O e-mail não é válido.", await resposta.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Cadastrar_SemNome_RetornaBadRequestComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar",
                new { cpf = GeradorCpf.Novo(), email = "semnome@teste.local" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome do tutor é obrigatório.", await resposta.Content.ReadAsStringAsync());
            Assert.Equal(0, (await ListarAsync(gestor, "")).Total);
        }

        [Fact]
        public async Task Editar_SemNome_RetornaBadRequestComMensagem()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            var id = await CadastrarAsync(gestor, cpf, "Nome Mantido");

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar", new { id, cpf, email = "x@teste.local" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O nome do tutor é obrigatório.", await resposta.Content.ReadAsStringAsync());
            Assert.Equal("Nome Mantido", (await ObterAsync(gestor, id)).Nome);
        }

        #endregion

        #region Edição parcial

        [Fact]
        public async Task Editar_SemCamposOpcionais_MantemValoresAtuais()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            var id = await CadastrarAsync(gestor, cpf, "Tutor Parcial");
            var antes = await ObterAsync(gestor, id);

            // Só os campos obrigatórios: telefone, whatsapp e endereço ficam de fora (nulos no DTO)
            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar",
                new { id, nome = "Tutor Parcial Editado", cpf, email = antes.Email });

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            var depois = await ObterAsync(gestor, id);
            Assert.Equal("Tutor Parcial Editado", depois.Nome);
            Assert.Equal(antes.Telefone, depois.Telefone);
            Assert.Equal(antes.Whatsapp, depois.Whatsapp);
            Assert.Equal(antes.Cep, depois.Cep);
            Assert.Equal(antes.Logradouro, depois.Logradouro);
            Assert.Equal(antes.Bairro, depois.Bairro);
            Assert.Equal(antes.Cidade, depois.Cidade);
            Assert.Equal(antes.Uf, depois.Uf);
            Assert.Equal(antes.DataCadastro, depois.DataCadastro);
            Assert.True(depois.IsAtivo);
        }

        [Fact]
        public async Task Editar_TutorInativo_MantemInativo()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            var id = await CadastrarAsync(gestor, cpf, "Tutor Inativo");
            await gestor.PutAsync($"{Rota}/{id}/status?ativo=false", null);

            var resposta = await gestor.PutAsJsonAsync($"{Rota}/editar",
                new { id, nome = "Tutor Inativo Editado", cpf, email = "inativo@teste.local", isAtivo = true });

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            var tutor = await ObterAsync(gestor, id);
            Assert.False(tutor.IsAtivo);
            Assert.NotNull(tutor.DataDesativacao);
        }

        #endregion

        #region Paginação

        [Fact]
        public async Task Listar_SemParametros_RetornaPrimeiraPaginaCom10MaisRecentesPrimeiro()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var ids = await CadastrarVariosAsync(gestor, 12);

            var pagina = await ListarAsync(gestor, "");

            Assert.Equal(12, pagina.Total);
            Assert.Equal(1, pagina.Pagina);
            Assert.Equal(10, pagina.TamanhoPagina);
            Assert.Equal(10, pagina.Itens.Count);
            var esperado = ids.AsEnumerable().Reverse().Take(10).ToList();
            Assert.Equal(esperado, pagina.Itens.Select(t => t.Id).ToList());
        }

        [Fact]
        public async Task Listar_SegundaPagina_RetornaRestante()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var ids = await CadastrarVariosAsync(gestor, 12);

            var pagina = await ListarAsync(gestor, "?pagina=2");

            Assert.Equal(12, pagina.Total);
            Assert.Equal(2, pagina.Pagina);
            Assert.Equal(new[] { ids[1], ids[0] }, pagina.Itens.Select(t => t.Id).ToArray());
        }

        [Fact]
        public async Task Listar_TamanhoPaginaAcimaDoMaximo_LimitaA100()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);

            var pagina = await ListarAsync(gestor, "?tamanhoPagina=500");

            Assert.Equal(100, pagina.TamanhoPagina);
        }

        [Fact]
        public async Task Listar_BuscaPorNome_RetornaSoQuemCasa()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var idProcurado = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Bartolomeu Silva");
            await CadastrarAsync(gestor, GeradorCpf.Novo(), "Carla Souza");

            var pagina = await ListarAsync(gestor, "?busca=bartolo");

            Assert.Equal(1, pagina.Total);
            Assert.Equal(idProcurado, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Listar_BuscaPorCpf_RetornaSoQuemCasa()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var cpf = GeradorCpf.Novo();
            var idProcurado = await CadastrarAsync(gestor, cpf, "Dono do CPF");
            await CadastrarAsync(gestor, GeradorCpf.Novo(), "Outro Tutor");

            var pagina = await ListarAsync(gestor, $"?busca={cpf}");

            Assert.Equal(idProcurado, Assert.Single(pagina.Itens).Id);
        }

        [Fact]
        public async Task Listar_OrdenarPorNomeAsc_RetornaEmOrdemAlfabetica()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            await CadastrarAsync(gestor, GeradorCpf.Novo(), "Carlos");
            await CadastrarAsync(gestor, GeradorCpf.Novo(), "Ana");
            await CadastrarAsync(gestor, GeradorCpf.Novo(), "Bruno");

            var pagina = await ListarAsync(gestor, "?ordenarPor=nome&ordem=asc");

            Assert.Equal(new[] { "Ana", "Bruno", "Carlos" }, pagina.Itens.Select(t => t.Nome).ToArray());
        }

        [Fact]
        public async Task Listar_OrdenarPorCampoForaDaListaBranca_UsaMaisRecentesPrimeiro()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var ids = await CadastrarVariosAsync(gestor, 3);

            var pagina = await ListarAsync(gestor, "?ordenarPor=cpf");

            Assert.Equal(ids.AsEnumerable().Reverse().ToArray(), pagina.Itens.Select(t => t.Id).ToArray());
        }

        [Fact]
        public async Task Listar_FiltroAtivoFalse_RetornaSoInativos()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var idInativo = await CadastrarAsync(gestor, GeradorCpf.Novo(), "Inativo");
            await CadastrarAsync(gestor, GeradorCpf.Novo(), "Ativo");
            await gestor.PutAsync($"{Rota}/{idInativo}/status?ativo=false", null);

            var pagina = await ListarAsync(gestor, "?ativo=false");

            Assert.Equal(idInativo, Assert.Single(pagina.Itens).Id);
        }

        #endregion

        #region Auxiliares

        private static object NovoTutor(string cpf, string nome) => new
        {
            nome,
            cpf,
            email = $"tutor.{Cenario.Unico()}@teste.local",
            telefone = "62999990000",
            whatsapp = "62999991111",
            cep = "74000000",
            logradouro = "Rua Teste",
            bairro = "Centro",
            cidade = "Goiânia",
            estado = "Goiás",
            uf = "GO",
        };

        private static async Task<int> CadastrarAsync(HttpClient cliente, string cpf, string nome)
        {
            var resposta = await cliente.PostAsJsonAsync($"{Rota}/cadastrar", NovoTutor(cpf, nome));
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerTutorIdAsync(resposta);
        }

        private static async Task<List<int>> CadastrarVariosAsync(HttpClient cliente, int quantidade)
        {
            var ids = new List<int>();
            for (var i = 1; i <= quantidade; i++)
                ids.Add(await CadastrarAsync(cliente, GeradorCpf.Novo(), $"Tutor {i:00}"));
            return ids;
        }

        private static async Task<int> LerTutorIdAsync(HttpResponseMessage resposta)
        {
            using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
            return json.RootElement.GetProperty("data").GetProperty("tutorId").GetInt32();
        }

        private static async Task<GetTutoresClinicaDto> ObterAsync(HttpClient cliente, int id)
        {
            var resposta = await cliente.GetAsync($"{Rota}/{id}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<GetTutoresClinicaDto>(resposta);
        }

        private static async Task<PaginaResultado<GetTutoresClinicaDto>> ListarAsync(HttpClient cliente, string query)
        {
            var resposta = await cliente.GetAsync($"{Rota}{query}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<PaginaResultado<GetTutoresClinicaDto>>(resposta);
        }

        private static async Task<T> LerAsync<T>(HttpResponseMessage resposta) =>
            (await resposta.Content.ReadFromJsonAsync<T>(Json))!;

        /// <summary>Clínica A com um tutor e um GESTOR em cada clínica (A e B).</summary>
        private async Task<(HttpClient gestorA, HttpClient gestorB, int idA)> TutorEmAComGestorDeBAsync()
        {
            var clinicaA = await _cenario.CriarClinicaAsync("A");
            var clinicaB = await _cenario.CriarClinicaAsync("B");
            var gestorA = await _cenario.ClienteAsync(clinicaA, Perfis.Gestor);
            var gestorB = await _cenario.ClienteAsync(clinicaB, Perfis.Gestor);
            var idA = await CadastrarAsync(gestorA, GeradorCpf.Novo(), "Tutor da Clínica A");
            return (gestorA, gestorB, idA);
        }

        #endregion
    }
}
