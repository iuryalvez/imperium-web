namespace backend.Dtos.Vinculo
{
    public class VinculoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ClinicaId { get; set; }
        public string? NomeClinica { get; set; }
        // Dicionário para armazenar os perfis do usuário na clínica
        // Dicionário de perfis com valores booleanos
        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}