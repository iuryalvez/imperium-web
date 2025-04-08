namespace backend.Dtos.Vinculo
{
    public class VinculoAdminDto
    {
        public int ClinicaId { get; set; }
        public string? NomeClinica { get; set; }
        // lista para armazenar os perfis do usuário na clínica
        // lista de perfis com valores booleanos
        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}