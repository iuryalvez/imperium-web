using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class GetUsuariosClinicaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public int UpcId { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public List<string> Perfis { get; set; } = new(); // Agora armazena uma lista de perfis
    }
}