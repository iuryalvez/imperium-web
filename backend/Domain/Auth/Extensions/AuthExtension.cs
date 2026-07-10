using System.Security.Claims;
using System.Text.Json;

namespace backend.Domain.Auth.Extensions {
    public static class AuthExtension {
        /// <summary>
        /// Obtém o valor de uma claim específica e converte para o tipo desejado.
        /// </summary>
        /// <typeparam name="T">Tipo esperado do valor da claim.</typeparam>
        /// <param name="user">O objeto ClaimsPrincipal do qual obter a claim.</param>
        /// <param name="claimType">O tipo da claim a ser buscada.</param>
        /// <returns>O valor convertido da claim, ou um default(T) se inválido.</returns>
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

        public static bool TemPerfil(this ClaimsPrincipal user, params string[] perfisNecessarios)
        {
            try
            {
                // Se "isAdmin" estiver entre os perfis necessários e o usuário for admin, retorna true imediatamente
                if (perfisNecessarios.Contains("isAdmin", StringComparer.OrdinalIgnoreCase))
                {
                    var isAdminClaim = user.FindFirst("isAdmin")?.Value;
                    if (bool.TryParse(isAdminClaim, out bool isAdmin) && isAdmin)
                    {
                        Console.WriteLine("[DEBUG] Usuário é administrador e 'isAdmin' foi requisitado como perfil.");
                        return true;
                    }
                }

                var perfisClaim = user.FindFirst("Perfis")?.Value;
                Console.WriteLine($"[DEBUG] Claim 'Perfis' encontrada: {perfisClaim}");

                if (string.IsNullOrEmpty(perfisClaim))
                {
                    Console.WriteLine("[DEBUG] Nenhuma claim 'Perfis' encontrada ou está vazia.");
                    return false;
                }

                var perfisDeserialize = JsonSerializer.Deserialize<List<string>>(perfisClaim);

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