using System.Security.Claims;
using System.Text.Json;

namespace backend.Domain.Auth.Extensions {
    public static class AuthExtension {
        /// <summary>Marcador de vínculo desativado na lista de perfis (UsuarioPerfilClinica.Perfis).</summary>
        public const string PerfilInativo = "INATIVO";

        /// <summary>Perfis que um vínculo pode ter na clínica (além do marcador INATIVO).</summary>
        public static readonly string[] PerfisDaClinica = { "GESTOR", "VETERINARIO", "SECRETARIO", "TECNICO" };

        /// <summary>
        /// Obtém o valor de uma claim específica e converte para o tipo desejado.
        /// </summary>
        /// <typeparam name="T">Tipo esperado do valor da claim.</typeparam>
        /// <param name="user">O objeto ClaimsPrincipal do qual obter a claim.</param>
        /// <param name="claimType">O tipo da claim a ser buscada.</param>
        /// <returns>O valor convertido da claim, ou um default(T) se inválido.</returns>
        /// <summary>
        /// Id do USUÁRIO logado (AspNetUsers.Id), para gravar quem fez a ação (ex.: Consulta.UsuarioId).
        /// No token de vínculo o NameIdentifier é o id do vínculo (UsuarioPerfilClinica), por isso existe a claim "usuarioId".
        /// Sem essa claim, só o token de login e o do admin têm o id do usuário no NameIdentifier.
        /// </summary>
        /// <returns>O id do usuário, ou nulo se não for possível identificá-lo.</returns>
        public static int? GetUsuarioId(this ClaimsPrincipal user)
        {
            if (int.TryParse(user.FindFirst("usuarioId")?.Value, out var usuarioId)) return usuarioId;

            var isAdmin = bool.TryParse(user.FindFirst("isAdmin")?.Value, out var admin) && admin;
            var isLogin = user.FindFirst("acessoLogin")?.Value == "true";
            if ((isAdmin || isLogin) && int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)) return id;

            return null;
        }

        public static T GetClaimValue<T>(this ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);

            if (claim == null)
            {
                return default; // Retorna o valor padrão do tipo (exemplo: 0 para int, null para string)
            }

            try
            {
                return (T)Convert.ChangeType(claim.Value, typeof(T));
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// Confere se o token tem pelo menos um dos perfis da clínica pedidos (claim "Perfis").
        /// Não há atalho para admin: o admin simulando acesso a uma clínica tem só os perfis que escolheu na simulação.
        /// Vínculo com o perfil INATIVO não passa em nenhuma policy da clínica, mesmo tendo outros perfis.
        /// </summary>
        /// <summary>
        /// Perfis que podem ver os dados de contato e documentos do tutor (CPF, telefone, e-mail).
        /// O TECNICO vê só o nome do tutor (LGPD: ele não precisa desses dados).
        /// </summary>
        public static bool PodeVerDadosDoTutor(this ClaimsPrincipal user) =>
            user.TemPerfil("GESTOR", "VETERINARIO", "SECRETARIO");

        public static bool TemPerfil(this ClaimsPrincipal user, params string[] perfisNecessarios)
        {
            try
            {
                var perfisClaim = user.FindFirst("Perfis")?.Value;

                if (string.IsNullOrEmpty(perfisClaim))
                {
                    return false;
                }

                var perfisDeserialize = JsonSerializer.Deserialize<List<string>>(perfisClaim) ?? new List<string>();

                if (perfisDeserialize.Contains(PerfilInativo, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }

                bool possuiPerfil = perfisNecessarios.Any(perfil => perfisDeserialize.Contains(perfil));

                return possuiPerfil;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TemAlgumPerfil(this ClaimsPrincipal user)
        {
            try
            {
                var isAdminClaim = user.FindFirst("isAdmin")?.Value;
                if (bool.TryParse(isAdminClaim, out bool isAdmin) && isAdmin)
                {
                    return true;
                }

                var perfisClaim = user.FindFirst("Perfis")?.Value;

                if (string.IsNullOrEmpty(perfisClaim))
                {
                    return false;
                }

                var perfisDeserialize = JsonSerializer.Deserialize<List<string>>(perfisClaim);
                bool temAlgum = perfisDeserialize.Any();

                return temAlgum;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}