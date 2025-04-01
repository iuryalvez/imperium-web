namespace backend.Dtos.Vinculo
{
    public class VinculoAdminDto
    {
        public int ClinicaId { get; set; }
        public string? NomeClinica { get; set; }
        // Dicionário para armazenar os perfis do usuário na clínica
        // Dicionário de perfis com valores booleanos
        public Dictionary<string, bool>? Perfis { get; set; } // Definir no controlador.
    }
}