namespace backend.Domain.Usuarios.DTOs.Vinculo
{
    public class GetVinculoDto
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int ClinicaId { get; set; }

        public string? NomeClinica { get; set; }
        // lista para armazenar os perfis do usuário na clínica

        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}