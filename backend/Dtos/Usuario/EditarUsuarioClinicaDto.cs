namespace backend.Dtos.Usuario
{
    public class EditarUsuarioClinicaDto
    {
        public int Id { get; set; }
        public int UpcId { get; set; }
        public string? Perfil { get; set; }
        public bool Habilitado { get; set; }    
    }
}