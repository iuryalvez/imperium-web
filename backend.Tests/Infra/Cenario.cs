using backend.Domain.GestaoClinica.Entities;
using backend.Domain.Usuarios.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

    /// <summary>Usuário criado pelo Identity, com senha, para os testes de login.</summary>
    public record UsuarioComSenha(int UsuarioId, int VinculoId, string Email, string Senha);

    /// <summary>Usuário de clínica criado no cenário: cliente autenticado, id do vínculo, id do usuário e e-mail.</summary>
    public record UsuarioDeTeste(HttpClient Cliente, int VinculoId, int UsuarioId, string Email);

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
        public async Task<HttpClient> ClienteAsync(ClinicaDeTeste clinica, params string[] perfis) =>
            (await UsuarioDaClinicaAsync(clinica, perfis)).Cliente;

        /// <summary>Igual a <see cref="ClienteAsync"/>, mas devolve também os ids do vínculo e do usuário.</summary>
        public async Task<UsuarioDeTeste> UsuarioDaClinicaAsync(ClinicaDeTeste clinica, params string[] perfis)
        {
            await using var contexto = _factory.CriarContexto();
            var usuario = NovoUsuario(isAdmin: false);
            contexto.Usuarios.Add(usuario);
            await contexto.SaveChangesAsync();

            var vinculo = new UsuarioPerfilClinica { UsuarioId = usuario.Id, ClinicaId = clinica.Id, Perfis = perfis.ToList() };
            contexto.UsuariosPerfisClinicas.Add(vinculo);
            await contexto.SaveChangesAsync();

            var cliente = ClienteComToken(TokenDeTeste.Vinculo(vinculo.Id, usuario.Id, usuario.Nome, clinica.Id, clinica.Nome, perfis, usuario.SecurityStamp!));
            return new UsuarioDeTeste(cliente, vinculo.Id, usuario.Id, usuario.Email!);
        }

        /// <summary>Cria um admin do sistema e devolve um cliente simulando acesso à clínica com os perfis escolhidos.</summary>
        public async Task<HttpClient> ClienteAdminSimulandoAsync(ClinicaDeTeste clinica, params string[] perfis)
        {
            await using var contexto = _factory.CriarContexto();
            var admin = NovoUsuario(isAdmin: true);
            contexto.Usuarios.Add(admin);
            await contexto.SaveChangesAsync();

            return ClienteComToken(TokenDeTeste.AdminSimulando(admin.Id, admin.Nome, clinica.Id, clinica.Nome, perfis, admin.SecurityStamp!));
        }

        /// <summary>Cria um admin do sistema e devolve um cliente com o token de login (rotas api/sistema/...).</summary>
        public async Task<HttpClient> ClienteAdminSistemaAsync()
        {
            await using var contexto = _factory.CriarContexto();
            var admin = NovoUsuario(isAdmin: true);
            contexto.Usuarios.Add(admin);
            await contexto.SaveChangesAsync();

            return ClienteComToken(TokenDeTeste.AdminSistema(admin.Id, admin.Nome, admin.SecurityStamp!));
        }

        /// <summary>Cliente sem token (usuário não autenticado).</summary>
        public HttpClient ClienteAnonimo() => CriarCliente();

        /// <summary>Cliente com o token de login (antes de escolher vínculo) de um usuário que já existe no banco.</summary>
        public async Task<HttpClient> ClienteDeLoginAsync(int usuarioId)
        {
            await using var contexto = _factory.CriarContexto();
            var usuario = await contexto.Usuarios.AsNoTracking().FirstAsync(u => u.Id == usuarioId);
            return ClienteComToken(TokenDeTeste.Login(usuario.Id, usuario.Nome, usuario.IsAdmin, usuario.SecurityStamp));
        }

        /// <summary>
        /// Cria, pelo Identity, um usuário com senha (para testar login) e um vínculo na clínica com os perfis pedidos.
        /// </summary>
        public async Task<UsuarioComSenha> UsuarioComSenhaAsync(ClinicaDeTeste clinica, string senha, params string[] perfis)
        {
            var usuario = NovoUsuario(isAdmin: false);
            usuario.EmailConfirmed = true;

            using (var escopo = _factory.Services.CreateScope())
            {
                var userManager = escopo.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
                var criado = await userManager.CreateAsync(usuario, senha);
                if (!criado.Succeeded)
                    throw new InvalidOperationException(string.Join(" ", criado.Errors.Select(e => e.Description)));
            }

            await using var contexto = _factory.CriarContexto();
            var vinculo = new UsuarioPerfilClinica { UsuarioId = usuario.Id, ClinicaId = clinica.Id, Perfis = perfis.ToList() };
            contexto.UsuariosPerfisClinicas.Add(vinculo);
            await contexto.SaveChangesAsync();

            return new UsuarioComSenha(usuario.Id, vinculo.Id, usuario.Email!, senha);
        }

        /// <summary>Cliente que envia o cookie informado (ex.: o "auth_token=..." devolvido pelo login).</summary>
        public HttpClient ClienteComCookie(string cookie)
        {
            var cliente = CriarCliente();
            cliente.DefaultRequestHeaders.Add("Cookie", cookie);
            return cliente;
        }

        public HttpClient ClienteComToken(string token) => ClienteComCookie($"auth_token={token}");

        private HttpClient CriarCliente()
        {
            // Cookies controlados à mão: o token vai sempre no cabeçalho Cookie
            var cliente = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

            // Cada cliente simula um IP diferente (X-Real-IP, como o Nginx entrega),
            // para o rate limit por IP de um teste não atrapalhar o outro
            cliente.DefaultRequestHeaders.Add("X-Real-IP", IpUnico());
            return cliente;
        }

        private static string IpUnico() =>
            $"10.{Random.Shared.Next(1, 255)}.{Random.Shared.Next(0, 256)}.{Random.Shared.Next(1, 255)}";

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
