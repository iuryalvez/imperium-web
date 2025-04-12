using Microsoft.AspNetCore.Identity;

namespace backend.Dtos.Usuario
{
    public class EditarUsuarioClinicaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public int UpcId { get; set; }
        public string? Nome { get; set; }
        public List<string>? Perfis { get; set; }
    }
}