using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using backend.Tests.Infra;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Auth
{
    /// <summary>
    /// Testes de integração da autenticação (api/auth): login e cookie, bloqueio por tentativas, escolha de vínculo,
    /// logout, revogação de sessão conferida no banco, checagem de origem (CSRF), rate limit e recuperação de senha.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class AuthTests
    {
        private const string Senha = "Senha@123";
        private const string RotaLogin = "/api/auth/login";
        private const string RotaCheck = "/api/auth/check";
        private const string RotaEsqueci = "/api/auth/forgot-password";
        private const string RotaRedefinir = "/api/auth/reset-password";
        private const string MensagemLinkInvalido = "O link de recuperação é inválido ou expirou. Solicite um novo.";

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public AuthTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Login e cookie

        [Fact]
        public async Task Login_CredenciaisValidas_DevolveCookieProtegidoESemTokenNoCorpo()
        {
            var usuario = await NovoUsuarioAsync();

            var resposta = await LoginAsync(usuario.Email, usuario.Senha);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

            var cookie = CabecalhoDoCookieDaSessao(resposta)!.ToLowerInvariant();
            Assert.Contains("httponly", cookie);
            Assert.Contains("samesite=strict", cookie);
            Assert.DoesNotContain("expires=", cookie); // sem "lembrar-me" o cookie some ao fechar o navegador

            // O JWT não pode ir no corpo: lá um script da página conseguiria ler
            var corpo = await TextoAsync(resposta);
            Assert.DoesNotContain("eyJ", corpo);
            Assert.DoesNotContain("token", corpo, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Login_ComLembrarMe_CookieTemValidadeEVinculoHerda()
        {
            var usuario = await NovoUsuarioAsync();

            var login = await LoginAsync(usuario.Email, usuario.Senha, lembrar: true);
            Assert.Contains("expires=", CabecalhoDoCookieDaSessao(login)!.ToLowerInvariant());

            // O token de vínculo continua sendo de "lembrar-me"
            var logado = _cenario.ClienteComCookie(CookieDaSessao(login)!);
            var vinculo = await logado.PostAsync($"/api/auth/sistema/{usuario.VinculoId}", null);
            Assert.Equal(HttpStatusCode.OK, vinculo.StatusCode);
            Assert.Contains("expires=", CabecalhoDoCookieDaSessao(vinculo)!.ToLowerInvariant());
        }

        [Fact]
        public async Task Login_SenhaErrada_RetornaUnauthorizedSemCookie()
        {
            var usuario = await NovoUsuarioAsync();

            var resposta = await LoginAsync(usuario.Email, "Errada@123");

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
            Assert.Null(CookieDaSessao(resposta));
        }

        [Fact]
        public async Task Login_EmailInexistente_MesmaMensagemDaSenhaErrada()
        {
            var usuario = await NovoUsuarioAsync();

            var senhaErrada = await LoginAsync(usuario.Email, "Errada@123");
            var emailInexistente = await LoginAsync($"ninguem.{Cenario.Unico()}@teste.local", Senha);

            Assert.Equal(HttpStatusCode.Unauthorized, emailInexistente.StatusCode);
            Assert.Equal(await TextoAsync(senhaErrada), await TextoAsync(emailInexistente));
        }

        [Fact]
        public async Task Login_CaptchaInvalido_RetornaUnauthorized()
        {
            var usuario = await NovoUsuarioAsync();

            var resposta = await LoginAsync(usuario.Email, usuario.Senha, captcha: TurnstileFalso.TokenInvalido);

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
            Assert.Null(CookieDaSessao(resposta));
        }

        [Fact]
        public async Task Login_UsuarioInativo_RetornaUnauthorized()
        {
            var usuario = await NovoUsuarioAsync();
            await AlterarUsuarioAsync(usuario.UsuarioId, u => u.IsAtivo = false);

            var resposta = await LoginAsync(usuario.Email, usuario.Senha);

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
            Assert.Null(CookieDaSessao(resposta));
        }

        [Fact]
        public async Task Login_CincoSenhasErradas_BloqueiaAteASenhaCerta()
        {
            var usuario = await NovoUsuarioAsync();
            var cliente = _cenario.ClienteAnonimo();

            for (var tentativa = 0; tentativa < 5; tentativa++)
                Assert.Equal(HttpStatusCode.Unauthorized, (await LoginAsync(usuario.Email, "Errada@123", cliente: cliente)).StatusCode);

            var comSenhaCerta = await LoginAsync(usuario.Email, usuario.Senha, cliente: cliente);

            Assert.Equal(HttpStatusCode.Unauthorized, comSenhaCerta.StatusCode);
            Assert.Null(CookieDaSessao(comSenhaCerta));
        }

        [Fact]
        public async Task Login_SenhaCertaAntesDoLimite_ZeraAContagemDeFalhas()
        {
            var usuario = await NovoUsuarioAsync();
            var cliente = _cenario.ClienteAnonimo();

            for (var tentativa = 0; tentativa < 4; tentativa++)
                await LoginAsync(usuario.Email, "Errada@123", cliente: cliente);
            Assert.Equal(HttpStatusCode.OK, (await LoginAsync(usuario.Email, usuario.Senha, cliente: cliente)).StatusCode);

            // Mais 4 erros depois do acerto ainda não bloqueiam
            for (var tentativa = 0; tentativa < 4; tentativa++)
                await LoginAsync(usuario.Email, "Errada@123", cliente: cliente);
            Assert.Equal(HttpStatusCode.OK, (await LoginAsync(usuario.Email, usuario.Senha, cliente: cliente)).StatusCode);
        }

        #endregion

        #region Vínculos

        [Fact]
        public async Task SelecionarVinculo_DoProprioUsuario_EntraNaClinicaComOsPerfis()
        {
            var clinica = await _cenario.CriarClinicaAsync("Vínculo");
            var usuario = await _cenario.UsuarioComSenhaAsync(clinica, Senha, Perfis.Veterinario);
            var logado = _cenario.ClienteComCookie(CookieDaSessao(await LoginAsync(usuario.Email, usuario.Senha))!);

            var statusLogin = await LerAsync<JsonElement>(await logado.GetAsync(RotaCheck));
            Assert.Equal("logado", statusLogin.GetProperty("status").GetString());

            var resposta = await logado.PostAsync($"/api/auth/sistema/{usuario.VinculoId}", null);
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

            var naClinica = _cenario.ClienteComCookie(CookieDaSessao(resposta)!);
            var status = await LerAsync<JsonElement>(await naClinica.GetAsync(RotaCheck));
            Assert.Equal("autenticado", status.GetProperty("status").GetString());
            Assert.Equal(clinica.Nome, status.GetProperty("nomeClinica").GetString());
            Assert.Equal(Perfis.Veterinario, Assert.Single(status.GetProperty("perfis").EnumerateArray()).GetString());
        }

        [Fact]
        public async Task SelecionarVinculo_DeOutroUsuario_RetornaNotFoundSemCookie()
        {
            var clinica = await _cenario.CriarClinicaAsync("Alheio");
            var usuario = await _cenario.UsuarioComSenhaAsync(clinica, Senha, Perfis.Tecnico);
            var outro = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);
            var logado = _cenario.ClienteComCookie(CookieDaSessao(await LoginAsync(usuario.Email, usuario.Senha))!);

            var resposta = await logado.PostAsync($"/api/auth/sistema/{outro.VinculoId}", null);

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Null(CookieDaSessao(resposta));
        }

        [Fact]
        public async Task ListarVinculos_ComTokenDeVinculo_DevolveSoOsDoProprioUsuario()
        {
            // No token de vínculo o NameIdentifier é o id do vínculo: a lista tem de sair pelo id do usuário
            var clinicaA = await _cenario.CriarClinicaAsync("Lista A");
            var clinicaB = await _cenario.CriarClinicaAsync("Lista B");
            await _cenario.UsuarioDaClinicaAsync(clinicaB, Perfis.Gestor); // desloca os ids de vínculo e usuário
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinicaA, Perfis.Secretario);

            var resposta = await usuario.Cliente.GetAsync("/api/auth/sistema");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var vinculo = Assert.Single((await LerAsync<JsonElement>(resposta)).EnumerateArray());
            Assert.Equal(usuario.VinculoId, vinculo.GetProperty("id").GetInt32());
            Assert.Equal(usuario.UsuarioId, vinculo.GetProperty("usuarioId").GetInt32());
            Assert.Equal(clinicaA.Id, vinculo.GetProperty("clinicaId").GetInt32());
        }

        [Fact]
        public async Task TrocarVinculo_VoltaAoTokenDeLoginDoMesmoUsuario()
        {
            var clinica = await _cenario.CriarClinicaAsync("Troca");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await usuario.Cliente.PostAsync("/api/auth/sistema/trocar-vinculo", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var deLogin = _cenario.ClienteComCookie(CookieDaSessao(resposta)!);
            var status = await LerAsync<JsonElement>(await deLogin.GetAsync(RotaCheck));
            Assert.Equal("logado", status.GetProperty("status").GetString());
        }

        #endregion

        #region Logout e revogação de sessão

        [Fact]
        public async Task Logout_ApagaOCookieEInvalidaOTokenNoServidor()
        {
            var usuario = await NovoUsuarioAsync();
            var cookie = CookieDaSessao(await LoginAsync(usuario.Email, usuario.Senha))!;
            var logado = _cenario.ClienteComCookie(cookie);
            Assert.Equal(HttpStatusCode.OK, (await logado.GetAsync(RotaCheck)).StatusCode);

            var logout = await logado.PostAsync("/api/auth/logout", null);

            Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
            Assert.NotNull(CabecalhoDoCookieDaSessao(logout)); // manda o navegador apagar
            Assert.Null(CookieDaSessao(logout));

            // Uma cópia do token antigo não vale mais
            Assert.Equal(HttpStatusCode.Unauthorized, (await _cenario.ClienteComCookie(cookie).GetAsync(RotaCheck)).StatusCode);
        }

        [Fact]
        public async Task Logout_DerrubaAsOutrasSessoesDoUsuario()
        {
            var usuario = await NovoUsuarioAsync();
            var celular = _cenario.ClienteComCookie(CookieDaSessao(await LoginAsync(usuario.Email, usuario.Senha))!);
            var computador = _cenario.ClienteComCookie(CookieDaSessao(await LoginAsync(usuario.Email, usuario.Senha))!);

            await computador.PostAsync("/api/auth/logout", null);

            Assert.Equal(HttpStatusCode.Unauthorized, (await celular.GetAsync(RotaCheck)).StatusCode);
        }

        [Fact]
        public async Task Logout_PorGet_NaoEAceito()
        {
            var clinica = await _cenario.CriarClinicaAsync("Logout GET");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var resposta = await usuario.Cliente.GetAsync("/api/auth/logout");

            Assert.Equal(HttpStatusCode.MethodNotAllowed, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await usuario.Cliente.GetAsync(RotaCheck)).StatusCode); // sessão continua
        }

        [Fact]
        public async Task Sessao_UsuarioDesativado_TokenDeixaDeValer()
        {
            var clinica = await _cenario.CriarClinicaAsync("Desativado");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);
            Assert.Equal(HttpStatusCode.OK, (await usuario.Cliente.GetAsync(RotaCheck)).StatusCode);

            await AlterarUsuarioAsync(usuario.UsuarioId, u => u.IsAtivo = false);

            Assert.Equal(HttpStatusCode.Unauthorized, (await usuario.Cliente.GetAsync(RotaCheck)).StatusCode);
        }

        [Fact]
        public async Task Sessao_PerfilRemovido_PerdeOAcessoNaRequisicaoSeguinte()
        {
            var clinica = await _cenario.CriarClinicaAsync("Perfil removido");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);
            Assert.Equal(HttpStatusCode.OK, (await usuario.Cliente.GetAsync("/api/clinica/usuarios")).StatusCode);

            // O token ainda diz GESTOR, mas o banco agora diz TECNICO
            await using (var contexto = _factory.CriarContexto())
            {
                var vinculo = await contexto.UsuariosPerfisClinicas.FirstAsync(v => v.Id == usuario.VinculoId);
                vinculo.Perfis = new List<string> { Perfis.Tecnico };
                await contexto.SaveChangesAsync();
            }

            Assert.Equal(HttpStatusCode.Forbidden, (await usuario.Cliente.GetAsync("/api/clinica/usuarios")).StatusCode);
            var status = await LerAsync<JsonElement>(await usuario.Cliente.GetAsync(RotaCheck));
            Assert.Equal(Perfis.Tecnico, Assert.Single(status.GetProperty("perfis").EnumerateArray()).GetString());
        }

        [Fact]
        public async Task Sessao_AdminRebaixado_TokenDeAdminDeixaDeValer()
        {
            var clinica = await _cenario.CriarClinicaAsync("Rebaixado");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);
            await AlterarUsuarioAsync(usuario.UsuarioId, u => u.IsAdmin = true);
            var stamp = await StampAsync(usuario.UsuarioId);
            var admin = _cenario.ClienteComToken(TokenDeTeste.AdminSistema(usuario.UsuarioId, "Admin de teste", stamp));
            Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/sistema/clinicas")).StatusCode);

            await AlterarUsuarioAsync(usuario.UsuarioId, u => u.IsAdmin = false);

            Assert.Equal(HttpStatusCode.Unauthorized, (await admin.GetAsync("/api/sistema/clinicas")).StatusCode);
        }

        [Fact]
        public async Task Sessao_TokenSemCarimboOuComCarimboErrado_Recusado()
        {
            var clinica = await _cenario.CriarClinicaAsync("Carimbo");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var semCarimbo = _cenario.ClienteComToken(TokenDeTeste.Login(usuario.UsuarioId, "Teste", isAdmin: false, stamp: null));
            var carimboErrado = _cenario.ClienteComToken(TokenDeTeste.Login(usuario.UsuarioId, "Teste", isAdmin: false, stamp: "outro-carimbo"));

            Assert.Equal(HttpStatusCode.Unauthorized, (await semCarimbo.GetAsync(RotaCheck)).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await carimboErrado.GetAsync(RotaCheck)).StatusCode);
        }

        [Fact]
        public async Task Renovacao_TokenPertoDeExpirar_GanhaCookieNovo()
        {
            var (usuarioId, stamp) = await UsuarioComStampAsync();
            var cliente = _cenario.ClienteComToken(
                TokenDeTeste.Login(usuarioId, "Teste", isAdmin: false, stamp, validade: TimeSpan.FromMinutes(2)));

            var resposta = await cliente.GetAsync(RotaCheck);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var renovado = CookieDaSessao(resposta);
            Assert.NotNull(renovado);
            Assert.Equal(HttpStatusCode.OK, (await _cenario.ClienteComCookie(renovado!).GetAsync(RotaCheck)).StatusCode);
        }

        [Fact]
        public async Task Renovacao_SessaoPassouDoTetoDe30Dias_NaoRenova()
        {
            var (usuarioId, stamp) = await UsuarioComStampAsync();
            var cliente = _cenario.ClienteComToken(TokenDeTeste.Login(usuarioId, "Teste", isAdmin: false, stamp,
                rememberMe: true, loginEm: DateTimeOffset.UtcNow.AddDays(-31), validade: TimeSpan.FromMinutes(2)));

            var resposta = await cliente.GetAsync(RotaCheck);

            // O token ainda vale pelos minutos que restam, mas não ganha vida nova
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Null(CabecalhoDoCookieDaSessao(resposta));
        }

        #endregion

        #region Origem (CSRF) e rate limit

        [Theory]
        [InlineData("/api/auth/check")]      // 401 sem sessão: os cabeçalhos vão também nos erros
        [InlineData("/api/rota-inexistente")] // 404
        public async Task CabecalhosDeSeguranca_VaoEmTodasAsRespostas(string rota)
        {
            var resposta = await _cenario.ClienteAnonimo().GetAsync(rota);

            Assert.Equal("nosniff", Assert.Single(resposta.Headers.GetValues("X-Content-Type-Options")));
            Assert.Equal("DENY", Assert.Single(resposta.Headers.GetValues("X-Frame-Options")));
            Assert.Equal("strict-origin-when-cross-origin", Assert.Single(resposta.Headers.GetValues("Referrer-Policy")));
            Assert.Equal("max-age=31536000", Assert.Single(resposta.Headers.GetValues("Strict-Transport-Security")));
        }

        [Fact]
        public async Task Origem_DeOutroSite_RequisicaoQueAlteraDadosERecusada()
        {
            var clinica = await _cenario.CriarClinicaAsync("Origem");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var requisicao = new HttpRequestMessage(HttpMethod.Post, "/api/auth/sistema/trocar-vinculo");
            requisicao.Headers.Add("Origin", "https://site-malicioso.ufg.br");
            var resposta = await usuario.Cliente.SendAsync(requisicao);

            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
            Assert.Null(CabecalhoDoCookieDaSessao(resposta)); // nada mudou na sessão
        }

        [Fact]
        public async Task Origem_DoProprioSistema_Passa()
        {
            var clinica = await _cenario.CriarClinicaAsync("Origem ok");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor);

            var requisicao = new HttpRequestMessage(HttpMethod.Post, "/api/auth/sistema/trocar-vinculo");
            requisicao.Headers.Add("Origin", "https://imperium.evz.ufg.br");
            var resposta = await usuario.Cliente.SendAsync(requisicao);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task RateLimit_MuitasRequisicoesDoMesmoIp_RetornaTooManyRequests()
        {
            var cliente = _cenario.ClienteAnonimo(); // um IP só
            var corpo = new { email = $"ninguem.{Cenario.Unico()}@teste.local", captchaToken = TurnstileFalso.TokenValido };

            for (var i = 0; i < ImperiumApiFactory.LimiteAuth; i++)
                Assert.Equal(HttpStatusCode.OK, (await cliente.PostAsJsonAsync(RotaEsqueci, corpo)).StatusCode);

            var excedente = await cliente.PostAsJsonAsync(RotaEsqueci, corpo);
            Assert.Equal(HttpStatusCode.TooManyRequests, excedente.StatusCode);

            // Outro IP não é afetado
            Assert.Equal(HttpStatusCode.OK, (await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaEsqueci, corpo)).StatusCode);
        }

        #endregion

        #region Recuperação de senha

        [Fact]
        public async Task EsqueciSenha_EmailInexistente_RespondeIgualAoCadastradoESemEnviar()
        {
            var usuario = await NovoUsuarioAsync();
            var inexistente = $"ninguem.{Cenario.Unico()}@teste.local";

            var cadastrado = await EsqueciAsync(usuario.Email);
            var naoCadastrado = await EsqueciAsync(inexistente);

            Assert.Equal(HttpStatusCode.OK, cadastrado.StatusCode);
            Assert.Equal(HttpStatusCode.OK, naoCadastrado.StatusCode);
            Assert.Equal(await TextoAsync(cadastrado), await TextoAsync(naoCadastrado));

            Assert.NotNull(await _factory.Emails.EsperarAsync(usuario.Email, "Redefinição"));
            Assert.Empty(_factory.Emails.Para(inexistente));
        }

        [Fact]
        public async Task EsqueciSenha_CaptchaInvalidoOuAusente_RetornaBadRequestSemEnviar()
        {
            var usuario = await NovoUsuarioAsync();

            var invalido = await EsqueciAsync(usuario.Email, TurnstileFalso.TokenInvalido);
            var ausente = await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaEsqueci, new { email = usuario.Email });

            Assert.Equal(HttpStatusCode.BadRequest, invalido.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, ausente.StatusCode);
            await Task.Delay(200);
            Assert.Empty(_factory.Emails.Para(usuario.Email));
        }

        [Fact]
        public async Task EsqueciSenha_LinkLevaIdDoUsuarioENaoOEmail()
        {
            var usuario = await NovoUsuarioAsync();

            var link = await PedirLinkAsync(usuario.Email);

            Assert.StartsWith($"{ImperiumApiFactory.FrontendUrl}/recuperar-senha/nova-senha?", link.ToString());
            var parametros = HttpUtility.ParseQueryString(link.Query);
            Assert.Equal(usuario.UsuarioId.ToString(), parametros["uid"]);
            Assert.False(string.IsNullOrEmpty(parametros["token"]));
            Assert.DoesNotContain("email", link.Query, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("teste.local", link.Query);
        }

        [Fact]
        public async Task RedefinirSenha_LinkValido_TrocaSenhaDerrubaSessoesEAvisaPorEmail()
        {
            var usuario = await NovoUsuarioAsync();
            var sessaoAntiga = _cenario.ClienteComCookie(CookieDaSessao(await LoginAsync(usuario.Email, usuario.Senha))!);
            var parametros = HttpUtility.ParseQueryString((await PedirLinkAsync(usuario.Email)).Query);
            var corpo = new { userId = int.Parse(parametros["uid"]!), token = parametros["token"], newPassword = "NovaSenha@456" };

            var resposta = await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaRedefinir, corpo);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await sessaoAntiga.GetAsync(RotaCheck)).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await LoginAsync(usuario.Email, usuario.Senha)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await LoginAsync(usuario.Email, "NovaSenha@456")).StatusCode);
            Assert.NotNull(await _factory.Emails.EsperarAsync(usuario.Email, "Sua senha foi alterada"));

            // O link só serve uma vez
            var reuso = await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaRedefinir, corpo);
            Assert.Equal(HttpStatusCode.BadRequest, reuso.StatusCode);
            Assert.Contains(MensagemLinkInvalido, await TextoAsync(reuso));
        }

        [Fact]
        public async Task RedefinirSenha_UsuarioDoSeedComIdNegativo_Funciona()
        {
            // Os admins do seed têm id negativo: o link e a rota precisam aceitar
            const string emailDoSeed = "ceron@example.com";

            var parametros = HttpUtility.ParseQueryString((await PedirLinkAsync(emailDoSeed)).Query);
            var userId = int.Parse(parametros["uid"]!);
            Assert.True(userId < 0);

            var resposta = await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaRedefinir,
                new { userId, token = parametros["token"], newPassword = "NovaSenha@456" });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await LoginAsync(emailDoSeed, "NovaSenha@456")).StatusCode);
        }

        [Fact]
        public async Task RedefinirSenha_ContaBloqueadaPorTentativas_VoltaAEntrar()
        {
            var usuario = await NovoUsuarioAsync();
            var cliente = _cenario.ClienteAnonimo();
            for (var tentativa = 0; tentativa < 5; tentativa++)
                await LoginAsync(usuario.Email, "Errada@123", cliente: cliente);
            var parametros = HttpUtility.ParseQueryString((await PedirLinkAsync(usuario.Email)).Query);

            await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaRedefinir,
                new { userId = usuario.UsuarioId, token = parametros["token"], newPassword = "NovaSenha@456" });

            Assert.Equal(HttpStatusCode.OK, (await LoginAsync(usuario.Email, "NovaSenha@456")).StatusCode);
        }

        [Theory]
        [InlineData("isto não é base64 %%%")]
        [InlineData("dG9rZW4taW52YWxpZG8")]
        public async Task RedefinirSenha_TokenMalformadoOuInvalido_RetornaBadRequestGenerico(string token)
        {
            var usuario = await NovoUsuarioAsync();

            var resposta = await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaRedefinir,
                new { userId = usuario.UsuarioId, token, newPassword = "NovaSenha@456" });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains(MensagemLinkInvalido, await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await LoginAsync(usuario.Email, usuario.Senha)).StatusCode); // senha não mudou
        }

        [Fact]
        public async Task RedefinirSenha_UsuarioInexistente_MesmaRespostaDoTokenInvalido()
        {
            var usuario = await NovoUsuarioAsync();
            var cliente = _cenario.ClienteAnonimo();

            var tokenInvalido = await cliente.PostAsJsonAsync(RotaRedefinir,
                new { userId = usuario.UsuarioId, token = "dG9rZW4taW52YWxpZG8", newPassword = "NovaSenha@456" });
            var usuarioInexistente = await cliente.PostAsJsonAsync(RotaRedefinir,
                new { userId = int.MaxValue, token = "dG9rZW4taW52YWxpZG8", newPassword = "NovaSenha@456" });

            Assert.Equal(HttpStatusCode.BadRequest, usuarioInexistente.StatusCode);
            Assert.Equal(await TextoAsync(tokenInvalido), await TextoAsync(usuarioInexistente));
        }

        [Fact]
        public async Task RedefinirSenha_SenhaCurta_RetornaBadRequestComARegra()
        {
            var usuario = await NovoUsuarioAsync();
            var parametros = HttpUtility.ParseQueryString((await PedirLinkAsync(usuario.Email)).Query);

            var resposta = await _cenario.ClienteAnonimo().PostAsJsonAsync(RotaRedefinir,
                new { userId = usuario.UsuarioId, token = parametros["token"], newPassword = "Ab@1234" }); // 7 caracteres

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("pelo menos 8 caracteres", await TextoAsync(resposta));
            Assert.Equal(HttpStatusCode.OK, (await LoginAsync(usuario.Email, usuario.Senha)).StatusCode);
        }

        #endregion

        #region Auxiliares

        private async Task<UsuarioComSenha> NovoUsuarioAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("Auth");
            return await _cenario.UsuarioComSenhaAsync(clinica, Senha, Perfis.Veterinario);
        }

        private async Task<(int UsuarioId, string Stamp)> UsuarioComStampAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("Renovação");
            var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Tecnico);
            return (usuario.UsuarioId, await StampAsync(usuario.UsuarioId));
        }

        /// <summary>Login por um cliente novo (IP próprio), salvo quando o teste precisa repetir do mesmo cliente.</summary>
        private Task<HttpResponseMessage> LoginAsync(string email, string senha, bool lembrar = false,
            string captcha = TurnstileFalso.TokenValido, HttpClient? cliente = null) =>
            (cliente ?? _cenario.ClienteAnonimo()).PostAsJsonAsync(RotaLogin,
                new { email, senha, captchaToken = captcha, rememberMe = lembrar });

        private Task<HttpResponseMessage> EsqueciAsync(string email, string captcha = TurnstileFalso.TokenValido) =>
            _cenario.ClienteAnonimo().PostAsJsonAsync(RotaEsqueci, new { email, captchaToken = captcha });

        /// <summary>Pede a recuperação e devolve o link que foi no e-mail.</summary>
        private async Task<Uri> PedirLinkAsync(string email)
        {
            Assert.Equal(HttpStatusCode.OK, (await EsqueciAsync(email)).StatusCode);
            var mensagem = await _factory.Emails.EsperarAsync(email, "Redefinição");
            Assert.NotNull(mensagem);

            var inicio = mensagem!.Corpo.IndexOf("href='", StringComparison.Ordinal) + "href='".Length;
            var fim = mensagem.Corpo.IndexOf('\'', inicio);
            return new Uri(mensagem.Corpo[inicio..fim]);
        }

        private async Task AlterarUsuarioAsync(int usuarioId, Action<backend.Domain.Usuarios.Entities.Usuario> alterar)
        {
            await using var contexto = _factory.CriarContexto();
            var usuario = await contexto.Usuarios.FirstAsync(u => u.Id == usuarioId);
            alterar(usuario);
            await contexto.SaveChangesAsync();
        }

        private async Task<string> StampAsync(int usuarioId)
        {
            await using var contexto = _factory.CriarContexto();
            return (await contexto.Usuarios.AsNoTracking().FirstAsync(u => u.Id == usuarioId)).SecurityStamp!;
        }

        #endregion
    }
}
