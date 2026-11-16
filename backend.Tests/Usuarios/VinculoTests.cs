using System.Net;
using System.Net.Http.Json;
using backend.Domain.Usuarios.DTOs.PerfilPessoal;
using backend.Domain.Usuarios.DTOs.Usuario;
using backend.Domain.Utils.Paginacao;
using backend.Tests.Infra;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Usuarios
{
    /// <summary>
    /// Testes de integração dos usuários e vínculos da clínica: api/clinica/usuarios (listar, cadastrar, editar perfis),
    /// api/clinica/vinculos/{upcId}/status e api/vinculos/{upcId} (desvincular, admin). Cobre o usuário global
    /// (e-mail único, vincular existente), isolamento, token de vínculo inativo e paginação.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class VinculoTests
    {
        private const string RotaUsuarios = "/api/clinica/usuarios";
        private const string Senha = "Senha@123";

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public VinculoTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Cadastro e edição pela clínica

        [Fact]
        public async Task Cadastrar_EmailNovo_CriaUsuarioEVinculoNaClinica()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var email = NovoEmail();

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var vinculo = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.Equal(email, vinculo.Email);
            Assert.Equal(clinica.Id, vinculo.ClinicaId);
            Assert.Equal(new[] { Perfis.Veterinario }, vinculo.Perfis);
            Assert.True(vinculo.IsAtivoNaClinica);
        }

        [Fact]
        public async Task Cadastrar_EmailJaExisteEmOutraClinica_VinculaSemAlterarDadosPessoais()
        {
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestorA.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario, "Nome Original"));

            var resposta = await gestorB.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Secretario, "Nome Trocado"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Contains("já tinha cadastro no sistema", await TextoAsync(resposta));
            var emB = Assert.Single((await ListarAsync(gestorB, "")).Itens);
            Assert.Equal("Nome Original", emB.Nome);
            Assert.Equal(new[] { Perfis.Secretario }, emB.Perfis);
            var emA = Assert.Single((await ListarAsync(gestorA, "")).Itens);
            Assert.Equal(new[] { Perfis.Veterinario }, emA.Perfis);
        }

        [Fact]
        public async Task Cadastrar_UsuarioJaVinculadoNaClinica_RetornaConflict()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Tecnico));

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("Este usuário já está vinculado a esta clínica.", await TextoAsync(resposta));
            Assert.Equal(new[] { Perfis.Veterinario }, Assert.Single((await ListarAsync(gestor, "")).Itens).Perfis);
        }

        [Fact]
        public async Task Cadastrar_SemPerfis_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", new { email = NovoEmail(), nome = "Sem perfil", senha = Senha, perfis = Array.Empty<string>() });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Pelo menos um perfil deve ser selecionado.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_EmailInvalido_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario("nao-e-email", Perfis.Tecnico));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O e-mail não é válido.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task EditarPerfis_VinculoDaClinica_AtualizaSoOsPerfis()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = outro.VinculoId, perfis = new[] { Perfis.Veterinario, Perfis.Secretario } });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var vinculo = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.Equal(new[] { Perfis.Veterinario, Perfis.Secretario }, vinculo.Perfis);
            Assert.Equal(outro.Email, vinculo.Email);
        }

        [Fact]
        public async Task EditarPerfis_ListaVazia_RetornaBadRequestENaoAltera()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = outro.VinculoId, perfis = Array.Empty<string>() });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Tecnico }, Assert.Single((await ListarAsync(gestor, "")).Itens).Perfis);
        }

        #endregion

        #region Status do vínculo

        [Fact]
        public async Task MudarStatus_Desativar_MarcaInativoEOTokenDoUsuarioDeixaDeValer()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Veterinario);
            Assert.Equal(HttpStatusCode.OK, (await outro.Cliente.GetAsync("/api/clinica/animais")).StatusCode);

            var resposta = await gestor.PutAsJsonAsync($"/api/clinica/vinculos/{outro.VinculoId}/status", new { activate = false });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.False(Assert.Single((await ListarAsync(gestor, "")).Itens).IsAtivoNaClinica);
            Assert.Equal(HttpStatusCode.Unauthorized, (await outro.Cliente.GetAsync("/api/clinica/animais")).StatusCode);
        }

        [Fact]
        public async Task MudarStatus_Reativar_VoltaAtivoEOTokenVoltaAValer()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Veterinario);
            await gestor.PutAsJsonAsync($"/api/clinica/vinculos/{outro.VinculoId}/status", new { activate = false });

            var resposta = await gestor.PutAsJsonAsync($"/api/clinica/vinculos/{outro.VinculoId}/status", new { activate = true });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.True(Assert.Single((await ListarAsync(gestor, "")).Itens).IsAtivoNaClinica);
            Assert.Equal(HttpStatusCode.OK, (await outro.Cliente.GetAsync("/api/clinica/animais")).StatusCode);
        }

        [Fact]
        public async Task Listar_FiltroAtivoFalse_RetornaSoInativos()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var inativo = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);
            await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);
            await gestor.PutAsJsonAsync($"/api/clinica/vinculos/{inativo.VinculoId}/status", new { activate = false });

            var pagina = await ListarAsync(gestor, "?ativo=false");

            Assert.Equal(inativo.VinculoId, Assert.Single(pagina.Itens).UpcId);
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task EditarPerfis_VinculoDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (clinicaA, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var deA = await _cenario.UsuarioDaClinicaAsync(clinicaA, Perfis.Tecnico);

            var resposta = await gestorB.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = deA.VinculoId, perfis = new[] { Perfis.Gestor } });

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Tecnico }, Assert.Single((await ListarAsync(gestorA, "")).Itens).Perfis);
        }

        [Fact]
        public async Task MudarStatus_VinculoDeOutraClinica_RetornaNotFoundENaoAltera()
        {
            var (clinicaA, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var deA = await _cenario.UsuarioDaClinicaAsync(clinicaA, Perfis.Tecnico);

            var resposta = await gestorB.PutAsJsonAsync($"/api/clinica/vinculos/{deA.VinculoId}/status", new { activate = false });

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.True(Assert.Single((await ListarAsync(gestorA, "")).Itens).IsAtivoNaClinica);
        }

        [Fact]
        public async Task Listar_OutraClinica_NaoMostraUsuariosDeA()
        {
            var (clinicaA, _) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            await _cenario.UsuarioDaClinicaAsync(clinicaA, Perfis.Tecnico);

            Assert.Equal(0, (await ListarAsync(gestorB, "")).Total);
        }

        [Fact]
        public async Task RotasComUpcId_VinculoInexistente_RetornaNotFound()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            const int inexistente = 999999999;

            Assert.Equal(HttpStatusCode.NotFound, (await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = inexistente, perfis = new[] { Perfis.Tecnico } })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await gestor.PutAsJsonAsync($"/api/clinica/vinculos/{inexistente}/status", new { activate = false })).StatusCode);
        }

        #endregion

        #region Perfis

        [Theory]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Secretario)]
        [InlineData(Perfis.Tecnico)]
        public async Task RotasDeUsuarios_PerfilSemGerenciaClinica_RetornaForbidden(string perfil)
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var cliente = await _cenario.ClienteAsync(clinica, perfil);
            var alvo = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.GetAsync(RotaUsuarios)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(NovoEmail(), Perfis.Gestor))).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = alvo.VinculoId, perfis = new[] { Perfis.Gestor } })).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PutAsJsonAsync($"/api/clinica/vinculos/{alvo.VinculoId}/status", new { activate = false })).StatusCode);
            Assert.Contains(Perfis.Tecnico, (await ListarAsync(gestor, $"?busca={Uri.EscapeDataString(alvo.Email)}")).Itens.Single().Perfis!);
        }

        #endregion

        #region Desvincular (admin)

        [Fact]
        public async Task Desvincular_UsuarioComDuasClinicas_RemoveSoAquelaClinica()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestorA.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            await gestorB.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            var upcB = Assert.Single((await ListarAsync(gestorB, "")).Itens).UpcId;

            var resposta = await admin.DeleteAsync($"/api/vinculos/{upcB}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(0, (await ListarAsync(gestorB, "")).Total);
            Assert.Equal(1, (await ListarAsync(gestorA, "")).Total);
        }

        [Fact]
        public async Task Desvincular_UltimoVinculoDoUsuario_RetornaBadRequestENaoRemove()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var unico = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await admin.DeleteAsync($"/api/vinculos/{unico.VinculoId}");

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Não é possível remover o último vínculo do usuário", await TextoAsync(resposta));
            Assert.Equal(1, (await ListarAsync(gestor, "")).Total);
        }

        [Fact]
        public async Task Desvincular_VinculoInexistente_RetornaNotFound()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await admin.DeleteAsync("/api/vinculos/999999999")).StatusCode);
        }

        [Fact]
        public async Task Desvincular_GestorDeClinica_RetornaForbidden()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            Assert.Equal(HttpStatusCode.Forbidden, (await gestor.DeleteAsync($"/api/vinculos/{outro.VinculoId}")).StatusCode);
        }

        #endregion

        #region Rotas removidas

        [Fact]
        public async Task ListarTodosOsVinculos_RotaRemovida_RetornaNotFound()
        {
            // GET api/vinculos devolvia os vínculos de todas as clínicas para qualquer usuário logado; a rota foi removida
            var (_, gestor) = await ClinicaComGestorAsync();
            var admin = await _cenario.ClienteAdminSistemaAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await gestor.GetAsync("/api/vinculos")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync("/api/vinculos")).StatusCode);
        }

        #endregion

        #region Perfis válidos e o próprio vínculo

        [Fact]
        public async Task Cadastrar_PerfilInvalido_RetornaBadRequestComMensagem()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(NovoEmail(), "DONO"));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Perfil inválido: DONO.", await TextoAsync(resposta));
            Assert.Equal(0, (await ListarAsync(gestor, "")).Total);
        }

        [Fact]
        public async Task Cadastrar_PerfilEmMinusculas_GravaNormalizado()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(NovoEmail(), " veterinario "));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Veterinario }, Assert.Single((await ListarAsync(gestor, "")).Itens).Perfis);
        }

        [Fact]
        public async Task EditarPerfis_PerfilInvalido_RetornaBadRequestENaoAltera()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = outro.VinculoId, perfis = new[] { Perfis.Tecnico, "ADMIN" } });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Perfil inválido: ADMIN.", await TextoAsync(resposta));
            Assert.Equal(new[] { Perfis.Tecnico }, Assert.Single((await ListarAsync(gestor, "")).Itens).Perfis);
        }

        [Fact]
        public async Task EditarPerfis_SoInativo_RetornaBadRequest()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = outro.VinculoId, perfis = new[] { "INATIVO" } });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Perfil inválido: INATIVO.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task EditarPerfis_VinculoInativoSemInativoNoEnvio_ContinuaInativo()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);
            await gestor.PutAsJsonAsync($"/api/clinica/vinculos/{outro.VinculoId}/status", new { activate = false });

            // O formulário perde o INATIVO quando o usuário mexe nos checkboxes de perfis
            var resposta = await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = outro.VinculoId, perfis = new[] { Perfis.Veterinario } });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var vinculo = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.False(vinculo.IsAtivoNaClinica);
            Assert.Contains(Perfis.Veterinario, vinculo.Perfis!);
            Assert.DoesNotContain(Perfis.Tecnico, vinculo.Perfis!);
            Assert.Equal(HttpStatusCode.Unauthorized, (await outro.Cliente.GetAsync("/api/clinica/animais")).StatusCode);
        }

        [Fact]
        public async Task EditarPerfis_ComInativoNoEnvio_RetornaBadRequestENaoAltera()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = outro.VinculoId, perfis = new[] { Perfis.Veterinario, "INATIVO" } });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Perfil inválido: INATIVO.", await TextoAsync(resposta));
            var vinculo = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.True(vinculo.IsAtivoNaClinica);
            Assert.Equal(new[] { Perfis.Tecnico }, vinculo.Perfis);
        }

        [Fact]
        public async Task Cadastrar_ComInativoNosPerfis_RetornaBadRequest()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar",
                new { email = NovoEmail(), nome = "Com inativo", senha = Senha, perfis = new[] { Perfis.Tecnico, "INATIVO" } });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Perfil inválido: INATIVO.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Cadastrar_SwitchStatusInativo_CriaVinculoInativo()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            var resposta = await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar",
                new { email = NovoEmail(), nome = "Nasce inativo", senha = Senha, perfis = new[] { Perfis.Tecnico }, isAtivo = false });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var vinculo = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.False(vinculo.IsAtivoNaClinica);
            Assert.Equal(new[] { Perfis.Tecnico }, (await ListarAsync(gestor, "?ativo=false")).Itens.Single().Perfis!.Where(p => p != "INATIVO"));
        }

        [Fact]
        public async Task Cadastrar_SemCampoDeStatus_CriaVinculoAtivo()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(NovoEmail(), Perfis.Tecnico));

            Assert.True(Assert.Single((await ListarAsync(gestor, "")).Itens).IsAtivoNaClinica);
        }

        [Fact]
        public async Task EditarPerfis_GestorTiraOProprioGestor_RetornaBadRequestENaoAltera()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.Cliente.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = gestor.VinculoId, perfis = new[] { Perfis.Veterinario } });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Você não pode remover o seu próprio perfil de GESTOR.", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await gestor.Cliente.GetAsync("/api/clinica/dashboard")).StatusCode);
            Assert.Equal(new[] { Perfis.Gestor }, await PerfisDoVinculoAsync(gestor.VinculoId));
        }

        [Fact]
        public async Task EditarPerfis_GestorAcrescentaPerfilNoProprioVinculo_RetornaOk()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.Cliente.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = gestor.VinculoId, perfis = new[] { Perfis.Gestor, Perfis.Veterinario } });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Gestor, Perfis.Veterinario }, await PerfisDoVinculoAsync(gestor.VinculoId));
        }

        [Fact]
        public async Task EditarPerfis_GestorTiraGestorDeOutroGestor_RetornaOk()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var outroGestor = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = outroGestor.VinculoId, perfis = new[] { Perfis.Secretario } });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Secretario }, await PerfisDoVinculoAsync(outroGestor.VinculoId));
        }

        [Fact]
        public async Task EditarPerfis_AdminSimulandoGestor_TiraGestorSemTrava()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);
            var admin = await _cenario.ClienteAdminSimulandoAsync(clinica, Perfis.Gestor);

            var resposta = await admin.PutAsJsonAsync($"{RotaUsuarios}/editar", new { upcId = gestor.VinculoId, perfis = new[] { Perfis.Veterinario } });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Veterinario }, await PerfisDoVinculoAsync(gestor.VinculoId));
        }

        [Fact]
        public async Task MudarStatus_GestorDesativaOProprioVinculo_RetornaBadRequest()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.Cliente.PutAsJsonAsync($"/api/clinica/vinculos/{gestor.VinculoId}/status", new { activate = false });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Você não pode desativar o seu próprio vínculo.", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await gestor.Cliente.GetAsync("/api/clinica/dashboard")).StatusCode);
        }

        #endregion

        #region Remover usuário da clínica

        [Fact]
        public async Task RemoverDaClinica_UsuarioComDuasClinicas_RemoveSoOVinculoDaClinica()
        {
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestorA.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            await gestorB.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            var usuarioId = Assert.Single((await ListarAsync(gestorB, "")).Itens).Id;

            var resposta = await gestorB.DeleteAsync($"{RotaUsuarios}/{usuarioId}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(0, (await ListarAsync(gestorB, "")).Total);
            Assert.Equal(usuarioId, Assert.Single((await ListarAsync(gestorA, "")).Itens).Id);
            Assert.True(await UsuarioExisteAsync(usuarioId));
        }

        [Fact]
        public async Task RemoverDaClinica_UsuarioSoDeOutraClinica_RetornaNotFoundENaoRemove()
        {
            var (clinicaA, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var deA = await _cenario.UsuarioDaClinicaAsync(clinicaA, Perfis.Tecnico);

            var resposta = await gestorB.DeleteAsync($"{RotaUsuarios}/{deA.UsuarioId}");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Equal(1, (await ListarAsync(gestorA, "")).Total);
        }

        [Fact]
        public async Task RemoverDaClinica_UltimoVinculoDoUsuario_RetornaBadRequestENaoRemove()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var unico = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await gestor.DeleteAsync($"{RotaUsuarios}/{unico.UsuarioId}");

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Não é possível remover o último vínculo do usuário", await TextoAsync(resposta));
            Assert.Equal(1, (await ListarAsync(gestor, "")).Total);
        }

        [Fact]
        public async Task RemoverDaClinica_ProprioVinculo_RetornaBadRequest()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await gestor.Cliente.DeleteAsync($"{RotaUsuarios}/{gestor.UsuarioId}");

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Você não pode remover o seu próprio vínculo com a clínica.", await TextoAsync(resposta));
        }

        [Theory]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Secretario)]
        [InlineData(Perfis.Tecnico)]
        public async Task RemoverDaClinica_PerfilSemGerenciaClinica_RetornaForbidden(string perfil)
        {
            var (clinica, _) = await ClinicaComGestorAsync();
            var cliente = await _cenario.ClienteAsync(clinica, perfil);
            var alvo = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.DeleteAsync($"{RotaUsuarios}/{alvo.UsuarioId}")).StatusCode);
        }

        [Fact]
        public async Task RemoverDaClinica_AdminSimulandoGestor_RemoveOVinculo()
        {
            var (clinicaA, _) = await ClinicaComGestorAsync();
            var (clinicaB, gestorB) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestorB.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Tecnico));
            var usuarioId = Assert.Single((await ListarAsync(gestorB, "")).Itens).Id;
            var gestorA = await _cenario.ClienteAsync(clinicaA, Perfis.Gestor);
            await gestorA.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Tecnico));
            var admin = await _cenario.ClienteAdminSimulandoAsync(clinicaB, Perfis.Gestor);

            var resposta = await admin.DeleteAsync($"{RotaUsuarios}/{usuarioId}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(0, (await ListarAsync(gestorB, "")).Total);
        }

        #endregion

        #region Excluir usuário (admin)

        [Fact]
        public async Task ExcluirUsuarioSistema_SemHistorico_ApagaUsuarioETodosOsVinculos()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestorA.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            await gestorB.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Secretario));
            var usuarioId = Assert.Single((await ListarAsync(gestorA, "")).Itens).Id;

            var resposta = await admin.DeleteAsync($"/api/sistema/usuarios/{usuarioId}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.False(await UsuarioExisteAsync(usuarioId));
            Assert.Equal(0, (await ListarAsync(gestorA, "")).Total);
            Assert.Equal(0, (await ListarAsync(gestorB, "")).Total);
        }

        [Fact]
        public async Task ExcluirUsuarioSistema_ComConsultaMarcada_RetornaConflictENaoApagaNada()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var secretario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Secretario);
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            var animalId = await CadastrarAnimalAsync(gestor, racaId);
            await CadastrarConsultaAsync(secretario.Cliente, animalId);

            var resposta = await admin.DeleteAsync($"/api/sistema/usuarios/{secretario.UsuarioId}");

            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
            Assert.Contains("tem consultas ou avaliações registradas", await TextoAsync(resposta));
            Assert.True(await UsuarioExisteAsync(secretario.UsuarioId));
            Assert.Equal(1, (await ListarAsync(gestor, "")).Total);
        }

        [Fact]
        public async Task ExcluirUsuarioSistema_Inexistente_RetornaNotFound()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await admin.DeleteAsync("/api/sistema/usuarios/999999999")).StatusCode);
        }

        [Fact]
        public async Task ExcluirUsuarioSistema_GestorDeClinica_RetornaForbidden()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var alvo = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            Assert.Equal(HttpStatusCode.Forbidden, (await gestor.DeleteAsync($"/api/sistema/usuarios/{alvo.UsuarioId}")).StatusCode);
            Assert.True(await UsuarioExisteAsync(alvo.UsuarioId));
        }

        #endregion

        #region Perfis pelo admin (sistema/usuarios)

        [Theory]
        [InlineData("DONO")]
        [InlineData("INATIVO")]
        public async Task EditarUsuarioSistema_PerfilInvalido_RetornaBadRequestENaoAltera(string perfil)
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var alvo = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var resposta = await admin.PutAsJsonAsync("/api/sistema/usuarios/editar",
                new { id = alvo.UsuarioId, upcId = alvo.VinculoId, nome = "Alvo", email = alvo.Email, perfis = new[] { Perfis.Veterinario, perfil }, isAdmin = false, isAtivo = true });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains($"Perfil inválido: {perfil}.", await TextoAsync(resposta));
            Assert.Equal(new[] { Perfis.Tecnico }, await PerfisDoVinculoAsync(alvo.VinculoId));
        }

        [Fact]
        public async Task EditarUsuarioSistema_PerfisValidos_NormalizaEMantemStatus()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var alvo = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);
            await gestor.PutAsJsonAsync($"/api/clinica/vinculos/{alvo.VinculoId}/status", new { activate = false });

            // isAtivo = true no corpo não reativa: o status muda só pela rota de status
            var resposta = await admin.PutAsJsonAsync("/api/sistema/usuarios/editar",
                new { id = alvo.UsuarioId, upcId = alvo.VinculoId, nome = "Alvo Editado", email = alvo.Email, perfis = new[] { "gestor", " veterinario" }, isAdmin = false, isAtivo = true });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Gestor, Perfis.Veterinario, "INATIVO" }, await PerfisDoVinculoAsync(alvo.VinculoId));
            Assert.False(Assert.Single((await ListarAsync(gestor, "")).Itens).IsAtivoNaClinica);
        }

        [Fact]
        public async Task EditarUsuarioSistema_GestorTirandoOProprioGestorPeloAdmin_SemTrava()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await admin.PutAsJsonAsync("/api/sistema/usuarios/editar",
                new { id = gestor.UsuarioId, upcId = gestor.VinculoId, nome = "Ex-gestor", email = gestor.Email, perfis = new[] { Perfis.Secretario }, isAdmin = false, isAtivo = true });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(new[] { Perfis.Secretario }, await PerfisDoVinculoAsync(gestor.VinculoId));
        }

        [Fact]
        public async Task CadastrarUsuarioSistema_AdminComPerfilDaClinica_RetornaBadRequest()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();

            var resposta = await admin.PostAsJsonAsync("/api/sistema/usuarios/cadastrar",
                new { email = NovoEmail(), nome = "Misturado", senha = Senha, perfis = new[] { "ADMIN", Perfis.Gestor } });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O perfil ADMIN não pode ser combinado com perfis da clínica.", await TextoAsync(resposta));
        }

        [Theory]
        [InlineData("DONO")]
        [InlineData("INATIVO")]
        public async Task CadastrarUsuarioSistema_PerfilInvalido_RetornaBadRequest(string perfil)
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinica = await _cenario.CriarClinicaAsync("A");

            var resposta = await admin.PostAsJsonAsync("/api/sistema/usuarios/cadastrar",
                new { email = NovoEmail(), nome = "Perfil ruim", senha = Senha, perfis = new[] { Perfis.Tecnico, perfil }, clinicaIds = new[] { clinica.Id } });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains($"Perfil inválido: {perfil}.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task ListarUsuariosSistema_VinculoInativoEmUmaClinica_MostraStatusPorVinculo()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (_, gestorA) = await ClinicaComGestorAsync();
            var (_, gestorB) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestorA.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            await gestorB.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            var upcB = Assert.Single((await ListarAsync(gestorB, "")).Itens).UpcId;
            await gestorB.PutAsJsonAsync($"/api/clinica/vinculos/{upcB}/status", new { activate = false });

            var resposta = await admin.GetAsync("/api/sistema/usuarios");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var linhas = (await LerAsync<ResultadoComDados<List<GetUsuariosSistemaDto>>>(resposta)).Data.Where(u => u.Email == email).ToList();
            Assert.Equal(2, linhas.Count);
            Assert.False(linhas.Single(l => l.UpcId == upcB).IsAtivo);
            Assert.True(linhas.Single(l => l.UpcId != upcB).IsAtivo);
        }

        [Fact]
        public async Task CadastrarUsuarioSistema_UsuarioDeClinicaInativo_CriaVinculoInativo()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var (clinica, gestor) = await ClinicaComGestorAsync();

            var resposta = await admin.PostAsJsonAsync("/api/sistema/usuarios/cadastrar",
                new { email = NovoEmail(), nome = "Inativo pelo admin", senha = Senha, perfis = new[] { "tecnico" }, clinicaIds = new[] { clinica.Id }, isAtivo = false });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var vinculo = Assert.Single((await ListarAsync(gestor, "")).Itens);
            Assert.False(vinculo.IsAtivoNaClinica);
            Assert.Contains(Perfis.Tecnico, vinculo.Perfis!);
        }

        #endregion

        #region Paginação

        [Fact]
        public async Task Listar_NaoMostraOProprioVinculoEPaginaMaisRecentesPrimeiro()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var ids = new List<int>();
            for (var i = 0; i < 12; i++) ids.Add((await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico)).VinculoId);

            var pagina = await ListarAsync(gestor, "");

            Assert.Equal(12, pagina.Total);
            Assert.Equal(10, pagina.TamanhoPagina);
            Assert.Equal(ids.AsEnumerable().Reverse().Take(10), pagina.Itens.Select(v => v.UpcId));
        }

        [Fact]
        public async Task Listar_FiltroPerfil_RetornaSoQuemTemOPerfil()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var vet = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Veterinario);
            await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var pagina = await ListarAsync(gestor, "?perfil=veterinario");

            Assert.Equal(vet.VinculoId, Assert.Single(pagina.Itens).UpcId);
        }

        [Fact]
        public async Task Listar_TamanhoPaginaAcimaDoMaximo_LimitaA100()
        {
            var (_, gestor) = await ClinicaComGestorAsync();

            Assert.Equal(100, (await ListarAsync(gestor, "?tamanhoPagina=1000")).TamanhoPagina);
        }

        #endregion

        #region Perfil pessoal (sistema/perfil)

        [Fact]
        public async Task AtualizarMeuPerfil_SenhaAtualIncorreta_RetornaBadRequestComMensagemEmPortugues()
        {
            var cliente = await ClienteComSenhaParaPerfilAsync();

            var resposta = await cliente.PutAsJsonAsync("/api/sistema/perfil",
                new { senhaAtual = "Errada@123", novaSenha = "NovaSenha@123" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A senha atual está incorreta.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task AtualizarMeuPerfil_SenhaAtualCorreta_AlteraSenhaComSucesso()
        {
            var cliente = await ClienteComSenhaParaPerfilAsync();

            var resposta = await cliente.PutAsJsonAsync("/api/sistema/perfil",
                new { senhaAtual = Senha, novaSenha = "NovaSenha@123" });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

            // Trocar a senha derruba as sessões antigas; quem trocou recebe um cookie novo e continua logado
            var cookieNovo = CookieDaSessao(resposta);
            Assert.NotNull(cookieNovo);
            var clienteRenovado = _cenario.ClienteComCookie(cookieNovo!);

            // A senha antiga deixa de valer na próxima tentativa
            var segunda = await clienteRenovado.PutAsJsonAsync("/api/sistema/perfil",
                new { senhaAtual = Senha, novaSenha = "OutraSenha@123" });
            Assert.Equal(HttpStatusCode.BadRequest, segunda.StatusCode);

            // O token de antes da troca não vale mais
            var comTokenAntigo = await cliente.GetAsync("/api/sistema/perfil");
            Assert.Equal(HttpStatusCode.Unauthorized, comTokenAntigo.StatusCode);
        }

        private async Task<HttpClient> ClienteComSenhaParaPerfilAsync()
        {
            var (_, gestor) = await ClinicaComGestorAsync();
            var email = NovoEmail();
            await gestor.PostAsJsonAsync($"{RotaUsuarios}/cadastrar", NovoUsuario(email, Perfis.Veterinario));
            var usuarioId = Assert.Single((await ListarAsync(gestor, "")).Itens).Id;

            return await _cenario.ClienteDeLoginAsync(usuarioId);
        }

        #endregion

        #region Campos de apoio

        [Fact]
        public async Task ListarUsuariosSistema_LinhaDeVinculo_TrazDadosDoUsuarioEDaClinica()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var clinica = await _cenario.CriarClinicaAsync("A");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Veterinario, Perfis.Secretario);

            var resposta = await admin.GetAsync("/api/sistema/usuarios");

            var linha = (await LerAsync<ResultadoComDados<List<GetUsuariosSistemaDto>>>(resposta)).Data.Single(u => u.UpcId == usuario.VinculoId);
            await using var contexto = _factory.CriarContexto();
            var gravado = await contexto.Usuarios.AsNoTracking().FirstAsync(u => u.Id == usuario.UsuarioId);
            Assert.Equal($"link-{usuario.VinculoId}", linha.CompositeKey);
            Assert.Equal(usuario.UsuarioId, linha.Id);
            Assert.Equal(clinica.Id, linha.ClinicaId);
            Assert.Equal(clinica.Nome, linha.NomeClinica);
            Assert.Equal(usuario.Email, linha.Email);
            Assert.Equal(usuario.Email, linha.UserName);
            Assert.Equal(gravado.Nome, linha.Nome);
            Assert.Equal(new[] { Perfis.Veterinario, Perfis.Secretario }, linha.Perfis);
            Assert.False(linha.IsAdmin);
            Assert.True(linha.IsAtivo);
            Assert.Null(linha.Cpf);
            Assert.Null(linha.Formacao);
            Assert.Null(linha.Telefone);
            Assert.Null(linha.Whatsapp);
            Assert.InRange(linha.DataCadastro.ToUniversalTime(), gravado.DataCadastro.AddSeconds(-1), gravado.DataCadastro.AddSeconds(1));
            Assert.NotNull(linha.DataUltimoAcesso);
        }

        [Fact]
        public async Task ListarUsuariosSistema_OutroAdminSemVinculo_TrazLinhaDeAdmin()
        {
            // A lista não traz o próprio admin que está consultando, só os outros
            var admin = await _cenario.ClienteAdminSistemaAsync();
            var outroAdmin = await _cenario.ClienteAdminSistemaAsync();
            var eu = (await LerAsync<ResultadoComDados<GetMeuPerfilDto>>(await outroAdmin.GetAsync("/api/sistema/perfil"))).Data;

            var resposta = await admin.GetAsync("/api/sistema/usuarios");

            var linha = (await LerAsync<ResultadoComDados<List<GetUsuariosSistemaDto>>>(resposta)).Data.Single(u => u.CompositeKey == $"admin-{eu.Id}");
            Assert.Equal(eu.Id, linha.Id);
            Assert.Equal(-1, linha.UpcId);
            Assert.Equal(-1, linha.ClinicaId);
            Assert.Equal("N/A", linha.NomeClinica);
            Assert.Equal(eu.Email, linha.Email);
            Assert.Equal(eu.Nome, linha.Nome);
            Assert.Equal(new[] { "ADMIN" }, linha.Perfis);
            Assert.True(linha.IsAdmin);
            Assert.True(linha.IsAtivo);
        }

        [Fact]
        public async Task Listar_UsuarioDaClinica_TrazDadosDoUsuarioEDoVinculo()
        {
            var (clinica, gestor) = await ClinicaComGestorAsync();
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);

            var linha = Assert.Single((await ListarAsync(gestor, "")).Itens);

            await using var contexto = _factory.CriarContexto();
            var gravado = await contexto.Usuarios.AsNoTracking().FirstAsync(u => u.Id == usuario.UsuarioId);
            Assert.Equal(usuario.UsuarioId, linha.Id);
            Assert.Equal(usuario.VinculoId, linha.UpcId);
            Assert.Equal(clinica.Id, linha.ClinicaId);
            Assert.Equal(usuario.Email, linha.Email);
            Assert.Equal(usuario.Email, linha.UserName);
            Assert.Equal(gravado.Nome, linha.Nome);
            Assert.Equal(new[] { Perfis.Tecnico }, linha.Perfis);
            Assert.True(linha.IsAtivoNaClinica);
            Assert.Null(linha.Cpf);
            Assert.Null(linha.Formacao);
            Assert.Null(linha.Telefone);
            Assert.Null(linha.Whatsapp);
            Assert.InRange(linha.DataCadastro.ToUniversalTime(), gravado.DataCadastro.AddSeconds(-1), gravado.DataCadastro.AddSeconds(1));
            Assert.InRange(linha.DataUltimoAcesso.ToUniversalTime(), gravado.DataUltimoAcesso.AddSeconds(-1), gravado.DataUltimoAcesso.AddSeconds(1));
        }

        #endregion

        #region Auxiliares

        private async Task<(ClinicaDeTeste clinica, HttpClient gestor)> ClinicaComGestorAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            return (clinica, await _cenario.ClienteAsync(clinica, Perfis.Gestor));
        }

        private async Task<bool> UsuarioExisteAsync(int usuarioId)
        {
            await using var contexto = _factory.CriarContexto();
            return await contexto.Usuarios.AnyAsync(u => u.Id == usuarioId);
        }

        private async Task<List<string>?> PerfisDoVinculoAsync(int vinculoId)
        {
            await using var contexto = _factory.CriarContexto();
            return (await contexto.UsuariosPerfisClinicas.AsNoTracking().FirstAsync(v => v.Id == vinculoId)).Perfis;
        }

        private static string NovoEmail() => $"pessoa.{Cenario.Unico()}@teste.local";

        private static object NovoUsuario(string email, string perfil, string? nome = null) => new
        {
            email,
            nome = nome ?? $"Pessoa {Cenario.Unico()}",
            senha = Senha,
            perfis = new[] { perfil },
        };

        private static async Task<PaginaResultado<GetUsuariosClinicaDto>> ListarAsync(HttpClient cliente, string query)
        {
            var resposta = await cliente.GetAsync($"{RotaUsuarios}{query}");
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return await LerAsync<PaginaResultado<GetUsuariosClinicaDto>>(resposta);
        }

        #endregion
    }
}
