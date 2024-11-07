namespace backend.Dtos.UsuarioPerfilClinica
{
    public class UsuarioPerfilClinicaDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PerfilId { get; set; }
        public string NomePerfil { get; set; }
        public int ClinicaId { get; set; }
        public string NomeClinica { get; set; }
    }
}