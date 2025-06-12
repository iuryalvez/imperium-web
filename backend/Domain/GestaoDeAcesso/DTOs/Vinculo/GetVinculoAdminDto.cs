namespace backend.Domain.GestaoDeAcesso.DTOs
{
    public class GetVinculoAdminDto
    {
        public int ClinicaId { get; set; }

        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}