using System.Security.Claims;
using System.Text.Json;

namespace backend.Services {
    public static class AppExtension {
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
            var perfisClaim = user.FindFirst("Perfis")?.Value;
            Console.WriteLine($"[DEBUG] Claim 'Perfis' encontrada: {perfisClaim}");

            if (string.IsNullOrEmpty(perfisClaim))
            {
                Console.WriteLine("[DEBUG] Nenhuma claim 'Perfis' encontrada ou está vazia.");
                return false;
            }

            try
            {
                var perfisAtivos = JsonSerializer.Deserialize<Dictionary<string, bool>>(perfisClaim);
                if (perfisAtivos == null)
                {
                    Console.WriteLine("[DEBUG] A desserialização da claim 'Perfis' retornou null.");
                    return false;
                }

                Console.WriteLine($"[DEBUG] Perfis ativos desserializados: {string.Join(", ", perfisAtivos.Select(p => $"{p.Key}: {p.Value}"))}");

                bool possuiPerfil = perfisNecessarios.Any(perfil => perfisAtivos.TryGetValue(perfil, out bool ativo) && ativo);
                Console.WriteLine($"[DEBUG] Resultado da verificação de perfil: {possuiPerfil}");

                return possuiPerfil;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ERRO] Falha ao desserializar a claim 'Perfis': {ex.Message}");
                return false;
            }
        }

        public static bool TemAlgumPerfil(this ClaimsPrincipal user)
        {
            var perfisClaim = user.FindFirst("Perfis")?.Value;
            Console.WriteLine($"[DEBUG] Claim 'Perfis' encontrada: {perfisClaim}");

            if (string.IsNullOrEmpty(perfisClaim))
            {
                Console.WriteLine("[DEBUG] Nenhuma claim 'Perfis' encontrada ou está vazia.");
                return false;
            }

            try
            {
                var perfisAtivos = JsonSerializer.Deserialize<Dictionary<string, bool>>(perfisClaim);
                if (perfisAtivos == null)
                {
                    Console.WriteLine("[DEBUG] A desserialização da claim 'Perfis' retornou null.");
                    return false;
                }

                Console.WriteLine($"[DEBUG] Perfis ativos desserializados: {string.Join(", ", perfisAtivos.Select(p => $"{p.Key}: {p.Value}"))}");

                bool temAlgum = perfisAtivos.Values.Any(ativo => ativo);
                Console.WriteLine($"[DEBUG] Resultado da verificação de existência de algum perfil ativo: {temAlgum}");

                return temAlgum;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ERRO] Falha ao desserializar a claim 'Perfis': {ex.Message}");
                return false;
            }
        }
    }
}