namespace backend.Dtos.UsuarioPerfilClinica
{
    public class CreateUsuarioPerfilClinicaRequestDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PerfilId { get; set; }
        public int ClinicaId { get; set; }
    }
}