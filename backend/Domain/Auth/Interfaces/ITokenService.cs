using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace backend.Domain.Auth.Interfaces
{
    public interface ITokenService
    {
        /// <summary>Nome do cookie que carrega o token (configuração Cookie:Name).</summary>
        string CookieName { get; }

        /// <summary>
        /// Token de login (antes de escolher o vínculo). Com <paramref name="rememberMe"/> informado é um login novo;
        /// nulo herda "lembrar-me" e o início da sessão do token atual (troca de vínculo).
        /// </summary>
        string GenerateTokenLogin(string usuarioId, bool? rememberMe = null);

        /// <summary>Token de vínculo do usuário na clínica. Herda "lembrar-me" e o início da sessão do token atual.</summary>
        string GenerateTokenVinculo(string usuarioPerfilClinicaId);

        /// <summary>Token do admin simulando acesso a uma clínica com os perfis escolhidos.</summary>
        string GenerateTokenVinculoAdmin(int clinicaId, List<string> perfisAtivos);

        void SetCookieToken(HttpContext context, string token, bool rememberMe = false);

        void ClearCookieToken(HttpContext context);

        /// <summary>Renova o token perto de expirar, até o teto absoluto da sessão.</summary>
        Task RenovaToken(TokenValidatedContext context);

        /// <summary>
        /// Reemite o cookie da sessão atual com o carimbo de segurança novo do usuário.
        /// Usado depois que o próprio usuário troca a senha, para ele não ser deslogado junto com as outras sessões.
        /// </summary>
        Task ReemitirSessaoAtualAsync(HttpContext context);
    }
}
