using System.Security.Claims;

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
    }
}