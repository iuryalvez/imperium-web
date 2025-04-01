using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class GetUsuariosDto : IdentityUser<int>
    {
        public int ClinicaId { get; set; }
        public string? NomeClinica { get; set; }
        public string? Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public List<string> Perfis { get; set; } = new(); // Agora armazena uma lista de perfis
    }
}
