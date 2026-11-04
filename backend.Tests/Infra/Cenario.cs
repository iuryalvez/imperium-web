using backend.Domain.GestaoClinica.Entities;
using backend.Domain.Usuarios.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace backend.Tests.Infra
{
    /// <summary>Perfis da clínica, como gravados em UsuarioPerfilClinica.Perfis e na claim "Perfis".</summary>
    public static class Perfis
    {
        public const string Gestor = "GESTOR";
        public const string Veterinario = "VETERINARIO";
        public const string Secretario = "SECRETARIO";
        public const string Tecnico = "TECNICO";
    }

    public record ClinicaDeTeste(int Id, string Nome);

    /// <summary>
    /// Monta os dados de cada teste direto no banco de teste: clínicas com nome único e usuários por perfil,
    /// e devolve um HttpClient já autenticado. Cada teste cria os próprios dados; nada depende de ordem.
    /// </summary>
    public class Cenario
    {
        private readonly ImperiumApiFactory _factory;

        public Cenario(ImperiumApiFactory factory)
        {
            _factory = factory;
        }

        /// <summary>Cria uma clínica ativa com nome único (ex.: "Clínica A 3f2a...").</summary>
        public async Task<ClinicaDeTeste> CriarClinicaAsync(string apelido)
        {
            var nome = $"Clínica {apelido} {Unico()}";
            await using var contexto = _factory.CriarContexto();
            var clinica = new Clinica { NomeClinica = nome, IsAtiva = true, DataCadastro = DateTime.UtcNow };
            contexto.Clinicas.Add(clinica);
            await contexto.SaveChangesAsync();
            return new ClinicaDeTeste(clinica.Id, nome);
        }

        /// <summary>Cria um usuário com vínculo na clínica com os perfis pedidos e devolve um cliente autenticado com o token de vínculo.</summary>
        public async Task<HttpClient> ClienteAsync(ClinicaDeTeste clinica, params string[] perfis)
        {
            await using var contexto = _factory.CriarContexto();
            var usuario = NovoUsuario(isAdmin: false);
            contexto.Usuarios.Add(usuario);
            await contexto.SaveChangesAsync();

            var vinculo = new UsuarioPerfilClinica { UsuarioId = usuario.Id, ClinicaId = clinica.Id, Perfis = perfis.ToList() };
            contexto.UsuariosPerfisClinicas.Add(vinculo);
            await contexto.SaveChangesAsync();

            return ClienteComToken(TokenDeTeste.Vinculo(vinculo.Id, usuario.Id, usuario.Nome, clinica.Id, clinica.Nome, perfis));
        }

        /// <summary>Cria um admin do sistema e devolve um cliente simulando acesso à clínica com os perfis escolhidos.</summary>
        public async Task<HttpClient> ClienteAdminSimulandoAsync(ClinicaDeTeste clinica, params string[] perfis)
        {
            await using var contexto = _factory.CriarContexto();
            var admin = NovoUsuario(isAdmin: true);
            contexto.Usuarios.Add(admin);
            await contexto.SaveChangesAsync();

            return ClienteComToken(TokenDeTeste.AdminSimulando(admin.Id, admin.Nome, clinica.Id, clinica.Nome, perfis));
        }

        /// <summary>Cliente sem token (usuário não autenticado).</summary>
        public HttpClient ClienteAnonimo() => CriarCliente();

        private HttpClient ClienteComToken(string token)
        {
            var cliente = CriarCliente();
            cliente.DefaultRequestHeaders.Add("Cookie", $"auth_token={token}");
            return cliente;
        }

        private HttpClient CriarCliente()
        {
            // Cookies controlados à mão: o token vai sempre no cabeçalho Cookie
            var cliente = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
            cliente.DefaultRequestHeaders.UserAgent.ParseAdd(TokenDeTeste.UserAgent);
            cliente.DefaultRequestHeaders.Add("X-Forwarded-For", TokenDeTeste.Ip);
            return cliente;
        }

        private static Usuario NovoUsuario(bool isAdmin)
        {
            var unico = Unico();
            var email = $"usuario.{unico}@teste.local";
            return new Usuario
            {
                Nome = $"Usuário de teste {unico}",
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString(),
                IsAdmin = isAdmin,
                IsAtivo = true,
                DataCadastro = DateTime.UtcNow,
                DataUltimoAcesso = DateTime.UtcNow,
            };
        }

        public static string Unico() => Guid.NewGuid().ToString("N")[..12];
    }

    /// <summary>Gera CPFs fictícios com dígitos verificadores válidos (só dígitos, 11 caracteres).</summary>
    public static class GeradorCpf
    {
        public static string Novo()
        {
            int[] digitos;
            do
            {
                digitos = Enumerable.Range(0, 9).Select(_ => Random.Shared.Next(10)).ToArray();
            } while (digitos.Distinct().Count() == 1); // evita 111.111.111-xx e afins

            var primeiro = Verificador(digitos, 10);
            var segundo = Verificador(digitos.Append(primeiro).ToArray(), 11);
            return string.Concat(digitos.Append(primeiro).Append(segundo));
        }

        private static int Verificador(int[] digitos, int pesoInicial)
        {
            var soma = digitos.Select((d, i) => d * (pesoInicial - i)).Sum();
            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
