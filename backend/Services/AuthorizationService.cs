using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> VerificarPermissaoClinica(int clinicaId)
        {
            // Obter o ID do vínculo do usuário com a clínica a partir do token JWT
            var tokenUpcId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("UsuarioPerfilClinicaId")?.Value);

            var usuarioPerfilClinica = await _context.UsuariosPerfisClinicas
                .FirstOrDefaultAsync(upc => upc.Id == tokenUpcId);

            // Verifica se o usuário está associado à clínica
            return usuarioPerfilClinica != null && usuarioPerfilClinica.ClinicaId == clinicaId;
        }
    }
}
