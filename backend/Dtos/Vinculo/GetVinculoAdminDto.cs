namespace backend.Dtos.Vinculo
{
    public class GetVinculoAdminDto
    {
        public int ClinicaId { get; set; }
        public string? NomeClinica { get; set; }
        // lista para armazenar os perfis do usuário na clínica
        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}