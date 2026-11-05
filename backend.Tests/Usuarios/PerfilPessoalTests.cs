using System.Net;
using System.Net.Http.Json;
using backend.Domain.Usuarios.DTOs.PerfilPessoal;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Usuarios
{
    /// <summary>
    /// Testes de integração do perfil pessoal (api/sistema/perfil): o usuário lê e edita os próprios dados
    /// tanto com o token de login quanto com o token de vínculo (dentro da clínica, onde o NameIdentifier
    /// é o id do vínculo e o id do usuário vem da claim usuarioId).
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class PerfilPessoalTests
    {
        private const string RotaPerfil = "/api/sistema/perfil";

        private readonly Cenario _cenario;

        public PerfilPessoalTests(ImperiumApiFactory factory)
        {
            _cenario = new Cenario(factory);
        }

        [Fact]
        public async Task GetMeuPerfil_TokenDeVinculo_RetornaDadosDoProprioUsuario()
        {
            var usuario = await UsuarioComIdDiferenteDoVinculoAsync();

            var resposta = await usuario.Cliente.GetAsync(RotaPerfil);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var perfil = (await LerAsync<ResultadoComDados<GetMeuPerfilDto>>(resposta)).Data;
            Assert.Equal(usuario.UsuarioId, perfil.Id);
            Assert.Equal(usuario.Email, perfil.Email);
        }

        [Fact]
        public async Task UpdateMeuPerfil_TokenDeVinculo_AlteraOProprioUsuario()
        {
            var usuario = await UsuarioComIdDiferenteDoVinculoAsync();
            var novaFormacao = $"Formação {Cenario.Unico()}";

            var resposta = await usuario.Cliente.PutAsJsonAsync(RotaPerfil, new UpdateMeuPerfilDto { Formacao = novaFormacao });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var perfil = (await LerAsync<ResultadoComDados<GetMeuPerfilDto>>(await usuario.Cliente.GetAsync(RotaPerfil))).Data;
            Assert.Equal(usuario.UsuarioId, perfil.Id);
            Assert.Equal(novaFormacao, perfil.Formacao);
            Assert.Equal(usuario.Email, perfil.Email); // edição parcial: o e-mail continua o mesmo
        }

        [Fact]
        public async Task GetMeuPerfil_TokenDeLoginDoAdmin_RetornaOk()
        {
            var admin = await _cenario.ClienteAdminSistemaAsync();

            var resposta = await admin.GetAsync(RotaPerfil);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task GetMeuPerfil_SemToken_RetornaUnauthorized()
        {
            var resposta = await _cenario.ClienteAnonimo().GetAsync(RotaPerfil);

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        /// <summary>
        /// Usuário da clínica cujo id de usuário difere do id do vínculo, para o teste pegar a troca entre os dois
        /// (os ids vêm de sequências diferentes e quase sempre já diferem).
        /// </summary>
        private async Task<UsuarioDeTeste> UsuarioComIdDiferenteDoVinculoAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("Perfil");
            for (var tentativa = 0; tentativa < 5; tentativa++)
            {
                var usuario = await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Veterinario);
                if (usuario.UsuarioId != usuario.VinculoId) return usuario;
            }
            throw new InvalidOperationException("Não foi possível criar um usuário com id diferente do vínculo.");
        }
    }
}
