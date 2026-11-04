using System.Security.Claims;
using System.Text.Json;

namespace backend.Domain.Auth.Extensions {
    public static class AuthExtension {
        /// <summary>Marcador de vínculo desativado na lista de perfis (UsuarioPerfilClinica.Perfis).</summary>
        public const string PerfilInativo = "INATIVO";

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
        /// Tokens antigos sem essa claim: só o do admin tem o id do usuário no NameIdentifier.
        /// </summary>
        /// <returns>O id do usuário, ou nulo se não for possível identificá-lo.</returns>
        public static int? GetUsuarioId(this ClaimsPrincipal user)
        {
            if (int.TryParse(user.FindFirst("usuarioId")?.Value, out var usuarioId)) return usuarioId;

            var isAdmin = bool.TryParse(user.FindFirst("isAdmin")?.Value, out var admin) && admin;
            if (isAdmin && int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var adminId)) return adminId;

            return null;
        }

        public static T GetClaimValue<T>(this ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);

            if (claim == null)
            {
                Console.WriteLine($"Erro: Claim '{claimType}' não encontrada no token.");
                return default; // Retorna o valor padrão do tipo (exemplo: 0 para int, null para string)
            }

            try
            {
                return (T)Convert.ChangeType(claim.Value, typeof(T));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao converter a claim '{claimType}' para o tipo {typeof(T).Name}: {ex.Message}");
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
                Console.WriteLine($"[DEBUG] Claim 'Perfis' encontrada: {perfisClaim}");

                if (string.IsNullOrEmpty(perfisClaim))
                {
                    Console.WriteLine("[DEBUG] Nenhuma claim 'Perfis' encontrada ou está vazia.");
                    return false;
                }

                var perfisDeserialize = JsonSerializer.Deserialize<List<string>>(perfisClaim) ?? new List<string>();

                if (perfisDeserialize.Contains(PerfilInativo, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine("[DEBUG] Vínculo inativo: acesso negado a todas as policies da clínica.");
                    return false;
                }

                bool possuiPerfil = perfisNecessarios.Any(perfil => perfisDeserialize.Contains(perfil));
                Console.WriteLine($"[DEBUG] Resultado da verificação de perfil: {possuiPerfil}");

                return possuiPerfil;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao ler as claims de perfil: {ex.Message}");
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
                    Console.WriteLine("[DEBUG] Usuário é administrador. Considerado como tendo perfil.");
                    return true;
                }

                var perfisClaim = user.FindFirst("Perfis")?.Value;
                Console.WriteLine($"[DEBUG] Claim 'Perfis' encontrada: {perfisClaim}");

                if (string.IsNullOrEmpty(perfisClaim))
                {
                    Console.WriteLine("[DEBUG] Nenhuma claim 'Perfis' encontrada ou está vazia.");
                    return false;
                }

                var perfisDeserialize = JsonSerializer.Deserialize<List<string>>(perfisClaim);
                bool temAlgum = perfisDeserialize.Any();

                Console.WriteLine($"[DEBUG] Resultado da verificação de existência de algum perfil ativo: {temAlgum}");
                return temAlgum;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao ler a claim 'Perfis': {ex.Message}");
                return false;
            }
        }
    }
}